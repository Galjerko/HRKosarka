using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamMatchHistory
{
    public class GetTeamMatchHistoryQuery : IRequest<QueryResponse<List<TeamMatchHistoryItemDTO>>>
    {
        public Guid TeamId { get; set; }
        public GetTeamMatchHistoryQuery(Guid teamId) => TeamId = teamId;
    }
}
