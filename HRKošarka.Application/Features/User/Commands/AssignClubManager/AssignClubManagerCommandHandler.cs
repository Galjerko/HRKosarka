using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.User.Commands.AssignClubManager
{
    public class AssignClubManagerCommandHandler : IRequestHandler<AssignClubManagerCommand, Unit>
    {
        private readonly IClubManagerService _clubManagerService;

        public AssignClubManagerCommandHandler(IClubManagerService clubManagerService)
        {
            _clubManagerService = clubManagerService;
        }

        public async Task<Unit> Handle(AssignClubManagerCommand request, CancellationToken cancellationToken)
        {
            var result = await _clubManagerService.AssignClubManager(request.UserId, request.ClubId);

            if (!result.IsSuccess)
            {
                throw new BadRequestException(result.Message ?? "Failed to assign club manager");
            }

            return Unit.Value;
        }
    }
}
