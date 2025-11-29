using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.User.Commands.RemoveClubManager
{
    public class RemoveClubManagerCommandHandler : IRequestHandler<RemoveClubManagerCommand, Unit>
    {
        private readonly IClubManagerService _clubManagerService;

        public RemoveClubManagerCommandHandler(IClubManagerService clubManagerService)
        {
            _clubManagerService = clubManagerService;
        }

        public async Task<Unit> Handle(RemoveClubManagerCommand request, CancellationToken cancellationToken)
        {
            var result = await _clubManagerService.RemoveClubManager(request.UserId);

            if (!result.IsSuccess)
            {
                throw new BadRequestException(result.Message ?? "Failed to remove club manager");
            }

            return Unit.Value;
        }
    }
}
