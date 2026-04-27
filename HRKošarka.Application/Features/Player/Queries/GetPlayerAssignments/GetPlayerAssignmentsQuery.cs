using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerAssignments
{
    public record GetPlayerAssignmentsQuery(Guid PlayerId) : IRequest<QueryResponse<List<PlayerAssignmentDTO>>>;
}
