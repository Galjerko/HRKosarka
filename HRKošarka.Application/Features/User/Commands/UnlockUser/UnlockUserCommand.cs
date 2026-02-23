using MediatR;

namespace HRKošarka.Application.Features.User.Commands.UnlockUser
{
    public class UnlockUserCommand : IRequest<Unit>
    {
        public string UserId { get; set; } = string.Empty;
    }

}
