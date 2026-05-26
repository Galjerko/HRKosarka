using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class LeagueStandingRepository : GenericRepository<LeagueStanding>, ILeagueStandingRepository
    {
        public LeagueStandingRepository(HRDatabaseContext context) : base(context) { }

        public async Task<LeagueStanding?> GetByTeamAndLeagueAsync(Guid teamId, Guid leagueId, Guid seasonId, CancellationToken ct = default)
        {
            return await _context.LeagueStandings
                .FirstOrDefaultAsync(s => s.TeamId == teamId && s.LeagueId == leagueId && s.SeasonId == seasonId, ct);
        }

        public async Task<LeagueStanding?> GetByTeamAndLeagueAsync(Guid teamId, Guid leagueId, CancellationToken ct = default)
        {
            return await _context.LeagueStandings
                .FirstOrDefaultAsync(s => s.TeamId == teamId && s.LeagueId == leagueId, ct);
        }

        public async Task<List<LeagueStanding>> GetByLeagueAsync(Guid leagueId, CancellationToken ct = default)
        {
            return await _context.LeagueStandings
                .Where(s => s.LeagueId == leagueId)
                .OrderByDescending(s => s.LeaguePoints)
                .ThenByDescending(s => s.PointsDifference)
                .ThenByDescending(s => s.PointsFor)
                .ToListAsync(ct);
        }
    }
}
