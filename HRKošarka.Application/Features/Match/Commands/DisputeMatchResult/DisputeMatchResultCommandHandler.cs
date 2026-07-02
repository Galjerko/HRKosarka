using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Application.Services;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.DisputeMatchResult
{
    public class DisputeMatchResultCommandHandler : IRequestHandler<DisputeMatchResultCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public DisputeMatchResultCommandHandler(
            IMatchRepository matchRepository,
            ITeamRepresentativeRepository repRepository,
            EmailNotificationService emailNotificationService)
        {
            _matchRepository = matchRepository;
            _repRepository = repRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<CommandResponse<bool>> Handle(DisputeMatchResultCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Match result is already confirmed.");

            if (match.ResultSubmissionStatus != ResultSubmissionStatus.HomeSubmitted)
                throw new BadRequestException("There is no submitted result to dispute.");

            bool isAdmin = string.IsNullOrEmpty(request.DisputerClubId) && string.IsNullOrEmpty(request.DisputerUserId);
            if (!isAdmin)
            {
                bool authorized = false;
                if (!string.IsNullOrEmpty(request.DisputerClubId))
                    authorized = match.AwayTeam.ClubId.ToString() == request.DisputerClubId;
                if (!authorized && !string.IsNullOrEmpty(request.DisputerUserId))
                    authorized = await _repRepository.IsActiveRepForTeamAsync(request.DisputerUserId, match.AwayTeamId, ct);
                if (!authorized)
                    throw new BadRequestException("Only the away team's manager or representative can dispute the result.");
            }

            if (string.IsNullOrWhiteSpace(request.Reason))
                throw new BadRequestException("A reason must be provided when disputing a result.");

            match.ResultSubmissionStatus = ResultSubmissionStatus.Disputed;
            match.DisputeReason = request.Reason.Trim();
            await _matchRepository.UpdateAsync(match, ct);

            var recipients = await _emailNotificationService.GetAdminRecipientsAsync(ct);
            recipients.UnionWith(await _emailNotificationService.GetTeamRecipientsAsync(
                match.HomeTeamId, match.HomeTeam.ClubId, ct));
            await _emailNotificationService.SendNotificationAsync(
                recipients,
                NotificationType.MatchDisputed,
                $"Result disputed: {match.HomeTeam.Name} vs {match.AwayTeam.Name}",
                $"The result of the match between {match.HomeTeam.Name} and {match.AwayTeam.Name} on {match.ActualScheduledDate:d} was disputed. Reason: {match.DisputeReason}",
                match.Id,
                linkPath: $"/matches/{match.Id}",
                linkText: "View match",
                ct: ct);

            return CommandResponse<bool>.Success(true, "Result disputed. An administrator will review.");
        }
    }
}
