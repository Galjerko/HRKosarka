using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Services;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.User.Commands.AssignClubManager
{
    public class AssignClubManagerCommandHandler : IRequestHandler<AssignClubManagerCommand, Unit>
    {
        private readonly IClubManagerService _clubManagerService;
        private readonly IClubRepository _clubRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public AssignClubManagerCommandHandler(
            IClubManagerService clubManagerService,
            IClubRepository clubRepository,
            EmailNotificationService emailNotificationService)
        {
            _clubManagerService = clubManagerService;
            _clubRepository = clubRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<Unit> Handle(AssignClubManagerCommand request, CancellationToken cancellationToken)
        {
            var result = await _clubManagerService.AssignClubManager(request.UserId, request.ClubId);

            if (!result.IsSuccess)
            {
                throw new BadRequestException(result.Message ?? "Failed to assign club manager");
            }

            var club = await _clubRepository.GetByIdAsync(request.ClubId, cancellationToken);
            await _emailNotificationService.SendNotificationAsync(
                new[] { request.UserId },
                NotificationType.ClubManagerAssigned,
                "You have been assigned as club manager",
                $"You have been assigned as manager of {club?.Name ?? "your club"}.",
                matchId: null,
                linkPath: $"/clubs/{request.ClubId}",
                linkText: "View club",
                ct: cancellationToken);

            return Unit.Value;
        }
    }
}
