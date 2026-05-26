using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamLeagueStanding
{
    public class GetTeamLeagueStandingQueryHandler : IRequestHandler<GetTeamLeagueStandingQuery, QueryResponse<TeamLeagueStandingDTO?>>
    {
        private readonly ILeagueStandingRepository _standingRepository;

        public GetTeamLeagueStandingQueryHandler(ILeagueStandingRepository standingRepository)
        {
            _standingRepository = standingRepository;
        }

        public async Task<QueryResponse<TeamLeagueStandingDTO?>> Handle(GetTeamLeagueStandingQuery request, CancellationToken ct)
        {
            var standing = await _standingRepository.GetByTeamAndLeagueAsync(request.TeamId, request.LeagueId, ct);

            if (standing == null)
                return QueryResponse<TeamLeagueStandingDTO?>.Success(null);

            var allStandings = await _standingRepository.GetByLeagueAsync(request.LeagueId, ct);
            var position = allStandings
                .OrderByDescending(s => s.LeaguePoints)
                .ThenByDescending(s => s.PointsDifference)
                .ThenByDescending(s => s.PointsFor)
                .ToList()
                .FindIndex(s => s.TeamId == request.TeamId) + 1;

            return QueryResponse<TeamLeagueStandingDTO?>.Success(new TeamLeagueStandingDTO
            {
                Position = position > 0 ? position : 1,
                GamesPlayed = standing.MatchesPlayed,
                Wins = standing.Wins,
                Losses = standing.Losses,
                PointsFor = standing.PointsFor,
                PointsAgainst = standing.PointsAgainst,
                PointsDifference = standing.PointsDifference,
                LeaguePoints = standing.LeaguePoints
            });
        }
    }
}
