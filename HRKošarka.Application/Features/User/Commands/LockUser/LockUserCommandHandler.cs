using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.User.Commands.LockUser
{
    public class LockUserCommandHandler : IRequestHandler<LockUserCommand, Unit>
    {
        private readonly IUserService _userService;

        public LockUserCommandHandler(IUserService userService)
        {
            _userService = userService;
        }

        public async Task<Unit> Handle(LockUserCommand request, CancellationToken cancellationToken)
        {
            var result = await _userService.LockUser(request.UserId);

            if (!result.IsSuccess)
            {
                throw new BadRequestException(result.Message ?? "Failed to lock user");
            }

            return Unit.Value;
        }
    }
}
