using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.RevokeTeamRepresentative
{
    public class RevokeTeamRepresentativeCommand : IRequest<CommandResponse<bool>>
    {
        public Guid TeamId { get; set; }
        public Guid RepresentativeId { get; set; }
    }
}
