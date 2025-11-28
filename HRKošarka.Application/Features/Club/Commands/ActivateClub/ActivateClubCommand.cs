using MediatR;

namespace HRKošarka.Application.Features.Club.Commands.ActivateClub
{
    public record ActivateClubCommand(Guid Id) : IRequest<Unit>;
}
