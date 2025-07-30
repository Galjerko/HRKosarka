using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.DeleteTeam
{
    public record DeleteTeamCommand(Guid Id) : IRequest<Unit>;
}