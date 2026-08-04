using HRKošarka.Application.Contracts.Email;
using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models;
using HRKošarka.Application.Models.Email;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using Microsoft.Extensions.Options;

namespace HRKošarka.Application.Services
{
    public class EmailNotificationService
    {
        private static readonly SemaphoreSlim _smtpConcurrencyLimiter = new(5, 5);

        private readonly IEmailSender _emailSender;
        private readonly IEmailNotificationRepository _emailNotificationRepository;
        private readonly ITeamRepresentativeRepository _teamRepresentativeRepository;
        private readonly IUserFavoriteTeamRepository _userFavoriteTeamRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IIdentityLookupService _identityLookupService;
        private readonly IClubManagerService _clubManagerService;
        private readonly IAppLogger<EmailNotificationService> _logger;
        private readonly string _clientBaseUrl;

        public EmailNotificationService(
            IEmailSender emailSender,
            IEmailNotificationRepository emailNotificationRepository,
            ITeamRepresentativeRepository teamRepresentativeRepository,
            IUserFavoriteTeamRepository userFavoriteTeamRepository,
            ITeamRepository teamRepository,
            IIdentityLookupService identityLookupService,
            IClubManagerService clubManagerService,
            IAppLogger<EmailNotificationService> logger,
            IOptions<ClientAppSettings> clientAppSettings)
        {
            _emailSender = emailSender;
            _emailNotificationRepository = emailNotificationRepository;
            _teamRepresentativeRepository = teamRepresentativeRepository;
            _userFavoriteTeamRepository = userFavoriteTeamRepository;
            _teamRepository = teamRepository;
            _identityLookupService = identityLookupService;
            _clubManagerService = clubManagerService;
            _logger = logger;
            _clientBaseUrl = clientAppSettings.Value.BaseUrl.TrimEnd('/');
        }

        // All active reps for the team, plus the club's manager if one is assigned — everyone
        // who can act on behalf of the team gets notified, not just one or the other.
        public async Task<HashSet<string>> GetTeamRecipientsAsync(Guid teamId, Guid clubId, CancellationToken ct = default)
        {
            var reps = await _teamRepresentativeRepository.GetByTeamAsync(teamId, ct);
            var recipients = reps.Where(r => r.IsActive).Select(r => r.UserId).ToHashSet();

            var managerId = await _clubManagerService.GetClubManagerUserId(clubId, ct);
            if (managerId != null)
                recipients.Add(managerId);

            return recipients;
        }

        public async Task<HashSet<string>> GetTeamFanRecipientsAsync(Guid teamId, CancellationToken ct = default)
        {
            var userIds = await _userFavoriteTeamRepository.GetUserIdsByTeamAsync(teamId, ct);
            return userIds.ToHashSet();
        }

        public async Task<HashSet<string>> GetAdminRecipientsAsync(CancellationToken ct = default)
        {
            var userIds = await _identityLookupService.GetUserIdsInRoleAsync("Administrator", ct);
            return userIds.ToHashSet();
        }

        // Reps/manager of both match teams, optionally widened to fans of either team.
        // Covers the recurring "both teams (+ fans)" recipient shape shared by confirm/forfeit/reset.
        public async Task<HashSet<string>> GetMatchRecipientsAsync(
            Guid homeTeamId, Guid homeClubId, Guid awayTeamId, Guid awayClubId, bool includeFans, CancellationToken ct = default)
        {
            var recipients = await GetTeamRecipientsAsync(homeTeamId, homeClubId, ct);
            recipients.UnionWith(await GetTeamRecipientsAsync(awayTeamId, awayClubId, ct));

            if (includeFans)
            {
                recipients.UnionWith(await GetTeamFanRecipientsAsync(homeTeamId, ct));
                recipients.UnionWith(await GetTeamFanRecipientsAsync(awayTeamId, ct));
            }

            return recipients;
        }

