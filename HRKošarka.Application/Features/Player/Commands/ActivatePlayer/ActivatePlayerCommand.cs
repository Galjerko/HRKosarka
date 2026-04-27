using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.ActivatePlayer
{
    public record ActivatePlayerCommand(Guid Id) : IRequest<Unit>;
}
