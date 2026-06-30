using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class PlayerTeamHistoryRepository : GenericRepository<PlayerTeamHistory>, IPlayerTeamHistoryRepository
    {
        public PlayerTeamHistoryRepository(HRDatabaseContext context) : base(context)
        {
        }

        public async Task<PlayerTeamHistory?> GetActiveByPlayerAndTeamAsync(
            Guid playerId, Guid teamId, CancellationToken cancellationToken = default)
        {
            return await _context.PlayerTeamHistory
                .FirstOrDefaultAsync(
                    pth => pth.PlayerId == playerId && pth.TeamId == teamId && pth.IsActive,
                    cancellationToken);
        }

        public async Task<List<PlayerTeamHistory>> GetActiveByPlayerAsync(
            Guid playerId, CancellationToken cancellationToken = default)
        {
            return await _context.PlayerTeamHistory
                .Include(pth => pth.Team)
                    .ThenInclude(t => t.Club)
                .Include(pth => pth.Team)
                    .ThenInclude(t => t.AgeCategory)
                .Include(pth => pth.Season)
                .Where(pth => pth.PlayerId == playerId && pth.IsActive)
                .OrderBy(pth => pth.Team.AgeCategory.Name)
                .ThenBy(pth => pth.Team.Name)
                .ToListAsync(cancellationToken);
        }

        public async Task<List<PlayerTeamHistory>> GetAllByPlayerAsync(
            Guid playerId, CancellationToken cancellationToken = default)
        {
            return await _context.PlayerTeamHistory
                .Include(pth => pth.Team)
                    .ThenInclude(t => t.Club)
                .Include(pth => pth.Team)
                    .ThenInclude(t => t.AgeCategory)
                .Include(pth => pth.Season)
                .Where(pth => pth.PlayerId == playerId)
                .OrderByDescending(pth => pth.IsActive)
                .ThenByDescending(pth => pth.JoinDate)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> IsJerseyNumberAvailableAsync(
            Guid teamId,
            int jerseyNumber,
            Guid? excludeHistoryId = null,
            CancellationToken cancellationToken = default)
        {
            var query = _context.PlayerTeamHistory
                .Where(pth => pth.TeamId == teamId && pth.IsActive && pth.JerseyNumber == jerseyNumber);

            if (excludeHistoryId.HasValue)
            {
                query = query.Where(pth => pth.Id != excludeHistoryId.Value);
            }

            return await query.AnyAsync(cancellationToken) == false;
        }

        public async Task<bool> HasActiveAssignmentsForPlayerAsync(Guid playerId, CancellationToken cancellationToken = default)
        {
            return await _context.PlayerTeamHistory
                .AnyAsync(pth => pth.PlayerId == playerId && pth.IsActive, cancellationToken);
        }

        public async Task<bool> HasActiveAssignmentsForTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            return await _context.PlayerTeamHistory
                .AnyAsync(pth => pth.TeamId == teamId && pth.IsActive, cancellationToken);
        }

        public async Task DeactivateAllForPlayerAsync(Guid playerId, CancellationToken cancellationToken = default)
        {
            var assignments = await _context.PlayerTeamHistory
                .Where(pth => pth.PlayerId == playerId && pth.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var a in assignments)
            {
                a.IsActive = false;
                a.LeaveDate = DateTime.Now;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task DeactivateAllForTeamAsync(Guid teamId, CancellationToken cancellationToken = default)
        {
            var assignments = await _context.PlayerTeamHistory
                .Where(pth => pth.TeamId == teamId && pth.IsActive)
                .ToListAsync(cancellationToken);

            foreach (var a in assignments)
            {
                a.IsActive = false;
                a.LeaveDate = DateTime.Now;
            }

            await _context.SaveChangesAsync(cancellationToken);
        }

        public async Task<List<PlayerTeamHistory>> GetRosterAsync(Guid teamId, Guid seasonId, CancellationToken cancellationToken = default)
        {
            return await _context.PlayerTeamHistory
                .Include(pth => pth.Player)
                .Where(pth => pth.TeamId == teamId
                           && pth.SeasonId == seasonId
                           && pth.IsActive
                           && pth.Player.DateDeleted == null)
                .OrderBy(pth => pth.Player.Position).ThenBy(pth => pth.JerseyNumber)
                .AsNoTracking()
                .ToListAsync(cancellationToken);
        }
    }
}
