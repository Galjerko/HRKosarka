using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Application.Services;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.SubmitHomeStats
{
    public class SubmitHomeStatsCommandHandler : IRequestHandler<SubmitHomeStatsCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public SubmitHomeStatsCommandHandler(
            IMatchRepository matchRepository,
            ITeamRepresentativeRepository repRepository,
            EmailNotificationService emailNotificationService)
        {
            _matchRepository = matchRepository;
            _repRepository = repRepository;
            _emailNotificationService = emailNotificationService;
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

            bool isAdmin = string.IsNullOrEmpty(request.SubmitterClubId) && string.IsNullOrEmpty(request.SubmitterUserId);
            if (!isAdmin)
            {
                bool authorized = false;
                if (!string.IsNullOrEmpty(request.SubmitterClubId))
                    authorized = match.HomeTeam.ClubId.ToString() == request.SubmitterClubId;
                if (!authorized && !string.IsNullOrEmpty(request.SubmitterUserId))
                    authorized = await _repRepository.IsActiveRepForTeamAsync(request.SubmitterUserId, match.HomeTeamId, ct);
                if (!authorized)
                    throw new BadRequestException("Only the home team's manager or representative can submit the home stats.");
            }

            if (!match.HomeScore.HasValue || !match.AwayScore.HasValue)
                throw new BadRequestException("Score must be entered before submitting.");

            match.ResultSubmissionStatus = ResultSubmissionStatus.HomeSubmitted;
            await _matchRepository.UpdateAsync(match, ct);

            var awayRecipients = await _emailNotificationService.GetTeamRecipientsAsync(
                match.AwayTeamId, match.AwayTeam.ClubId, ct);
            await _emailNotificationService.SendNotificationAsync(
                awayRecipients,
                NotificationType.StatsSubmitted,
                $"Action required: confirm the result of {match.HomeTeam.Name} vs {match.AwayTeam.Name}",
                $"{match.HomeTeam.Name} submitted the result ({match.HomeScore}-{match.AwayScore}) for your match on {match.ActualScheduledDate:d}. Please confirm or dispute it.",
                match.Id,
                linkPath: $"/matches/{match.Id}",
                linkText: "View match",
                ct: ct);

            return CommandResponse<bool>.Success(true, "Stats submitted. The away team can now confirm or dispute.");
        }
    }
}
