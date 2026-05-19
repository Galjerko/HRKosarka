using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.DisputeMatchResult
{
    public class DisputeMatchResultCommandHandler : IRequestHandler<DisputeMatchResultCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;

        public DisputeMatchResultCommandHandler(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<CommandResponse<bool>> Handle(DisputeMatchResultCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Match result is already confirmed.");

            if (match.ResultSubmissionStatus != ResultSubmissionStatus.HomeSubmitted)
                throw new BadRequestException("There is no submitted result to dispute.");

            if (!string.IsNullOrEmpty(request.DisputerClubId) &&
                match.AwayTeam.ClubId.ToString() != request.DisputerClubId)
                throw new BadRequestException("Only the away team's club manager can dispute the result.");

            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new BadRequestException("A reason must be provided when disputing a result.");

            match.ResultSubmissionStatus = ResultSubmissionStatus.Disputed;
            match.DisputeReason = request.Reason.Trim();
            await _matchRepository.UpdateAsync(match, ct);

            return CommandResponse<bool>.Success(true, "Result disputed. An administrator will review.");
        }
    }
}
