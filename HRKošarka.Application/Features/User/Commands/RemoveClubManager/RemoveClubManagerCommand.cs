using MediatR;

namespace HRKošarka.Application.Features.User.Commands.RemoveClubManager
{
    public class RemoveClubManagerCommand : IRequest<Unit>
    {
        public string UserId { get; set; } = string.Empty;
    }
}
