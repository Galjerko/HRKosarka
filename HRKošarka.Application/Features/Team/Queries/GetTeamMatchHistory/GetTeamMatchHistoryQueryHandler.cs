using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamMatchHistory
{
    public class GetTeamMatchHistoryQueryHandler
        : IRequestHandler<GetTeamMatchHistoryQuery, QueryResponse<List<TeamMatchHistoryItemDTO>>>
    {
        private readonly IMatchRepository _matchRepository;

        public GetTeamMatchHistoryQueryHandler(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<QueryResponse<List<TeamMatchHistoryItemDTO>>> Handle(
            GetTeamMatchHistoryQuery request, CancellationToken cancellationToken)
        {
            var items = await _matchRepository.GetTeamMatchHistoryAsync(request.TeamId, cancellationToken);
            return QueryResponse<List<TeamMatchHistoryItemDTO>>.Success(items);
        }
    }
}
