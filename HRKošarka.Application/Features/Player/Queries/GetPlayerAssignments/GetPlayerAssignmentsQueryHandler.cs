using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerAssignments
{
    public class GetPlayerAssignmentsQueryHandler
        : IRequestHandler<GetPlayerAssignmentsQuery, QueryResponse<List<PlayerAssignmentDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IPlayerTeamHistoryRepository _historyRepository;

        public GetPlayerAssignmentsQueryHandler(IMapper mapper, IPlayerTeamHistoryRepository historyRepository)
        {
            _mapper = mapper;
            _historyRepository = historyRepository;
        }

        public async Task<QueryResponse<List<PlayerAssignmentDTO>>> Handle(
            GetPlayerAssignmentsQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _historyRepository.GetActiveByPlayerAsync(request.PlayerId, cancellationToken);
            var data = _mapper.Map<List<PlayerAssignmentDTO>>(assignments);
            return QueryResponse<List<PlayerAssignmentDTO>>.Success(data);
        }
    }
}
