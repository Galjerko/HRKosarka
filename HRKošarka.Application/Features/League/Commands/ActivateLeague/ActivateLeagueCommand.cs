using MediatR;

namespace HRKošarka.Application.Features.League.Commands.ActivateLeague
{
    public record ActivateLeagueCommand(Guid Id) : IRequest<Unit>;
}
