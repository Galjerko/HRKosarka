using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using HRKošarka.Domain.Helpers;
using MediatR;
using System.Text.Json;
using DomainMatch = HRKošarka.Domain.Match;

namespace HRKošarka.Application.Features.League.Commands.GeneratePlayoff
{
    public class GeneratePlayoffCommandHandler : IRequestHandler<GeneratePlayoffCommand, CommandResponse<bool>>
    {
        private record SeedEntry(Guid TeamId, int Seed);

        private readonly ILeagueRepository _leagueRepository;
        private readonly ILeagueStandingRepository _standingRepository;
        private readonly IPlayoffRepository _playoffRepository;

        public GeneratePlayoffCommandHandler(
            ILeagueRepository leagueRepository,
            ILeagueStandingRepository standingRepository,
            IPlayoffRepository playoffRepository)
        {
            _leagueRepository = leagueRepository;
            _standingRepository = standingRepository;
            _playoffRepository = playoffRepository;
        }

        public async Task<CommandResponse<bool>> Handle(GeneratePlayoffCommand request, CancellationToken ct)
        {
            var validationResult = await new GeneratePlayoffCommandValidator().ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid playoff generation data", validationResult);

            var league = await _leagueRepository.GetLeagueWithDetailsAsync(request.LeagueId, ct)
                ?? throw new NotFoundException(nameof(Domain.League), request.LeagueId);

            if (!league.HasPlayoff)
                throw new BadRequestException("This league is not configured for a playoff.");

            if (!league.ScheduleGenerated)
                throw new BadRequestException("Regular-season schedule must be generated before creating a playoff.");

            if (league.PlayoffGenerated)
                throw new BadRequestException("Playoff has already been generated for this league.");

            if (!league.PlayoffTeamCount.HasValue || !IsValidTeamCount(league.PlayoffTeamCount.Value))
                throw new BadRequestException("League must have a valid PlayoffTeamCount (2, 4, or 8).");

            var teamCount = league.PlayoffTeamCount.Value;
            int expectedRounds = GetRoundCount(teamCount);

            if (request.RoundWinsNeeded.Count != expectedRounds)
                throw new BadRequestException($"WinsNeeded must be specified for all {expectedRounds} round(s).");

            if (await _leagueRepository.HasUnfinishedRegularSeasonMatchesAsync(league.Id, ct))
                throw new BadRequestException("All regular-season matches must be completed before generating the playoff.");

            var standings = await _standingRepository.GetByLeagueAsync(league.Id, ct);
            var qualified = standings.Where(s => s.MatchesPlayed > 0).Take(teamCount).ToList();

            if (qualified.Count < teamCount)
                throw new BadRequestException(
                    $"Need at least {teamCount} teams with games played, but only {qualified.Count} qualify.");

            // Normalize admin-supplied start date to 19:00; breaks are NOT applied to playoff scheduling
            var firstPlayoffDate = request.PlayoffStartDate.Date.AddHours(19);
            var capDate = (league.PlayoffEndDate ?? league.EndDate).Date.AddHours(19);

            if (firstPlayoffDate < league.StartDate.Date.AddHours(19))
                throw new BadRequestException(
                    "The playoff start date cannot be before the league start date.");

            if (firstPlayoffDate > capDate)
                throw new BadRequestException(
                    "The playoff start date falls after the league end date. " +
                    "Adjust the playoff end date or choose an earlier start date.");

            var seeds = qualified
                .Select((s, idx) => new SeedEntry(s.TeamId, idx + 1))
                .ToList();

            var allSeries = BuildFullBracket(seeds, teamCount, request.RoundWinsNeeded, request.Include3rdPlace,
                league.Id, firstPlayoffDate, capDate, league.DefaultVenue);

            var winsDict = new Dictionary<string, int>();
            for (int i = 0; i < request.RoundWinsNeeded.Count; i++)
                winsDict[(i + 1).ToString()] = request.RoundWinsNeeded[i];
            if (request.Include3rdPlace && request.RoundWinsNeeded.Count > 0)
                winsDict["3rdPlace"] = request.RoundWinsNeeded.Last();

            league.PlayoffRoundWinsNeeded = JsonSerializer.Serialize(winsDict);
            league.PlayoffGenerated = true;

            await _playoffRepository.CreateInitialBracketAsync(allSeries, league, ct);

            return CommandResponse<bool>.Success(true, $"Playoff bracket generated with {allSeries.Count} series.");
        }

