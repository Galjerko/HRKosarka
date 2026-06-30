using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class PlayoffRepository : GenericRepository<PlayoffSeries>, IPlayoffRepository
    {
        public PlayoffRepository(HRDatabaseContext context) : base(context)
        {
        }

        public async Task<PlayoffSeries?> GetSeriesWithMatchesAsync(Guid seriesId, CancellationToken ct = default)
        {
            return await _context.PlayoffSeries
                .Include(s => s.Matches)
                .Include(s => s.HomeTeam)
                .Include(s => s.AwayTeam)
                .FirstOrDefaultAsync(s => s.Id == seriesId, ct);
        }

        public async Task<List<PlayoffSeries>> GetSeriesByRoundAsync(Guid leagueId, int roundNumber, CancellationToken ct = default)
        {
            return await _context.PlayoffSeries
                .Include(s => s.Matches)
                .Where(s => s.LeagueId == leagueId && s.RoundNumber == roundNumber)
                .OrderBy(s => s.SeriesNumber)
                .ToListAsync(ct);
        }

        public async Task<List<PlayoffSeries>> GetAllSeriesForLeagueAsync(Guid leagueId, CancellationToken ct = default)
        {
            return await _context.PlayoffSeries
                .Include(s => s.HomeTeam).ThenInclude(t => t!.Club)
                .Include(s => s.AwayTeam).ThenInclude(t => t!.Club)
                .Include(s => s.Matches).ThenInclude(m => m.HomeTeam!).ThenInclude(t => t.Club!)
                .Where(s => s.LeagueId == leagueId)
                .OrderBy(s => s.RoundNumber)
                .ThenBy(s => s.SeriesNumber)
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<bool> HasIncompleteSeriesAsync(Guid leagueId, CancellationToken ct = default)
        {
            return await _context.PlayoffSeries
                .AnyAsync(s => s.LeagueId == leagueId && !s.IsCompleted, ct);
        }

        public async Task CreateInitialBracketAsync(List<PlayoffSeries> allSeries, League league, CancellationToken ct = default)
        {
            await _context.PlayoffSeries.AddRangeAsync(allSeries, ct);
            _context.Entry(league).State = EntityState.Modified;
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateSeriesAndActivateNextAsync(
            PlayoffSeries updatedSeries,
            List<PlayoffSeries> nextRoundStubsToActivate,
            List<Match> matchesToCreate,
            CancellationToken ct = default)
        {
            _context.Entry(updatedSeries).State = EntityState.Modified;

            foreach (var stub in nextRoundStubsToActivate)
            {
                _context.Entry(stub).State = EntityState.Modified;
            }

            if (matchesToCreate.Count > 0)
            {
                await _context.Matches.AddRangeAsync(matchesToCreate, ct);
            }

            await _context.SaveChangesAsync(ct);
        }
    }
}
