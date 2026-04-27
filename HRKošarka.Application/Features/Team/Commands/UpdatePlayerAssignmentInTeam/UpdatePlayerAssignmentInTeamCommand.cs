using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.UpdatePlayerAssignmentInTeam
{
    public class UpdatePlayerAssignmentInTeamCommand : IRequest<CommandResponse<bool>>
    {
        public Guid TeamId { get; set; }
        public Guid PlayerId { get; set; }
        public int? JerseyNumber { get; set; }
    }
}
