using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.RecordForfeit
{
    public class RecordForfeitCommandHandler : IRequestHandler<RecordForfeitCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ILeagueStandingRepository _standingRepository;

        public RecordForfeitCommandHandler(
            IMatchRepository matchRepository,
            ILeagueStandingRepository standingRepository)
        {
            _matchRepository = matchRepository;
            _standingRepository = standingRepository;
        }

        public async Task<CommandResponse<bool>> Handle(RecordForfeitCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetMatchWithFullDetailsAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Match result is already confirmed.");

            if (match.HomeTeamId != request.ForfeitingTeamId && match.AwayTeamId != request.ForfeitingTeamId)
                throw new BadRequestException("The specified team is not playing in this match.");

            bool homeTeamForfeit = match.HomeTeamId == request.ForfeitingTeamId;
            match.HomeScore = homeTeamForfeit ? 0 : 20;
            match.AwayScore = homeTeamForfeit ? 20 : 0;
            match.Status = MatchStatus.Forfeit;
            match.IsResultConfirmed = true;
            match.ResultSubmissionStatus = ResultSubmissionStatus.Confirmed;
            match.ConfirmedByUserId = request.ConfirmedByUserId;
            match.ConfirmedAt = DateTime.UtcNow;
            await _matchRepository.UpdateAsync(match, ct);

            var seasonId = match.League.SeasonId;
            await UpdateStanding(match.LeagueId, match.HomeTeamId, seasonId,
                match.HomeScore.Value, match.AwayScore.Value, ct);
            await UpdateStanding(match.LeagueId, match.AwayTeamId, seasonId,
                match.AwayScore.Value, match.HomeScore.Value, ct);
            await RecalculatePositions(match.LeagueId, ct);

            return CommandResponse<bool>.Success(true, "Forfeit recorded.");
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
    }
}
