using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.ConfirmMatchResult
{
    public class ConfirmMatchResultCommandHandler : IRequestHandler<ConfirmMatchResultCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerMatchStatsRepository _statsRepository;
        private readonly ILeagueStandingRepository _standingRepository;
        private readonly IPlayerSeasonStatsRepository _seasonStatsRepository;
        private readonly ITeamRepresentativeRepository _repRepository;

        public ConfirmMatchResultCommandHandler(
            IMatchRepository matchRepository,
            IPlayerMatchStatsRepository statsRepository,
            ILeagueStandingRepository standingRepository,
            IPlayerSeasonStatsRepository seasonStatsRepository,
            ITeamRepresentativeRepository repRepository)
        {
            _matchRepository = matchRepository;
            _statsRepository = statsRepository;
            _standingRepository = standingRepository;
            _seasonStatsRepository = seasonStatsRepository;
            _repRepository = repRepository;
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

            return CommandResponse<bool>.Success(true, "Match result confirmed.");
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
