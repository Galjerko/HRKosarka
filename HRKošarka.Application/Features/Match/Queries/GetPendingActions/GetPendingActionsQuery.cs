using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Queries.GetPendingActions
{
    public class GetPendingActionsQuery : IRequest<QueryResponse<List<PendingActionDTO>>>
    {
        public Guid? ClubId { get; set; }   // null for admin
        public bool IsAdmin { get; set; }
    }
}
