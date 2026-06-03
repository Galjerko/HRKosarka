using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using HRKošarka.Domain.Helpers;
using MediatR;
using DomainMatch = HRKošarka.Domain.Match;

namespace HRKošarka.Application.Features.Match.Commands.ConfirmMatchResult
{
    public class ConfirmMatchResultCommandHandler : IRequestHandler<ConfirmMatchResultCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerMatchStatsRepository _statsRepository;
        private readonly ILeagueStandingRepository _standingRepository;
        private readonly IPlayerSeasonStatsRepository _seasonStatsRepository;
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly ILeagueRepository _leagueRepository;

        public ConfirmMatchResultCommandHandler(
            IMatchRepository matchRepository,
            IPlayerMatchStatsRepository statsRepository,
            ILeagueStandingRepository standingRepository,
            IPlayerSeasonStatsRepository seasonStatsRepository,
            ITeamRepresentativeRepository repRepository,
            ILeagueRepository leagueRepository)
        {
            _matchRepository = matchRepository;
            _statsRepository = statsRepository;
            _standingRepository = standingRepository;
            _seasonStatsRepository = seasonStatsRepository;
            _repRepository = repRepository;
            _leagueRepository = leagueRepository;
        }

        public async Task<CommandResponse<bool>> Handle(ConfirmMatchResultCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetMatchWithFullDetailsAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Match result is already confirmed.");

            if (!request.IsForced && match.ResultSubmissionStatus != ResultSubmissionStatus.HomeSubmitted)
                throw new BadRequestException("Home team has not submitted the result yet.");

            if (!request.IsForced)
            {
                bool authorized = false;
                if (!string.IsNullOrEmpty(request.ConfirmerClubId))
                    authorized = match.AwayTeam.ClubId.ToString() == request.ConfirmerClubId;
                if (!authorized && !string.IsNullOrEmpty(request.ConfirmerUserId))
                    authorized = await _repRepository.IsActiveRepForTeamAsync(request.ConfirmerUserId, match.AwayTeamId, ct);
                if (!authorized)
                    throw new BadRequestException("Only the away team's manager or representative can confirm the result.");
            }

            if (!match.HomeScore.HasValue || !match.AwayScore.HasValue)
                throw new BadRequestException("Score must be entered before confirming.");

            var allStats = await _statsRepository.GetPlayedStatsForMatchAsync(match.Id, ct);
            var homeStats = allStats.Where(s => s.TeamId == match.HomeTeamId).ToList();
            var awayStats = allStats.Where(s => s.TeamId == match.AwayTeamId).ToList();

            if (homeStats.Any() && homeStats.Sum(s => s.Points) != match.HomeScore.Value)
                throw new BadRequestException(
                    $"Home team player totals ({homeStats.Sum(s => s.Points)} pts) do not match the score ({match.HomeScore.Value} pts).");

            if (awayStats.Any() && awayStats.Sum(s => s.Points) != match.AwayScore.Value)
                throw new BadRequestException(
                    $"Away team player totals ({awayStats.Sum(s => s.Points)} pts) do not match the score ({match.AwayScore.Value} pts).");

            if (allStats.Any(s => s.Points < s.ThreePointers * 3))
                throw new BadRequestException("One or more players have fewer total points than their three-pointers alone account for.");

            if (match.HomeScore.Value == match.AwayScore.Value)
                throw new BadRequestException("Final score cannot be tied. Enter overtime scores to resolve.");

            ValidateQuarterResults(match.QuarterResults, match.HomeScore.Value, match.AwayScore.Value);

            match.IsResultConfirmed = true;
            match.ResultSubmissionStatus = ResultSubmissionStatus.Confirmed;
            match.Status = MatchStatus.Completed;
            match.ConfirmedByUserId = request.ConfirmedByUserId;
            match.ConfirmedAt = DateTime.UtcNow;
            await _matchRepository.UpdateAsync(match, ct);

            var seasonId = match.League.SeasonId;
            await UpdateStanding(match.LeagueId, match.HomeTeamId, seasonId,
                match.HomeScore.Value, match.AwayScore.Value, ct);
            await UpdateStanding(match.LeagueId, match.AwayTeamId, seasonId,
                match.AwayScore.Value, match.HomeScore.Value, ct);
            await RecalculatePositions(match.LeagueId, ct);

            foreach (var stat in allStats)
                await UpdatePlayerSeasonStats(stat, match.LeagueId, seasonId, ct);

            if (match.League.CompetitionType == CompetitionType.Cup)
                await AdvanceCupBracketIfRoundComplete(match, ct);

            return CommandResponse<bool>.Success(true, "Match result confirmed.");
        }

