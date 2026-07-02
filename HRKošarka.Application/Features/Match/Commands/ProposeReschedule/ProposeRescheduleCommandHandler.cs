using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Application.Services;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.ProposeReschedule
{
    public class ProposeRescheduleCommandHandler : IRequestHandler<ProposeRescheduleCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchReschedulingRequestRepository _reschedulingRepository;
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public ProposeRescheduleCommandHandler(
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

        public async Task<CommandResponse<bool>> Handle(ProposeRescheduleCommand request, CancellationToken ct)
        {
            var validationResult = await new ProposeRescheduleCommandValidator().ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid reschedule proposal.", validationResult);

            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Cannot reschedule a match that is already confirmed.");

            if (match.Status == MatchStatus.Forfeit)
                throw new BadRequestException("Cannot reschedule a forfeited match.");

            if (request.ProposedDate.Date < match.League.StartDate.Date ||
                request.ProposedDate.Date > match.League.EndDate.Date)
                throw new BadRequestException(
                    $"The proposed date must be within the league period ({match.League.StartDate:dd.MM.yyyy} – {match.League.EndDate:dd.MM.yyyy}).");

            Guid proposerTeamId;
            if (request.ProposerClubId.HasValue && request.ProposerClubId != Guid.Empty)
            {
                bool isHomeClub = match.HomeTeam.ClubId == request.ProposerClubId;
                bool isAwayClub = match.AwayTeam.ClubId == request.ProposerClubId;
                if (!isHomeClub && !isAwayClub)
                    throw new BadRequestException("Only a manager or representative of one of the match teams can propose a reschedule.");
                proposerTeamId = isHomeClub ? match.HomeTeamId : match.AwayTeamId;
            }
            else if (!string.IsNullOrEmpty(request.ProposerUserId))
            {
                bool isHomeRep = await _repRepository.IsActiveRepForTeamAsync(request.ProposerUserId, match.HomeTeamId, ct);
                bool isAwayRep = !isHomeRep && await _repRepository.IsActiveRepForTeamAsync(request.ProposerUserId, match.AwayTeamId, ct);
                if (!isHomeRep && !isAwayRep)
                    throw new BadRequestException("Only a manager or representative of one of the match teams can propose a reschedule.");
                proposerTeamId = isHomeRep ? match.HomeTeamId : match.AwayTeamId;
            }
            else
            {
                throw new BadRequestException("Only a manager or representative of one of the match teams can propose a reschedule.");
            }

            await _reschedulingRepository.ExpireStaleForMatchAsync(request.MatchId, ct);

            var existing = await _reschedulingRepository.GetActiveForMatchAsync(request.MatchId, ct);
            if (existing != null)
                throw new BadRequestException("There is already a pending reschedule proposal for this match. The other team must respond first.");

            var proposerClubId = proposerTeamId == match.HomeTeamId ? match.HomeTeam.ClubId : match.AwayTeam.ClubId;

            var reschedulingRequest = new MatchReschedulingRequest
            {
                MatchId = request.MatchId,
                RequestedByUserId = request.ProposerUserId,
                RequestedByClubId = proposerClubId,
                RequestedByTeamId = proposerTeamId,
                ProposedDate = request.ProposedDate,
                Reason = request.Reason,
                Status = RequestStatus.Pending,
                ExpiresAt = DateTime.UtcNow.AddHours(48)
            };

            await _reschedulingRepository.CreateAsync(reschedulingRequest, ct);

            match.SchedulingStatus = SchedulingStatus.ProposalPending;
            match.LastSchedulingUpdate = DateTime.UtcNow;
            await _matchRepository.UpdateAsync(match, ct);

            var otherTeamId = proposerTeamId == match.HomeTeamId ? match.AwayTeamId : match.HomeTeamId;
            var otherClubId = proposerTeamId == match.HomeTeamId ? match.AwayTeam.ClubId : match.HomeTeam.ClubId;
            var recipients = await _emailNotificationService.GetTeamRecipientsAsync(otherTeamId, otherClubId, ct);
            await _emailNotificationService.SendNotificationAsync(
                recipients,
                NotificationType.RescheduleProposed,
                $"Reschedule proposed: {match.HomeTeam.Name} vs {match.AwayTeam.Name}",
                $"A new date of {request.ProposedDate:d} has been proposed for your match between {match.HomeTeam.Name} and {match.AwayTeam.Name}. You have 48 hours to respond.",
                match.Id,
                linkPath: $"/matches/{match.Id}",
                linkText: "View match",
                ct: ct);

            return CommandResponse<bool>.Success(true, "Reschedule proposal submitted. The other team has 48 hours to respond.");
        }
    }
}
