using MediatR;

namespace HRKošarka.Application.Features.League.Commands.DeactivateLeague
{
    public record DeactivateLeagueCommand(Guid Id) : IRequest<Unit>;
}
