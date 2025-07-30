using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.DeactivateTeam
{
    public record DeactivateTeamCommand(Guid Id) : IRequest<Unit>;
}