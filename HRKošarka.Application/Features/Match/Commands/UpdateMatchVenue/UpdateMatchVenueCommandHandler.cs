using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Application.Services;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.UpdateMatchVenue
{
    public class UpdateMatchVenueCommandHandler : IRequestHandler<UpdateMatchVenueCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public UpdateMatchVenueCommandHandler(
            IMatchRepository matchRepository,
            ITeamRepresentativeRepository repRepository,
            EmailNotificationService emailNotificationService)
        {
            _matchRepository = matchRepository;
            _repRepository = repRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<CommandResponse<bool>> Handle(UpdateMatchVenueCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Cannot change venue of a confirmed match.");

            bool isAdmin = string.IsNullOrEmpty(request.RequesterClubId) && string.IsNullOrEmpty(request.RequesterUserId);
            if (!isAdmin)
            {
                bool authorized = false;
                if (!string.IsNullOrEmpty(request.RequesterClubId))
                    authorized = match.HomeTeam.ClubId.ToString() == request.RequesterClubId;
                if (!authorized && !string.IsNullOrEmpty(request.RequesterUserId))
                    authorized = await _repRepository.IsActiveRepForTeamAsync(request.RequesterUserId, match.HomeTeamId, ct);
                if (!authorized)
                    throw new BadRequestException("Only the home team's manager or representative can set the venue.");
            }

            match.VenueOverride = string.IsNullOrWhiteSpace(request.Venue) ? null : request.Venue.Trim();
            await _matchRepository.UpdateAsync(match, ct);

            var recipients = await _emailNotificationService.GetTeamRecipientsAsync(match.AwayTeamId, match.AwayTeam.ClubId, ct);
            await _emailNotificationService.SendNotificationAsync(
                recipients,
                NotificationType.VenueChanged,
                $"Venue updated: {match.HomeTeam.Name} vs {match.AwayTeam.Name}",
                $"The venue for your match against {match.HomeTeam.Name} on {match.ActualScheduledDate:d} has been set to: {match.VenueOverride ?? "the default venue"}.",
                match.Id,
                linkPath: $"/matches/{match.Id}",
                linkText: "View match",
                ct: ct);

            return CommandResponse<bool>.Success(true, "Venue updated.");
        }
    }
}
