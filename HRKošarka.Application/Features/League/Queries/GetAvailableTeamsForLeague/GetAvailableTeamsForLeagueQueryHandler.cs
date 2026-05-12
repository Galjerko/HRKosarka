using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetAvailableTeamsForLeague
{
    public class GetAvailableTeamsForLeagueQueryHandler : IRequestHandler<GetAvailableTeamsForLeagueQuery, QueryResponse<List<AvailableTeamForLeagueDTO>>>
    {
        private readonly ILeagueRepository _leagueRepository;

        public GetAvailableTeamsForLeagueQueryHandler(ILeagueRepository leagueRepository)
        {
            _leagueRepository = leagueRepository;
        }

        public async Task<QueryResponse<List<AvailableTeamForLeagueDTO>>> Handle(GetAvailableTeamsForLeagueQuery request, CancellationToken cancellationToken)
        {
            var teams = await _leagueRepository.GetAvailableTeamsForLeagueAsync(request.LeagueId, request.SearchTerm, cancellationToken);
            return QueryResponse<List<AvailableTeamForLeagueDTO>>.Success(teams, "Available teams retrieved successfully.");
        }
    }
}
