using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamLeagues
{
    public class GetTeamLeaguesQueryHandler : IRequestHandler<GetTeamLeaguesQuery, QueryResponse<List<TeamLeagueDTO>>>
    {
        private readonly ILeagueRepository _leagueRepository;

        public GetTeamLeaguesQueryHandler(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public async Task<QueryResponse<List<TeamLeagueDTO>>> Handle(GetTeamLeaguesQuery request, CancellationToken cancellationToken)
        {
            var leagues = await _leagueRepository.GetTeamLeaguesAsync(request.TeamId, cancellationToken);
            return QueryResponse<List<TeamLeagueDTO>>.Success(leagues, "Team leagues retrieved successfully.");
        }
    }
}
