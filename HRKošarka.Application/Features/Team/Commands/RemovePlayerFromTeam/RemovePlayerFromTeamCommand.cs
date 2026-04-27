using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.RemovePlayerFromTeam
{
    public record RemovePlayerFromTeamCommand(Guid TeamId, Guid PlayerId) : IRequest<Unit>;
}
