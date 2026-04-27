using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.DeletePlayer
{
    public record DeletePlayerCommand(Guid Id) : IRequest<Unit>;
}
