using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamLeaguePlayerStats
{
    public class GetTeamLeaguePlayerStatsQueryHandler
        : IRequestHandler<GetTeamLeaguePlayerStatsQuery, QueryResponse<List<TeamPlayerStatDTO>>>
    {
        private readonly IPlayerSeasonStatsRepository _playerSeasonStatsRepository;

        public GetTeamLeaguePlayerStatsQueryHandler(IPlayerSeasonStatsRepository playerSeasonStatsRepository)
        {
            _playerSeasonStatsRepository = playerSeasonStatsRepository;
        }

        public async Task<QueryResponse<List<TeamPlayerStatDTO>>> Handle(
            GetTeamLeaguePlayerStatsQuery request, CancellationToken ct)
        {
            var stats = await _playerSeasonStatsRepository.GetByTeamAndLeagueAsync(request.TeamId, request.LeagueId, ct);
            return QueryResponse<List<TeamPlayerStatDTO>>.Success(stats);
        }
    }
}
