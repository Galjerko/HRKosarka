using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Queries.GetPendingActions
{
    public class GetPendingActionsQueryHandler
        : IRequestHandler<GetPendingActionsQuery, QueryResponse<List<PendingActionDTO>>>
    {
        private readonly IMatchRepository _matchRepository;

        public GetPendingActionsQueryHandler(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<QueryResponse<List<PendingActionDTO>>> Handle(
            GetPendingActionsQuery request, CancellationToken ct)
        {
            var items = await _matchRepository.GetPendingActionsAsync(request.ClubId, request.IsAdmin, ct);
            return QueryResponse<List<PendingActionDTO>>.Success(items);
        }
    }
}