        private async Task AdvanceCupBracketIfRoundComplete(DomainMatch confirmedMatch, CancellationToken ct)
        {
            var roundMatches = await _matchRepository.GetRoundMatchesAsync(
                confirmedMatch.LeagueId, confirmedMatch.Round, ct);

            if (roundMatches.Any(m => !m.IsResultConfirmed))
                return;

            var orderedWinners = roundMatches
                .OrderBy(m => m.DateCreated)
                .ThenBy(m => m.HomeTeamId)
                .Select(m => m.HomeScore!.Value > m.AwayScore!.Value ? m.HomeTeamId : m.AwayTeamId)
                .ToList();

            List<Guid> nextRoundTeams;
            if (confirmedMatch.Round == 1)
            {
                // Identify bye teams: registered in league but absent from all round 1 matches
                var teamsInRound1 = roundMatches
                    .SelectMany(m => new[] { m.HomeTeamId, m.AwayTeamId })
                    .ToHashSet();
                var allLeagueTeams = await _leagueRepository.GetLeagueTeamsAsync(confirmedMatch.LeagueId, ct);
                var byeTeams = allLeagueTeams
                    .Where(t => !teamsInRound1.Contains(t.TeamId))
                    .Select(t => t.TeamId)
                    .ToList();

                // Interleave: [bye0, winner0, bye1, winner1, ...] to maintain bracket structure
                nextRoundTeams = new List<Guid>();
                for (int i = 0; i < Math.Max(byeTeams.Count, orderedWinners.Count); i++)
                {
                    if (i < byeTeams.Count) nextRoundTeams.Add(byeTeams[i]);
                    if (i < orderedWinners.Count) nextRoundTeams.Add(orderedWinners[i]);
                }
            }
            else
            {
                nextRoundTeams = orderedWinners;
            }

            if (nextRoundTeams.Count <= 1)
                return; // Final was just played — tournament complete

            var nextRound = confirmedMatch.Round + 1;
            var nextRoundName = CupBracketScheduler.GetCupRoundName(nextRoundTeams.Count);

            var breaks = await _leagueRepository.GetLeagueBreaksAsync(confirmedMatch.LeagueId, ct);
            var breakRanges = breaks.Select(b => (b.StartDate, b.EndDate)).ToList();
            var lastRoundDate = roundMatches.Max(m => m.DefaultScheduledDate);
            var nextDate = CupBracketScheduler.FindNextValidSaturday(lastRoundDate.AddDays(1), breakRanges);

            var newMatches = new List<DomainMatch>();
            for (int i = 0; i < nextRoundTeams.Count; i += 2)
            {
                newMatches.Add(new DomainMatch
                {
                    LeagueId = confirmedMatch.LeagueId,
                    HomeTeamId = nextRoundTeams[i],
                    AwayTeamId = nextRoundTeams[i + 1],
                    Round = nextRound,
                    RoundName = nextRoundName,
                    DefaultScheduledDate = nextDate,
                    ActualScheduledDate = nextDate,
                    Status = MatchStatus.Scheduled,
                    SchedulingStatus = SchedulingStatus.Default,
                    LastSchedulingUpdate = DateTime.Now
                });
            }

            await _matchRepository.CreateRangeAsync(newMatches, ct);
        }

