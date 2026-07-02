using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.UserFavoriteTeam.Queries.GetMyFavoriteTeams;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class UserFavoriteTeamRepository : GenericRepository<UserFavoriteTeam>, IUserFavoriteTeamRepository
    {
        public UserFavoriteTeamRepository(HRDatabaseContext context) : base(context) { }

        public async Task<UserFavoriteTeam?> GetByUserAndTeamAsync(
            string userId, Guid teamId, CancellationToken ct = default)
        {
            return await _context.UserFavoriteTeams
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(f => f.UserId == userId && f.TeamId == teamId, ct);
        }

        public async Task<bool> IsFavoritedAsync(
            string userId, Guid teamId, CancellationToken ct = default)
        {
            return await _context.UserFavoriteTeams
                .AnyAsync(f => f.UserId == userId && f.TeamId == teamId, ct);
        }

        public async Task<List<FavoriteTeamDTO>> GetByUserAsync(
            string userId, CancellationToken ct = default)
        {
            return await _context.UserFavoriteTeams
                .Include(f => f.Team).ThenInclude(t => t.Club)
                .Where(f => f.UserId == userId && f.Team.DateDeleted == null)
                .OrderBy(f => f.Team.Name)
                .Select(f => new FavoriteTeamDTO
                {
                    TeamId = f.TeamId,
                    TeamName = f.Team.Name,
                    ClubName = f.Team.Club.Name,
                    TeamIsActive = f.Team.DeactivateDate == null
                })
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<List<string>> GetUserIdsByTeamAsync(
            Guid teamId, CancellationToken ct = default)
        {
            return await _context.UserFavoriteTeams
                .Where(f => f.TeamId == teamId && f.NotifyByEmail)
                .Select(f => f.UserId)
                .ToListAsync(ct);
        }
    }
}
