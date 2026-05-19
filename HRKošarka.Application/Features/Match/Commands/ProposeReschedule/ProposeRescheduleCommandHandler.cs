using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.ProposeReschedule
{
    public class ProposeRescheduleCommandHandler : IRequestHandler<ProposeRescheduleCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchReschedulingRequestRepository _reschedulingRepository;

        public ProposeRescheduleCommandHandler(
            IMatchRepository matchRepository,
            IMatchReschedulingRequestRepository reschedulingRepository)
        {
            _matchRepository = matchRepository;
            _reschedulingRepository = reschedulingRepository;
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

            if (match.HomeTeam.ClubId != request.ProposerClubId && match.AwayTeam.ClubId != request.ProposerClubId)
                throw new BadRequestException("Only a club manager from one of the match teams can propose a reschedule.");

            await _reschedulingRepository.ExpireStaleForMatchAsync(request.MatchId, ct);

            var existing = await _reschedulingRepository.GetActiveForMatchAsync(request.MatchId, ct);
            if (existing != null)
                throw new BadRequestException("There is already a pending reschedule proposal for this match. The other team must respond first.");

            var reschedulingRequest = new MatchReschedulingRequest
            {
                MatchId = request.MatchId,
                RequestedByUserId = request.ProposerUserId,
                RequestedByClubId = request.ProposerClubId,
                ProposedDate = request.ProposedDate,
                Reason = request.Reason,
                Status = RequestStatus.Pending,
                ExpiresAt = DateTime.UtcNow.AddHours(48)
            };

            await _reschedulingRepository.CreateAsync(reschedulingRequest, ct);

            match.SchedulingStatus = SchedulingStatus.ProposalPending;
            match.LastSchedulingUpdate = DateTime.UtcNow;
            await _matchRepository.UpdateAsync(match, ct);

            return CommandResponse<bool>.Success(true, "Reschedule proposal submitted. The other team has 48 hours to respond.");
        }
    }
}
