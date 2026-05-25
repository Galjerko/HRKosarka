using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.RespondToReschedule
{
    public class RespondToRescheduleCommandHandler : IRequestHandler<RespondToRescheduleCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IMatchReschedulingRequestRepository _reschedulingRepository;
        private readonly ITeamRepresentativeRepository _repRepository;

        public RespondToRescheduleCommandHandler(
            IMatchRepository matchRepository,
            IMatchReschedulingRequestRepository reschedulingRepository,
            ITeamRepresentativeRepository repRepository)
        {
            _matchRepository = matchRepository;
            _reschedulingRepository = reschedulingRepository;
            _repRepository = repRepository;
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

            if (request.Accept)
            {
                proposal.Status = RequestStatus.Accepted;
                match.ActualScheduledDate = proposal.ProposedDate;
                match.Status = MatchStatus.Rescheduled;
                match.SchedulingStatus = SchedulingStatus.Agreed;
                match.LastSchedulingUpdate = DateTime.UtcNow;
                await _matchRepository.UpdateAsync(match, ct);
                await _reschedulingRepository.UpdateAsync(proposal, ct);
                return CommandResponse<bool>.Success(true, "Reschedule accepted. The match has been moved to the new date.");
            }
            else
            {
                proposal.Status = RequestStatus.Rejected;
                match.SchedulingStatus = SchedulingStatus.Default;
                match.LastSchedulingUpdate = DateTime.UtcNow;
                await _matchRepository.UpdateAsync(match, ct);
                await _reschedulingRepository.UpdateAsync(proposal, ct);
                return CommandResponse<bool>.Success(true, "Reschedule rejected. The match remains on the original date.");
            }
        }
    }
}
