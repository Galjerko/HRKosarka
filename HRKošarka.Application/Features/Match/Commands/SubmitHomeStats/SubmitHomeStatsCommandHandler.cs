using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.SubmitHomeStats
{
    public class SubmitHomeStatsCommandHandler : IRequestHandler<SubmitHomeStatsCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;

        public SubmitHomeStatsCommandHandler(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<CommandResponse<bool>> Handle(SubmitHomeStatsCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Match result is already confirmed.");

            if (match.ResultSubmissionStatus == ResultSubmissionStatus.Disputed)
                throw new BadRequestException("Match is disputed. An administrator must resolve it first.");

            if (match.ResultSubmissionStatus == ResultSubmissionStatus.HomeSubmitted)
                throw new BadRequestException("Stats have already been submitted. Waiting for the away team to confirm.");

            if (!string.IsNullOrEmpty(request.SubmitterClubId) &&
                match.HomeTeam.ClubId.ToString() != request.SubmitterClubId)
                throw new BadRequestException("Only the home team's club manager can submit the home stats.");

            if (!match.HomeScore.HasValue || !match.AwayScore.HasValue)
                throw new BadRequestException("Score must be entered before submitting.");

            match.ResultSubmissionStatus = ResultSubmissionStatus.HomeSubmitted;
            await _matchRepository.UpdateAsync(match, ct);

            return CommandResponse<bool>.Success(true, "Stats submitted. The away team can now confirm or dispute.");
        }
    }
}
