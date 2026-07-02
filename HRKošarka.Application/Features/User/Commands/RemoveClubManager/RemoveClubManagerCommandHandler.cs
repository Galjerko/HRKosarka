using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Services;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.User.Commands.RemoveClubManager
{
    public class RemoveClubManagerCommandHandler : IRequestHandler<RemoveClubManagerCommand, Unit>
    {
        private readonly IClubManagerService _clubManagerService;
        private readonly IClubRepository _clubRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public RemoveClubManagerCommandHandler(
            IClubManagerService clubManagerService,
            IClubRepository clubRepository,
            EmailNotificationService emailNotificationService)
        {
            _clubManagerService = clubManagerService;
            _clubRepository = clubRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<Unit> Handle(RemoveClubManagerCommand request, CancellationToken cancellationToken)
        {
            // Capture the managed club before removal clears it from the user record.
            var managedClubId = await _clubManagerService.GetManagedClubId(request.UserId);

            var result = await _clubManagerService.RemoveClubManager(request.UserId);

            if (!result.IsSuccess)
            {
                throw new BadRequestException(result.Message ?? "Failed to remove club manager");
            }

            var club = managedClubId.HasValue
                ? await _clubRepository.GetByIdAsync(managedClubId.Value, cancellationToken)
                : null;

            await _emailNotificationService.SendNotificationAsync(
                new[] { request.UserId },
                NotificationType.ClubManagerRemoved,
                "Your club manager role has been removed",
                $"Your club manager role for {club?.Name ?? "your club"} has been removed.",
                matchId: null,
                linkPath: managedClubId.HasValue ? $"/clubs/{managedClubId}" : null,
                linkText: "View club",
                ct: cancellationToken);

            return Unit.Value;
        }
    }
}
