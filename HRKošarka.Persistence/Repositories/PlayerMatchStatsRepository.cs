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
    }
}
