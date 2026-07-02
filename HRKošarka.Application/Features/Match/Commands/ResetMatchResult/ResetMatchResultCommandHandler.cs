using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Application.Services;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.ResetMatchResult
{
    public class ResetMatchResultCommandHandler : IRequestHandler<ResetMatchResultCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly IPlayerMatchStatsRepository _statsRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public ResetMatchResultCommandHandler(
            IMatchRepository matchRepository,
            IPlayerMatchStatsRepository statsRepository,
            EmailNotificationService emailNotificationService)
        {
            _matchRepository = matchRepository;
            _statsRepository = statsRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<CommandResponse<bool>> Handle(ResetMatchResultCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("A confirmed match result cannot be reset.");

            match.HomeScore = null;
            match.AwayScore = null;
            match.ResultSubmissionStatus = ResultSubmissionStatus.NotSubmitted;
            match.DisputeReason = null;
            await _matchRepository.UpdateAsync(match, ct);

            await _statsRepository.DeleteAllForMatchAsync(match.Id, ct);

            var recipients = await _emailNotificationService.GetMatchRecipientsAsync(
                match.HomeTeamId, match.HomeTeam.ClubId, match.AwayTeamId, match.AwayTeam.ClubId, includeFans: false, ct);
            await _emailNotificationService.SendNotificationAsync(
                recipients,
                NotificationType.MatchReset,
                $"Match result reset: {match.HomeTeam.Name} vs {match.AwayTeam.Name}",
                $"The result of the match between {match.HomeTeam.Name} and {match.AwayTeam.Name} on {match.ActualScheduledDate:d} has been reset by an administrator. The home team must re-enter the result.",
                match.Id,
                linkPath: $"/matches/{match.Id}",
                linkText: "View match",
                ct: ct);

            return CommandResponse<bool>.Success(true, "Match result has been reset. The home team must re-enter the result.");
        }
    }
}
