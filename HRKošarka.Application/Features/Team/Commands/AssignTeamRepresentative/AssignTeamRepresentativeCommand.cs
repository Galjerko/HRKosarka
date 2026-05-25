using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.AssignTeamRepresentative
{
    public class AssignTeamRepresentativeCommand : IRequest<CommandResponse<Guid>>
    {
        public Guid TeamId { get; set; }
        public string UserId { get; set; } = string.Empty;
    }
}
