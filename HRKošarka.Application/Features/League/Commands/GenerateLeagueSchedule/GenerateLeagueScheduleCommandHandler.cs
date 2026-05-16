using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.GenerateLeagueSchedule
{
    public class GenerateLeagueScheduleCommandHandler : IRequestHandler<GenerateLeagueScheduleCommand, CommandResponse<int>>
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly IGenericRepository<Match> _matchRepository;
        private readonly IAppLogger<GenerateLeagueScheduleCommandHandler> _logger;

        public GenerateLeagueScheduleCommandHandler(
            ILeagueRepository leagueRepository,
            IGenericRepository<Match> matchRepository,
            IAppLogger<GenerateLeagueScheduleCommandHandler> logger)
        {
            _leagueRepository = leagueRepository;
            _matchRepository = matchRepository;
            _logger = logger;
        }

        public async Task<CommandResponse<int>> Handle(GenerateLeagueScheduleCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to generate schedule for league {LeagueId}", request.LeagueId);

            var league = await _leagueRepository.GetLeagueWithDetailsAsync(request.LeagueId, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.League), request.LeagueId);

            if (!league.IsActive)
                throw new BadRequestException("Cannot generate a schedule for an inactive league.");

            if (league.ScheduleGenerated)
                throw new BadRequestException("Schedule has already been generated for this league.");

            // TODO: Cup bracket generation is not yet implemented.
            //
            // Cup (knockout/elimination) requires a fundamentally different algorithm:
            //   1. GenerateCupDraw command — randomly shuffles teams into a bracket, creates only
            //      Round 1 matches. Subsequent rounds cannot be pre-generated because pairings
            //      depend on match results (winners advance).
            //   2. AdvanceCupWinner — called when a Cup match result is confirmed. Checks whether
            //      all matches in the current round are completed. If yes, generates the next round's
            //      match records using the winners from each pairing.
            //   3. Round naming for Cup differs: "Round of 16", "Quarter-final", "Semi-final", "Final"
            //      (derived from remaining team count, not sequential numbering).
            //   4. Byes: if team count is not a power of 2, some teams skip Round 1. No Match record
            //      is created for a bye; the team is inserted directly into Round 2 as a seeded winner.
            //   5. NumberOfRounds on the League entity is irrelevant for Cup and should be ignored.
            //
            // When to build: implement alongside the Match Results feature. The AdvanceCupWinner
            // step is a natural part of saving a result — once all round matches are done, the
            // system auto-generates the next round. The date-assignment infrastructure (Saturdays +
            // break-skipping) reused from this handler applies unchanged to Cup rounds.
            //
            // See CLAUDE.md → "Feature: Cup Bracket Generation (TODO)" for full spec.
            if (league.CompetitionType == CompetitionType.Cup)
                throw new BadRequestException(
                    "Cup bracket generation is not yet supported. " +
                    "Create the league as Competition Type 'League' to use round-robin scheduling.");

            var registeredTeams = await _leagueRepository.GetLeagueTeamsAsync(request.LeagueId, cancellationToken);
            if (registeredTeams.Count < 2)
                throw new BadRequestException("At least 2 teams must be registered before generating a schedule.");

            var breaks = await _leagueRepository.GetLeagueBreaksAsync(request.LeagueId, cancellationToken);

            // Shuffle teams so the bye and home/away rotation are not tied to registration order
            var teamIds = registeredTeams.Select(t => (Guid?)t.TeamId).ToList();
            var rng = new Random();
            for (int i = teamIds.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (teamIds[i], teamIds[j]) = (teamIds[j], teamIds[i]);
            }
            if (teamIds.Count % 2 != 0)
                teamIds.Add(null);

            int n = teamIds.Count;
            int roundsPerHalf = n - 1;
            int totalRounds = roundsPerHalf * league.NumberOfRounds;

            // Generate round-robin pairings using rotation algorithm
            var allRounds = new List<List<(Guid Home, Guid Away)>>();
            var slots = teamIds.ToList();

            for (int round = 0; round < roundsPerHalf; round++)
            {
                var roundMatches = new List<(Guid Home, Guid Away)>();
                for (int i = 0; i < n / 2; i++)
                {
                    var home = slots[i];
                    var away = slots[n - 1 - i];
                    if (home.HasValue && away.HasValue)
                        roundMatches.Add((home.Value, away.Value));
                }
                allRounds.Add(roundMatches);

                // Rotate: keep slots[0] fixed, rotate slots[1..n-1] clockwise
                var last = slots[n - 1];
                for (int i = n - 1; i > 1; i--)
                    slots[i] = slots[i - 1];
                slots[1] = last;
            }

            // For double round-robin: add reversed fixtures (home/away swapped)
            if (league.NumberOfRounds == 2)
            {
                var secondHalf = allRounds
                    .Select(r => r.Select(m => (m.Away, m.Home)).ToList())
                    .ToList();
                allRounds.AddRange(secondHalf);
            }

            // Assign dates: one Saturday per round, skipping break weeks
            var matches = new List<Match>();
            var currentDate = league.StartDate;
            var breakRanges = breaks.Select(b => (b.StartDate, b.EndDate)).ToList();

            for (int roundIndex = 0; roundIndex < allRounds.Count; roundIndex++)
            {
                var roundDate = FindNextValidSaturday(currentDate, breakRanges);
                currentDate = roundDate.AddDays(1);

                int roundNumber = roundIndex + 1;

                // Swap home/away on even rounds within each half so every team alternates H/A.
                // roundWithinHalf is 1-indexed; even rounds get swapped.
                int roundWithinHalf = (roundIndex % roundsPerHalf) + 1;
                bool swapHomeAway = roundWithinHalf % 2 == 0;

                foreach (var (generatedHome, generatedAway) in allRounds[roundIndex])
                {
                    var actualHome = swapHomeAway ? generatedAway : generatedHome;
                    var actualAway = swapHomeAway ? generatedHome : generatedAway;

                    matches.Add(new Match
                    {
                        LeagueId = request.LeagueId,
                        HomeTeamId = actualHome,
                        AwayTeamId = actualAway,
                        Round = roundNumber,
                        RoundName = $"Round {roundNumber}",
                        DefaultScheduledDate = roundDate,
                        ActualScheduledDate = roundDate,
                        Status = MatchStatus.Scheduled,
                        SchedulingStatus = SchedulingStatus.Default,
                        LastSchedulingUpdate = DateTime.Now
                    });
                }
            }

            await _matchRepository.CreateRangeAsync(matches, cancellationToken);

            league.ScheduleGenerated = true;
            await _leagueRepository.UpdateAsync(league, cancellationToken);

            _logger.LogInformation(
                "Generated {Count} matches across {Rounds} rounds for league {LeagueId}",
                matches.Count, allRounds.Count, request.LeagueId);

            return CommandResponse<int>.Success(matches.Count,
                $"Schedule generated: {matches.Count} matches across {allRounds.Count} rounds.");
        }

        private static DateTime FindNextValidSaturday(DateTime from, List<(DateTime Start, DateTime End)> breaks)
        {
            // Find the first Saturday on or after 'from'
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)from.DayOfWeek + 7) % 7;
            if (daysUntilSaturday == 0 && from.TimeOfDay >= TimeSpan.FromHours(19))
                daysUntilSaturday = 7; // already past 19:00 on a Saturday, use next week
            var candidate = from.Date.AddDays(daysUntilSaturday).AddHours(19);

            // Skip any week where Monday–Sunday overlaps a break
            while (IsWeekInBreak(candidate, breaks))
                candidate = candidate.AddDays(7);

            return candidate;
        }

        private static bool IsWeekInBreak(DateTime saturday, List<(DateTime Start, DateTime End)> breaks)
        {
            // Week containing this Saturday: Monday (saturday - 5) to Sunday (saturday + 1)
            var weekStart = saturday.Date.AddDays(-5);
            var weekEnd = saturday.Date.AddDays(1);

            return breaks.Any(b => b.Start.Date <= weekEnd && b.End.Date >= weekStart);
        }
    }
}