        // Reps/managers of each newly-scheduled match's teams + fans of those teams, one notification per match.
        public async Task NotifyCupRoundAdvancedAsync(IReadOnlyList<Match> newMatches, CancellationToken ct = default)
        {
            var teamIds = newMatches.SelectMany(m => new[] { m.HomeTeamId, m.AwayTeamId }).Distinct().ToList();
            var teams = await _teamRepository.GetByIdsAsync(teamIds, ct);

            foreach (var newMatch in newMatches)
            {
                var homeTeam = teams[newMatch.HomeTeamId];
                var awayTeam = teams[newMatch.AwayTeamId];

                var recipients = await GetMatchRecipientsAsync(
                    newMatch.HomeTeamId, homeTeam.ClubId, newMatch.AwayTeamId, awayTeam.ClubId, includeFans: true, ct);

                await SendNotificationAsync(
                    recipients,
                    NotificationType.CupRoundAdvanced,
                    $"{newMatch.RoundName}: {homeTeam.Name} vs {awayTeam.Name}",
                    $"Your team advanced to {newMatch.RoundName}. Next match: {homeTeam.Name} vs {awayTeam.Name} on {newMatch.ActualScheduledDate:d}.",
                    newMatch.Id,
                    linkPath: $"/matches/{newMatch.Id}",
                    linkText: "View match",
                    ct: ct);
            }
        }

        // Resolves each recipient's email individually so recipients merged from different
        // groups (reps, fans, admins) are still deduplicated by UserId before any email is sent.
        // linkPath is a site-relative path (e.g. "/matches/{id}") rendered as an HTML anchor in the body.
        public async Task SendNotificationAsync(
            IEnumerable<string> recipientUserIds,
            NotificationType type,
            string subject,
            string body,
            Guid? matchId,
            string? linkPath = null,
            string linkText = "View details",
            CancellationToken ct = default)
        {
            // Body is sent as HTML (EmailSender sets IsBodyHtml = true), so encode user-derived
            // text (team/club names, dispute reasons) before appending the raw anchor markup.
            var encodedBody = System.Net.WebUtility.HtmlEncode(body);
            var fullBody = string.IsNullOrWhiteSpace(linkPath)
                ? encodedBody
                : $"{encodedBody}<br/><br/><a href=\"{_clientBaseUrl}{linkPath}\">{System.Net.WebUtility.HtmlEncode(linkText)}</a>";

            // Email lookups stay sequential (they share one scoped DbContext via UserManager,
            // which isn't safe for concurrent calls). Each lookup is just a fast PK read though —
            // the real cost is the SMTP round-trip below, so that's what actually needs to be parallel.
            var recipients = new List<(string UserId, string? Email)>();
            foreach (var userId in recipientUserIds.Distinct())
                recipients.Add((userId, await _identityLookupService.GetEmailByUserIdAsync(userId, ct)));

            // EmailSender opens a fresh SmtpClient (full TLS handshake + auth) per call, so sending
            // one-at-a-time made a 10-recipient notification take 10x a single send's latency and
            // blocked the whole command (confirm/dispute/etc.) on it. Fire them concurrently instead.
            var notifications = await Task.WhenAll(recipients.Select(r =>
                SendToRecipientAsync(r.UserId, r.Email, type, subject, fullBody, matchId)));

            if (notifications.Length > 0)
                await _emailNotificationRepository.CreateRangeAsync(notifications.ToList(), ct);
        }

        private async Task<EmailNotification> SendToRecipientAsync(
            string userId, string? email, NotificationType type, string subject, string fullBody, Guid? matchId)
        {
            var success = false;

            if (!string.IsNullOrWhiteSpace(email))
            {
                await _smtpConcurrencyLimiter.WaitAsync();
                try
                {
                    await _emailSender.SendEmail(new EmailMessage { To = email, Subject = subject, Body = fullBody });
                    success = true;
                    _logger.LogInformation("Sent {NotificationType} email to {Email}", type, email);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to send {NotificationType} email to {Email}", type, email);
                }
                finally
                {
                    _smtpConcurrencyLimiter.Release();
                }
            }
            else
            {
                _logger.LogWarning("Skipping {NotificationType} email for user {UserId}: no email on file", type, userId);
            }

            return new EmailNotification
            {
                UserId = userId,
                RecipientEmail = email,
                MatchId = matchId,
                NotificationType = type,
                Subject = subject,
                Body = fullBody,
                SentAt = DateTime.UtcNow,
                IsSuccessful = success
            };
        }
    }
}
