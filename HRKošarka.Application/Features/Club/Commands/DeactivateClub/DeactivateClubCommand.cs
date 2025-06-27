using MediatR;

namespace HRKošarka.Application.Features.Club.Commands.DeactivateClub
{
    public record DeactivateClubCommand(Guid Id) : IRequest<Unit>;
}
