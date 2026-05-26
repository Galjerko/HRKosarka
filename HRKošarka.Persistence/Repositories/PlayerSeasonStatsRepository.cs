using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.Team.Queries.GetTeamLeaguePlayerStats;
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

        public async Task<List<TeamPlayerStatDTO>> GetByTeamAndLeagueAsync(Guid teamId, Guid leagueId, CancellationToken ct = default)
        {
            return await _context.PlayerSeasonStats
                .Include(s => s.Player)
                .Where(s => s.TeamId == teamId && s.LeagueId == leagueId && s.MatchesPlayed > 0)
                .OrderByDescending(s => s.AveragePoints)
                .ThenByDescending(s => s.AverageThreePointers)
                .Select(s => new TeamPlayerStatDTO
                {
                    PlayerId = s.PlayerId,
                    PlayerName = $"{s.Player.FirstName} {s.Player.LastName}",
                    GamesPlayed = s.MatchesPlayed,
                    PPG = s.AveragePoints,
                    ThreePG = s.AverageThreePointers,
                    FPG = s.AverageFouls,
                    TotalPoints = s.TotalPoints,
                    TotalThreePointers = s.TotalThreePointers,
                    TotalFouls = s.TotalFouls
                })
                .AsNoTracking()
                .ToListAsync(ct);
        }
    }
}
