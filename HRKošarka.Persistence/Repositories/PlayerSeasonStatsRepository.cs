using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class PlayerSeasonStatsRepository : GenericRepository<PlayerSeasonStats>, IPlayerSeasonStatsRepository
    {
        public PlayerSeasonStatsRepository(HRDatabaseContext context) : base(context) { }

        public async Task<PlayerSeasonStats?> GetByPlayerAndLeagueAsync(Guid playerId, Guid leagueId, Guid seasonId, CancellationToken ct = default)
        {
            return await _context.PlayerSeasonStats
                .FirstOrDefaultAsync(s => s.PlayerId == playerId && s.LeagueId == leagueId && s.SeasonId == seasonId, ct);
        }

        public async Task<List<PlayerSeasonStats>> GetAllByPlayerAsync(Guid playerId, CancellationToken ct = default)
        {
            return await _context.PlayerSeasonStats
                .Include(s => s.Season)
                .Include(s => s.League)
                .Include(s => s.Team)
                .Where(s => s.PlayerId == playerId && s.MatchesPlayed > 0)
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
