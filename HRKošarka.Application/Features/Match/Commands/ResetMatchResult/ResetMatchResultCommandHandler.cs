using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.ResetMatchResult
{
    public class ResetMatchResultCommandHandler : IRequestHandler<ResetMatchResultCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerMatchStatsRepository _statsRepository;

        public ResetMatchResultCommandHandler(
            IMatchRepository matchRepository,
            IPlayerMatchStatsRepository statsRepository)
        {
            _matchRepository = matchRepository;
            _statsRepository = statsRepository;
        }

        public async Task<CommandResponse<bool>> Handle(ResetMatchResultCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("A confirmed match result cannot be reset.");

            match.HomeScore = null;
            match.AwayScore = null;
            match.ResultSubmissionStatus = ResultSubmissionStatus.NotSubmitted;
            match.DisputeReason = null;
            await _matchRepository.UpdateAsync(match, ct);

            await _statsRepository.DeleteAllForMatchAsync(match.Id, ct);

            return CommandResponse<bool>.Success(true, "Match result has been reset. The home team must re-enter the result.");
        }
    }
}