        private List<PlayoffSeries> BuildFullBracket(
            List<SeedEntry> seeds,
            int teamCount,
            List<int> winsNeededPerRound,
            bool include3rdPlace,
            Guid leagueId,
            DateTime firstDate,
            DateTime capDate,
            string? defaultVenue)
        {
            var allSeries = new List<PlayoffSeries>();
            int totalRounds = GetRoundCount(teamCount);
            var round1Pairings = GetRound1Pairings(teamCount);
            var round1Series = new List<PlayoffSeries>();
            int winsNeeded = winsNeededPerRound[0];

            for (int i = 0; i < round1Pairings.Count; i++)
            {
                var (topSeed, bottomSeed) = round1Pairings[i];
                var homeTeam = seeds.First(s => s.Seed == topSeed);
                var awayTeam = seeds.First(s => s.Seed == bottomSeed);

                var series = new PlayoffSeries
                {
                    LeagueId = leagueId,
                    RoundNumber = 1,
                    RoundName = GetRoundName(1, totalRounds),
                    SeriesNumber = i + 1,
                    WinsNeeded = winsNeeded,
                    HomeTeamId = homeTeam.TeamId,
                    AwayTeamId = awayTeam.TeamId,
                    HomeSeedNumber = topSeed,
                    AwaySeedNumber = bottomSeed,
                    HomeFeederSeriesId = null,
                    AwayFeederSeriesId = null
                };

                // Pre-generate the minimum guaranteed games (WinsNeeded) with alternating venues and +2/+3 day spacing
                var gameSlots = PlayoffSeriesScheduler.GenerateInitialGames(
                    homeTeam.TeamId, awayTeam.TeamId, winsNeeded, firstDate, winsNeeded);

                foreach (var slot in gameSlots)
                {
                    if (slot.Date > capDate)
                        throw new BadRequestException(
                            $"Playoff scheduling would exceed the configured end date ({capDate:dd.MM.yyyy}). " +
                            "Adjust the playoff end date or choose an earlier start date.");

                    series.Matches.Add(new DomainMatch
                    {
                        LeagueId = leagueId,
                        HomeTeamId = slot.HomeTeamId,
                        AwayTeamId = slot.AwayTeamId,
                        Round = 1,
                        RoundName = series.RoundName,
                        DefaultScheduledDate = slot.Date,
                        ActualScheduledDate = slot.Date,
                        Status = MatchStatus.Scheduled,
                        SchedulingStatus = SchedulingStatus.Default,
                        LastSchedulingUpdate = DateTime.Now,
                        VenueOverride = defaultVenue,
                        PlayoffSeriesId = series.Id
                    });
                }

                round1Series.Add(series);
                allSeries.Add(series);
            }

            if (totalRounds == 1)
                return allSeries;

            // Build subsequent round stubs — teams unknown, no matches yet
            var previousRoundSeries = round1Series;

            for (int round = 2; round <= totalRounds; round++)
            {
                int roundWins = winsNeededPerRound[round - 1];
                string roundName = GetRoundName(round, totalRounds);
                var thisRoundSeries = new List<PlayoffSeries>();
                int seriesCount = previousRoundSeries.Count / 2;

                for (int i = 0; i < seriesCount; i++)
                {
                    var homeFeeder = previousRoundSeries[i * 2];
                    var awayFeeder = previousRoundSeries[i * 2 + 1];

                    var stub = new PlayoffSeries
                    {
                        LeagueId = leagueId,
                        RoundNumber = round,
                        RoundName = roundName,
                        SeriesNumber = i + 1,
                        WinsNeeded = roundWins,
                        HomeTeamId = null,
                        AwayTeamId = null,
                        HomeFeederSeriesId = homeFeeder.Id,
                        AwayFeederSeriesId = awayFeeder.Id
                    };

                    thisRoundSeries.Add(stub);
                    allSeries.Add(stub);
                }

                previousRoundSeries = thisRoundSeries;
            }

            if (include3rdPlace && totalRounds >= 2)
            {
                var semifinals = allSeries
                    .Where(s => s.RoundNumber == totalRounds - 1)
                    .OrderBy(s => s.SeriesNumber)
                    .ToList();

                if (semifinals.Count == 2)
                {
                    var thirdPlace = new PlayoffSeries
                    {
                        LeagueId = leagueId,
                        RoundNumber = totalRounds + 1,
                        RoundName = "3rd Place",
                        SeriesNumber = 1,
                        WinsNeeded = winsNeededPerRound.Last(),
                        HomeTeamId = null,
                        AwayTeamId = null,
                        HomeFeederSeriesId = semifinals[0].Id,
                        AwayFeederSeriesId = semifinals[1].Id
                    };
                    allSeries.Add(thirdPlace);
                }
            }

            return allSeries;
        }

        // NBA-style bracket pairings for round 1 — (lowerSeedNumber, higherSeedNumber) in bracket order
        private static List<(int Top, int Bottom)> GetRound1Pairings(int teamCount) => teamCount switch
        {
            2 => new() { (1, 2) },
            4 => new() { (1, 4), (2, 3) },
            8 => new() { (1, 8), (4, 5), (3, 6), (2, 7) },
            _ => throw new InvalidOperationException($"Unsupported team count: {teamCount}")
        };

        private static int GetRoundCount(int teamCount) => teamCount switch
        {
            2 => 1,
            4 => 2,
            8 => 3,
            _ => throw new InvalidOperationException($"Unsupported team count: {teamCount}")
        };

        private static string GetRoundName(int roundNumber, int totalRounds)
        {
            int fromEnd = totalRounds - roundNumber;
            return fromEnd switch
            {
                0 => "Final",
                1 => "Semi-Final",
                2 => "Quarter-Final",
                _ => $"Round {roundNumber}"
            };
        }

        private static bool IsValidTeamCount(int count) => count is 2 or 4 or 8;
    }
}
