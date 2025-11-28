
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.ActivateTeam
{
    public record ActivateTeamCommand(Guid Id) : IRequest<Unit>;

}