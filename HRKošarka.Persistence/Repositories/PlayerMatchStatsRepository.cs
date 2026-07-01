using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class PlayerMatchStatsRepository : GenericRepository<PlayerMatchStats>, IPlayerMatchStatsRepository
    {
        public PlayerMatchStatsRepository(HRDatabaseContext context) : base(context) { }

        public async Task<PlayerMatchStats?> GetByMatchAndPlayerAsync(Guid matchId, Guid playerId, CancellationToken ct = default)
        {
            return await _context.PlayerMatchStats
                .FirstOrDefaultAsync(s => s.MatchId == matchId && s.PlayerId == playerId, ct);
        }

        public async Task<List<PlayerMatchStats>> GetPlayedStatsForMatchAsync(Guid matchId, CancellationToken ct = default)
        {
            return await _context.PlayerMatchStats
                .Where(s => s.MatchId == matchId && !s.DidNotPlay && s.TeamId.HasValue)
                .ToListAsync(ct);
        }

        public async Task DeleteAllForMatchAsync(Guid matchId, CancellationToken ct = default)
        {
            await _context.PlayerMatchStats
                .Where(s => s.MatchId == matchId)
                .ExecuteDeleteAsync(ct);
        }

        public async Task<int> CountByMatchAndTeamAsync(Guid matchId, Guid teamId, CancellationToken ct = default)
        {
            return await _context.PlayerMatchStats
                .CountAsync(s => s.MatchId == matchId && s.TeamId == teamId, ct);
        }

        public async Task<List<PlayerMatchStats>> GetAllByPlayerWithMatchAsync(Guid playerId, CancellationToken ct = default)
        {
            return await _context.PlayerMatchStats
                .Include(s => s.Match).ThenInclude(m => m.HomeTeam)
                .Include(s => s.Match).ThenInclude(m => m.AwayTeam)
                .Where(s => s.PlayerId == playerId && !s.DidNotPlay && s.TeamId.HasValue && s.Match.PlayoffSeriesId == null)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<List<PlayerMatchStats>> GetAllByPlayerPlayoffWithMatchAsync(Guid playerId, CancellationToken ct = default)
        {
            return await _context.PlayerMatchStats
                .Include(s => s.Match).ThenInclude(m => m.HomeTeam)
                .Include(s => s.Match).ThenInclude(m => m.AwayTeam)
                .Include(s => s.Match).ThenInclude(m => m.League).ThenInclude(l => l.Season)
                .Include(s => s.Team)
                .Where(s => s.PlayerId == playerId && !s.DidNotPlay && s.TeamId.HasValue && s.Match.PlayoffSeriesId != null)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
