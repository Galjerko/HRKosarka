using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.User.Commands.UnlockUser
{
    public class UnlockUserCommandHandler : IRequestHandler<UnlockUserCommand, Unit>
    {
        private readonly IUserService _userService;

        public UnlockUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Unit> Handle(UnlockUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _userService.UnlockUser(request.UserId);

            if (!result.IsSuccess)
            {
                throw new BadRequestException(result.Message ?? "Failed to unlock user");
            }

            return Unit.Value;
        }
    }
}
