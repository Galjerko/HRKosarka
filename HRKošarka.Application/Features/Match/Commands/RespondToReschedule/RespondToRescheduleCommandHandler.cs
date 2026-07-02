using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Application.Services;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.RespondToReschedule
{
    public class RespondToRescheduleCommandHandler : IRequestHandler<RespondToRescheduleCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchReschedulingRequestRepository _reschedulingRepository;
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public RespondToRescheduleCommandHandler(
            IMatchRepository matchRepository,
            IMatchReschedulingRequestRepository reschedulingRepository,
            ITeamRepresentativeRepository repRepository,
            EmailNotificationService emailNotificationService)
        {
            _matchRepository = matchRepository;
            _reschedulingRepository = reschedulingRepository;
            _repRepository = repRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<CommandResponse<bool>> Handle(RespondToRescheduleCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            var proposal = await _reschedulingRepository.GetActiveForMatchAsync(request.MatchId, ct)
                ?? throw new BadRequestException("No active reschedule proposal found for this match.");

            Guid responderTeamId;
            if (request.ResponderClubId.HasValue && request.ResponderClubId != Guid.Empty)
            {
                bool isHomeClub = match.HomeTeam.ClubId == request.ResponderClubId;
                bool isAwayClub = match.AwayTeam.ClubId == request.ResponderClubId;
                if (!isHomeClub && !isAwayClub)
                    throw new BadRequestException("Only a manager or representative of one of the match teams can respond.");
                responderTeamId = isHomeClub ? match.HomeTeamId : match.AwayTeamId;
            }
            else if (!string.IsNullOrEmpty(request.ResponderUserId))
            {
                bool isHomeRep = await _repRepository.IsActiveRepForTeamAsync(request.ResponderUserId, match.HomeTeamId, ct);
                bool isAwayRep = !isHomeRep && await _repRepository.IsActiveRepForTeamAsync(request.ResponderUserId, match.AwayTeamId, ct);
                if (!isHomeRep && !isAwayRep)
                    throw new BadRequestException("Only a manager or representative of one of the match teams can respond.");
                responderTeamId = isHomeRep ? match.HomeTeamId : match.AwayTeamId;
            }
            else
            {
                throw new BadRequestException("Only a manager or representative of one of the match teams can respond.");
            }

            // Prevent responding to own proposal
            bool ownProposalByTeam = proposal.RequestedByTeamId == responderTeamId;
            bool ownProposalByClub = !proposal.RequestedByTeamId.HasValue &&
                request.ResponderClubId.HasValue &&
                proposal.RequestedByClubId == request.ResponderClubId;
            if (ownProposalByTeam || ownProposalByClub)
                throw new BadRequestException("You cannot respond to your own reschedule proposal.");

            proposal.ResponseByUserId = request.ResponderUserId;
            proposal.RespondedAt = DateTime.UtcNow;

            var proposingTeamId = proposal.RequestedByTeamId
                ?? (match.HomeTeam.ClubId == proposal.RequestedByClubId ? match.HomeTeamId : match.AwayTeamId);
            var proposingClubId = proposingTeamId == match.HomeTeamId ? match.HomeTeam.ClubId : match.AwayTeam.ClubId;

            if (request.Accept)
            {
                proposal.Status = RequestStatus.Accepted;
                match.ActualScheduledDate = proposal.ProposedDate;
                match.Status = MatchStatus.Rescheduled;
                match.SchedulingStatus = SchedulingStatus.Agreed;
                match.LastSchedulingUpdate = DateTime.UtcNow;
                await _matchRepository.UpdateAsync(match, ct);
                await _reschedulingRepository.UpdateAsync(proposal, ct);

                var recipients = await _emailNotificationService.GetTeamRecipientsAsync(proposingTeamId, proposingClubId, ct);
                recipients.UnionWith(await _emailNotificationService.GetTeamFanRecipientsAsync(match.HomeTeamId, ct));
                recipients.UnionWith(await _emailNotificationService.GetTeamFanRecipientsAsync(match.AwayTeamId, ct));
                await _emailNotificationService.SendNotificationAsync(
                    recipients,
                    NotificationType.RescheduleAccepted,
                    $"Reschedule accepted: {match.HomeTeam.Name} vs {match.AwayTeam.Name}",
                    $"The reschedule proposal for the match between {match.HomeTeam.Name} and {match.AwayTeam.Name} was accepted. New date: {match.ActualScheduledDate:d}.",
                    match.Id,
                    linkPath: $"/matches/{match.Id}",
                    linkText: "View match",
                    ct: ct);

                return CommandResponse<bool>.Success(true, "Reschedule accepted. The match has been moved to the new date.");
            }
            else
            {
                proposal.Status = RequestStatus.Rejected;
                match.SchedulingStatus = SchedulingStatus.Default;
                match.LastSchedulingUpdate = DateTime.UtcNow;
                await _matchRepository.UpdateAsync(match, ct);
                await _reschedulingRepository.UpdateAsync(proposal, ct);

                var recipients = await _emailNotificationService.GetTeamRecipientsAsync(proposingTeamId, proposingClubId, ct);
                await _emailNotificationService.SendNotificationAsync(
                    recipients,
                    NotificationType.RescheduleRejected,
                    $"Reschedule rejected: {match.HomeTeam.Name} vs {match.AwayTeam.Name}",
                    $"The reschedule proposal for the match between {match.HomeTeam.Name} and {match.AwayTeam.Name} was rejected. The match remains on {match.ActualScheduledDate:d}.",
                    match.Id,
                    linkPath: $"/matches/{match.Id}",
                    linkText: "View match",
                    ct: ct);

                return CommandResponse<bool>.Success(true, "Reschedule rejected. The match remains on the original date.");
            }
        }
    }
}
