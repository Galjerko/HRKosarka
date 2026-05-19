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

        public RespondToRescheduleCommandHandler(
            IMatchRepository matchRepository,
            IMatchReschedulingRequestRepository reschedulingRepository)
        {
            _matchRepository = matchRepository;
            _reschedulingRepository = reschedulingRepository;
        }

        public async Task<CommandResponse<bool>> Handle(RespondToRescheduleCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            var proposal = await _reschedulingRepository.GetActiveForMatchAsync(request.MatchId, ct)
                ?? throw new BadRequestException("No active reschedule proposal found for this match.");

            if (proposal.RequestedByClubId == request.ResponderClubId)
                throw new BadRequestException("You cannot respond to your own reschedule proposal.");

            if (match.HomeTeam.ClubId != request.ResponderClubId && match.AwayTeam.ClubId != request.ResponderClubId)
                throw new BadRequestException("Only a club manager from one of the match teams can respond.");

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
