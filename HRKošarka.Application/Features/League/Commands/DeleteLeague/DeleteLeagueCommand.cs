using MediatR;

namespace HRKošarka.Application.Features.League.Commands.DeleteLeague
{
    public record DeleteLeagueCommand(Guid Id) : IRequest<Unit>;
}
