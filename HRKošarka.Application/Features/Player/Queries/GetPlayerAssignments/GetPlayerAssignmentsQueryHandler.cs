using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerAssignments
{
    public class GetPlayerAssignmentsQueryHandler
        : IRequestHandler<GetPlayerAssignmentsQuery, QueryResponse<List<PlayerAssignmentDTO>>>
    {
        private readonly IPlayerTeamHistoryRepository _historyRepository;

        public GetPlayerAssignmentsQueryHandler(IPlayerTeamHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task<QueryResponse<List<PlayerAssignmentDTO>>> Handle(
            GetPlayerAssignmentsQuery request, CancellationToken cancellationToken)
        {
            var assignments = await _historyRepository.GetAllByPlayerAsync(request.PlayerId, cancellationToken);
            var data = assignments.Select(a => new PlayerAssignmentDTO
            {
                Id = a.Id,
                TeamId = a.TeamId,
                TeamName = a.Team.Name,
                ClubId = a.Team.ClubId,
                ClubName = a.Team.Club.Name,
                AgeCategoryName = a.Team.AgeCategory.Name,
                Gender = a.Team.Gender,
                SeasonId = a.SeasonId,
                SeasonName = a.Season.Name,
                JoinDate = a.JoinDate,
                LeaveDate = a.LeaveDate,
                JerseyNumber = a.JerseyNumber,
                IsActive = a.IsActive
            }).ToList();
            return QueryResponse<List<PlayerAssignmentDTO>>.Success(data);
        }
    }
}
