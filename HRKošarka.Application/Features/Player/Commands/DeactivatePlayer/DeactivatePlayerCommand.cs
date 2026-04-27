using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.DeactivatePlayer
{
    public record DeactivatePlayerCommand(Guid Id) : IRequest<Unit>;
}
