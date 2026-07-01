using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.League.Queries.GetLeagueStandings;
using HRKošarka.Application.Features.Match.Queries.GetPendingActions;
using HRKošarka.Application.Features.Team.Queries.GetTeamMatchHistory;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class MatchRepository : GenericRepository<Match>, IMatchRepository
    {
        public MatchRepository(HRDatabaseContext context) : base(context) { }

        public async Task<Match?> GetByIdWithIncludesAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Matches
                .Include(m => m.HomeTeam).ThenInclude(t => t.Club)
                .Include(m => m.AwayTeam).ThenInclude(t => t.Club)
                .Include(m => m.League)
                .FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task<Match?> GetMatchWithFullDetailsAsync(Guid id, CancellationToken ct = default)
        {
            return await _context.Matches
                .Include(m => m.League).ThenInclude(l => l.Season)
                .Include(m => m.HomeTeam).ThenInclude(t => t.Club)
                .Include(m => m.AwayTeam).ThenInclude(t => t.Club)
                .Include(m => m.PlayerStats)
                .FirstOrDefaultAsync(m => m.Id == id, ct);
        }

        public async Task<List<PendingActionDTO>> GetPendingActionsAsync(
            Guid? clubId, bool isAdmin, string? teamRepUserId = null, CancellationToken ct = default)
        {
            var result = new List<PendingActionDTO>();
            var now = DateTime.UtcNow;

            if (isAdmin)
            {
                // Admin: disputed matches
                var disputed = await _context.Matches
                    .Include(m => m.League)
                    .Include(m => m.HomeTeam)
                    .Include(m => m.AwayTeam)
                    .Where(m => m.ResultSubmissionStatus == HRKošarka.Domain.Common.ResultSubmissionStatus.Disputed
                             && !m.IsResultConfirmed)
                    .OrderBy(m => m.ActualScheduledDate)
                    .Select(m => new PendingActionDTO
                    {
                        MatchId = m.Id,
                        LeagueName = m.League.Name,
                        RoundName = m.RoundName ?? $"Round {m.Round}",
                        HomeTeamName = m.HomeTeam.Name,
                        AwayTeamName = m.AwayTeam.Name,
                        ScheduledDate = m.ActualScheduledDate,
                        ActionType = PendingActionType.ResolveDispute
                    })
                    .AsNoTracking()
                    .ToListAsync(ct);

                result.AddRange(disputed);
                return result;
            }

            if (clubId == null && string.IsNullOrEmpty(teamRepUserId)) return result;

            if (!string.IsNullOrEmpty(teamRepUserId))
            {
                var teamIds = await _context.TeamRepresentatives
                    .Where(tr => tr.UserId == teamRepUserId && tr.DeactivateDate == null)
                    .Select(tr => tr.TeamId)
                    .ToListAsync(ct);

                if (!teamIds.Any()) return result;

                var repSubmitHome = await _context.Matches
                    .Include(m => m.League).Include(m => m.HomeTeam).Include(m => m.AwayTeam)
                    .Where(m => teamIds.Contains(m.HomeTeamId)
                             && !m.IsResultConfirmed
                             && m.ResultSubmissionStatus == HRKošarka.Domain.Common.ResultSubmissionStatus.NotSubmitted
                             && m.Status != HRKošarka.Domain.Common.MatchStatus.Forfeit)
                    .OrderBy(m => m.ActualScheduledDate)
                    .Select(m => new PendingActionDTO
                    {
                        MatchId = m.Id, LeagueName = m.League.Name,
                        RoundName = m.RoundName ?? $"Round {m.Round}",
                        HomeTeamName = m.HomeTeam.Name, AwayTeamName = m.AwayTeam.Name,
                        ScheduledDate = m.ActualScheduledDate, ActionType = PendingActionType.SubmitHomeStats
                    }).AsNoTracking().ToListAsync(ct);
                result.AddRange(repSubmitHome);

                var repAwayPending = await _context.Matches
                    .Include(m => m.League).Include(m => m.HomeTeam).Include(m => m.AwayTeam)
                    .Where(m => teamIds.Contains(m.AwayTeamId)
                             && !m.IsResultConfirmed
                             && m.ResultSubmissionStatus == HRKošarka.Domain.Common.ResultSubmissionStatus.HomeSubmitted)
                    .OrderBy(m => m.ActualScheduledDate).AsNoTracking().ToListAsync(ct);

                foreach (var m in repAwayPending)
                {
                    var hasAwayStats = await _context.PlayerMatchStats
                        .AnyAsync(s => s.MatchId == m.Id && s.TeamId == m.AwayTeamId, ct);
                    result.Add(new PendingActionDTO
                    {
                        MatchId = m.Id, LeagueName = m.League.Name,
                        RoundName = m.RoundName ?? $"Round {m.Round}",
                        HomeTeamName = m.HomeTeam.Name, AwayTeamName = m.AwayTeam.Name,
                        ScheduledDate = m.ActualScheduledDate,
                        ActionType = hasAwayStats ? PendingActionType.ConfirmResult : PendingActionType.EnterAwayStats
                    });
                }

                var repProposals = await _context.MatchReschedulingRequests
                    .Include(r => r.Match).ThenInclude(m => m.League)
                    .Include(r => r.Match).ThenInclude(m => m.HomeTeam)
                    .Include(r => r.Match).ThenInclude(m => m.AwayTeam)
                    .Where(r => r.Status == HRKošarka.Domain.Common.RequestStatus.Pending
                             && r.ExpiresAt > now
                             && (teamIds.Contains(r.Match.HomeTeamId) || teamIds.Contains(r.Match.AwayTeamId)))
                    .OrderBy(r => r.ExpiresAt).AsNoTracking().ToListAsync(ct);

                foreach (var req in repProposals)
                {
                    bool isProposer = req.RequestedByUserId == teamRepUserId ||
                        (req.RequestedByTeamId.HasValue && teamIds.Contains(req.RequestedByTeamId.Value));
                    result.Add(new PendingActionDTO
                    {
                        MatchId = req.MatchId, LeagueName = req.Match.League.Name,
                        RoundName = req.Match.RoundName ?? $"Round {req.Match.Round}",
                        HomeTeamName = req.Match.HomeTeam.Name, AwayTeamName = req.Match.AwayTeam.Name,
                        ScheduledDate = req.Match.ActualScheduledDate,
                        ActionType = isProposer ? PendingActionType.ProposalPending : PendingActionType.RespondToProposal
                    });
                }

                return result.OrderBy(a => a.ScheduledDate).ToList();
            }

            if (clubId == null) return result;

            // Home manager: matches where my team is home, stats not yet submitted
            var submitHome = await _context.Matches
                .Include(m => m.League)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeTeam.ClubId == clubId
                         && !m.IsResultConfirmed
                         && m.ResultSubmissionStatus == HRKošarka.Domain.Common.ResultSubmissionStatus.NotSubmitted
                         && m.Status != HRKošarka.Domain.Common.MatchStatus.Forfeit)
                .OrderBy(m => m.ActualScheduledDate)
                .Select(m => new PendingActionDTO
                {
                    MatchId = m.Id,
                    LeagueName = m.League.Name,
                    RoundName = m.RoundName ?? $"Round {m.Round}",
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamName = m.AwayTeam.Name,
                    ScheduledDate = m.ActualScheduledDate,
                    ActionType = PendingActionType.SubmitHomeStats
                })
                .AsNoTracking()
                .ToListAsync(ct);
            result.AddRange(submitHome);

            // Away manager: home submitted, need to enter away stats + confirm
            var awayPending = await _context.Matches
                .Include(m => m.League)
                .Include(m => m.HomeTeam)
                .Include(m => m.AwayTeam)
                .Where(m => m.AwayTeam.ClubId == clubId
                         && !m.IsResultConfirmed
                         && m.ResultSubmissionStatus == HRKošarka.Domain.Common.ResultSubmissionStatus.HomeSubmitted)
                .OrderBy(m => m.ActualScheduledDate)
                .AsNoTracking()
                .ToListAsync(ct);

            foreach (var m in awayPending)
            {
                // Check if away stats have been entered already
                var hasAwayStats = await _context.PlayerMatchStats
                    .AnyAsync(s => s.MatchId == m.Id && s.TeamId == m.AwayTeamId, ct);

                result.Add(new PendingActionDTO
                {
                    MatchId = m.Id,
                    LeagueName = m.League.Name,
                    RoundName = m.RoundName ?? $"Round {m.Round}",
                    HomeTeamName = m.HomeTeam.Name,
                    AwayTeamName = m.AwayTeam.Name,
                    ScheduledDate = m.ActualScheduledDate,
                    ActionType = hasAwayStats ? PendingActionType.ConfirmResult : PendingActionType.EnterAwayStats
                });
            }

            // Reschedule proposals (both home and away)
            var proposals = await _context.MatchReschedulingRequests
                .Include(r => r.Match).ThenInclude(m => m.League)
                .Include(r => r.Match).ThenInclude(m => m.HomeTeam)
                .Include(r => r.Match).ThenInclude(m => m.AwayTeam)
                .Where(r => r.Status == HRKošarka.Domain.Common.RequestStatus.Pending
                         && r.ExpiresAt > now
                         && (r.Match.HomeTeam.ClubId == clubId || r.Match.AwayTeam.ClubId == clubId))
                .OrderBy(r => r.ExpiresAt)
                .AsNoTracking()
                .ToListAsync(ct);

            foreach (var req in proposals)
            {
                result.Add(new PendingActionDTO
                {
                    MatchId = req.MatchId,
                    LeagueName = req.Match.League.Name,
                    RoundName = req.Match.RoundName ?? $"Round {req.Match.Round}",
                    HomeTeamName = req.Match.HomeTeam.Name,
                    AwayTeamName = req.Match.AwayTeam.Name,
                    ScheduledDate = req.Match.ActualScheduledDate,
                    ActionType = req.RequestedByClubId == clubId
                        ? PendingActionType.ProposalPending
                        : PendingActionType.RespondToProposal
                });
            }

            return result.OrderBy(a => a.ScheduledDate).ToList();
        }

        public async Task<List<CompletedMatchSlimDTO>> GetCompletedMatchesByLeagueAsync(
            Guid leagueId, CancellationToken ct = default)
        {
            return await _context.Matches
                .Where(m => m.LeagueId == leagueId && m.Status == MatchStatus.Completed)
                .OrderByDescending(m => m.ActualScheduledDate)
                .Select(m => new CompletedMatchSlimDTO
                {
                    HomeTeamId = m.HomeTeamId,
                    AwayTeamId = m.AwayTeamId,
                    HomeScore = m.HomeScore,
                    AwayScore = m.AwayScore
                })
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<List<Match>> GetRoundMatchesAsync(Guid leagueId, int round, CancellationToken ct = default)
        {
            return await _context.Matches
                .Where(m => m.LeagueId == leagueId && m.Round == round)
                .OrderBy(m => m.DateCreated)
                .ThenBy(m => m.HomeTeamId)
                .ToListAsync(ct);
        }

        public async Task<List<TeamMatchHistoryItemDTO>> GetTeamMatchHistoryAsync(
            Guid teamId, CancellationToken ct = default)
        {
            return await _context.Matches
                .Include(m => m.League).ThenInclude(l => l.Season)
                .Include(m => m.HomeTeam).ThenInclude(t => t.Club)
                .Include(m => m.AwayTeam)
                .Where(m => m.HomeTeamId == teamId || m.AwayTeamId == teamId)
                .OrderByDescending(m => m.ActualScheduledDate)
                .Select(m => new TeamMatchHistoryItemDTO
                {
                    MatchId = m.Id,
                    Round = m.Round,
                    RoundName = m.RoundName,
                    LeagueId = m.LeagueId,
                    LeagueName = m.League.Name,
                    SeasonName = m.League.Season.Name,
                    CompetitionType = m.League.CompetitionType,
                    IsPlayoff = m.PlayoffSeriesId != null,
                    IsHome = m.HomeTeamId == teamId,
                    OpponentTeamId = m.HomeTeamId == teamId ? m.AwayTeamId : m.HomeTeamId,
                    OpponentTeamName = m.HomeTeamId == teamId ? m.AwayTeam.Name : m.HomeTeam.Name,
                    ScheduledDate = m.ActualScheduledDate,
                    Status = m.Status,
                    TeamScore = m.HomeTeamId == teamId ? m.HomeScore : m.AwayScore,
                    OpponentScore = m.HomeTeamId == teamId ? m.AwayScore : m.HomeScore,
                    Venue = m.VenueOverride ?? m.HomeTeam.Club.VenueName,
                })
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
