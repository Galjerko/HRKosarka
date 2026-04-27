using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetAvailableTeamsForPlayer
{
    public class GetAvailableTeamsForPlayerQueryHandler
        : IRequestHandler<GetAvailableTeamsForPlayerQuery, QueryResponse<List<AvailableTeamDTO>>>
    {
        private readonly ITeamRepository _teamRepository;

        public GetAvailableTeamsForPlayerQueryHandler(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;
        }

        public async Task<QueryResponse<List<AvailableTeamDTO>>> Handle(
            GetAvailableTeamsForPlayerQuery request, CancellationToken cancellationToken)
        {
            var teams = await _teamRepository.GetAvailableTeamsForPlayerAsync(
                request.PlayerId, request.SearchTerm, cancellationToken);
            return QueryResponse<List<AvailableTeamDTO>>.Success(teams);
        }
    }
}
