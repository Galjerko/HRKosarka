using MediatR;

namespace HRKošarka.Application.Features.User.Commands.LockUser
{
    public class LockUserCommand : IRequest<Unit>
    {
        public string UserId { get; set; } = string.Empty;
    }

}
