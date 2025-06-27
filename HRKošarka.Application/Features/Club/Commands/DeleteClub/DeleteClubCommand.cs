using MediatR;

namespace HRKošarka.Application.Features.Club.Commands.DeleteClub
{
    public record DeleteClubCommand(Guid Id) : IRequest<Unit>;
}