        private static void ValidateQuarterResults(string? raw, int homeScore, int awayScore)
        {
            if (string.IsNullOrWhiteSpace(raw))
                throw new BadRequestException("Quarter scores are required before confirming the result.");

            var segments = raw.Split(';');
            if (segments.Length < 4)
                throw new BadRequestException("Quarter scores must contain at least 4 periods (Q1–Q4).");

            int homeTotal = 0, awayTotal = 0;
            int homeRegulation = 0, awayRegulation = 0;

            for (int i = 0; i < segments.Length; i++)
            {
                var parts = segments[i].Split(':');
                if (parts.Length != 2
                    || !int.TryParse(parts[0], out int h) || h < 0
                    || !int.TryParse(parts[1], out int a) || a < 0)
                    throw new BadRequestException($"Quarter score segment '{segments[i]}' is invalid. Expected format: home:away.");

                homeTotal += h;
                awayTotal += a;
                if (i < 4) { homeRegulation += h; awayRegulation += a; }
            }

            if (homeTotal != homeScore)
                throw new BadRequestException($"Quarter scores (home total: {homeTotal}) do not match the final score ({homeScore}).");
            if (awayTotal != awayScore)
                throw new BadRequestException($"Quarter scores (away total: {awayTotal}) do not match the final score ({awayScore}).");

            if (homeRegulation == awayRegulation && segments.Length < 5)
                throw new BadRequestException("Regulation ended tied. At least one overtime period must be entered.");
        }

        private async Task UpdateStanding(Guid leagueId, Guid teamId, Guid seasonId,
            int teamScore, int opponentScore, CancellationToken ct)
        {
            var standing = await _standingRepository.GetByTeamAndLeagueAsync(teamId, leagueId, seasonId, ct);
            bool isNew = standing == null;
            standing ??= new LeagueStanding { LeagueId = leagueId, TeamId = teamId, SeasonId = seasonId };

            standing.MatchesPlayed++;
            standing.PointsFor += teamScore;
            standing.PointsAgainst += opponentScore;
            standing.PointsDifference = standing.PointsFor - standing.PointsAgainst;

            if (teamScore > opponentScore) { standing.Wins++; standing.LeaguePoints += 2; }
            else { standing.Losses++; standing.LeaguePoints += 1; }

            standing.LastUpdated = DateTime.UtcNow;

            if (isNew) await _standingRepository.CreateAsync(standing, ct);
            else await _standingRepository.UpdateAsync(standing, ct);
        }

        private async Task RecalculatePositions(Guid leagueId, CancellationToken ct)
        {
            var standings = await _standingRepository.GetByLeagueAsync(leagueId, ct);
            for (int i = 0; i < standings.Count; i++)
            {
                standings[i].Position = i + 1;
                await _standingRepository.UpdateAsync(standings[i], ct);
            }
        }

        private async Task UpdatePlayerSeasonStats(PlayerMatchStats stat, Guid leagueId, Guid seasonId, CancellationToken ct)
        {
            var existing = await _seasonStatsRepository.GetByPlayerAndLeagueAsync(stat.PlayerId, leagueId, seasonId, ct);
            bool isNew = existing == null;
            existing ??= new PlayerSeasonStats
            {
                PlayerId = stat.PlayerId,
                LeagueId = leagueId,
                SeasonId = seasonId,
                TeamId = stat.TeamId!.Value
            };

            existing.MatchesPlayed++;
            existing.TotalPoints += stat.Points;
            existing.TotalFouls += stat.Fouls;
            existing.TotalThreePointers += stat.ThreePointers;
            existing.AveragePoints = Math.Round(existing.TotalPoints / (decimal)existing.MatchesPlayed, 2);
            existing.AverageFouls = Math.Round(existing.TotalFouls / (decimal)existing.MatchesPlayed, 2);
            existing.AverageThreePointers = Math.Round(existing.TotalThreePointers / (decimal)existing.MatchesPlayed, 2);
            existing.LastUpdated = DateTime.UtcNow;

            if (isNew) await _seasonStatsRepository.CreateAsync(existing, ct);
            else await _seasonStatsRepository.UpdateAsync(existing, ct);
        }
    }
}
