using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.Team.Queries.GetMyRepresentativeships;
using HRKošarka.Application.Features.Team.Queries.GetTeamRepresentatives;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class TeamRepresentativeRepository : GenericRepository<TeamRepresentative>, ITeamRepresentativeRepository
    {
        public TeamRepresentativeRepository(HRDatabaseContext context) : base(context) { }

        public async Task<List<TeamRepresentativeDTO>> GetByTeamAsync(Guid teamId, CancellationToken ct = default)
        {
            return await _context.Database
                .SqlQuery<TeamRepresentativeDTO>($"""
                    SELECT
                        tr.Id,
                        tr.UserId,
                        ISNULL(u.Email, tr.UserId) AS UserEmail,
                        tr.AssignedDate,
                        CAST(CASE WHEN tr.DeactivateDate IS NULL THEN 1 ELSE 0 END AS BIT) AS IsActive
                    FROM TeamRepresentatives tr
                    LEFT JOIN AspNetUsers u ON tr.UserId = u.Id
                    WHERE tr.TeamId = {teamId} AND tr.DateDeleted IS NULL
                    ORDER BY tr.AssignedDate
                    """)
                .ToListAsync(ct);
        }

        public async Task<TeamRepresentative?> GetByUserAndTeamAsync(
            string userId, Guid teamId, CancellationToken ct = default)
        {
            return await _context.TeamRepresentatives
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(tr => tr.UserId == userId && tr.TeamId == teamId && tr.DateDeleted == null, ct);
        }

        public async Task<bool> IsActiveRepForTeamAsync(
            string userId, Guid teamId, CancellationToken ct = default)
        {
            return await _context.TeamRepresentatives
                .AnyAsync(tr => tr.UserId == userId && tr.TeamId == teamId && tr.DeactivateDate == null, ct);
        }

        public async Task<List<TeamRepMembershipDTO>> GetActiveMembershipsByUserAsync(
            string userId, CancellationToken ct = default)
        {
            return await _context.TeamRepresentatives
                .Include(tr => tr.Team).ThenInclude(t => t.Club)
                .Where(tr => tr.UserId == userId && tr.DeactivateDate == null && tr.Team.DateDeleted == null)
                .Select(tr => new TeamRepMembershipDTO
                {
                    TeamId = tr.TeamId,
                    TeamName = tr.Team.Name,
                    ClubName = tr.Team.Club.Name,
                    TeamIsActive = tr.Team.DeactivateDate == null
                })
                .AsNoTracking()
                .ToListAsync(ct);
        }

        public async Task<List<Guid>> GetActiveTeamIdsByUserAsync(
            string userId, CancellationToken ct = default)
        {
            return await _context.TeamRepresentatives
                .Where(tr => tr.UserId == userId && tr.DeactivateDate == null)
                .Select(tr => tr.TeamId)
                .ToListAsync(ct);
        }

        public async Task DeactivateAllForTeamAsync(Guid teamId, CancellationToken ct = default)
        {
            var reps = await _context.TeamRepresentatives
                .Where(tr => tr.TeamId == teamId && tr.DeactivateDate == null)
                .ToListAsync(ct);
            foreach (var rep in reps)
                rep.DeactivateDate = DateTime.Now;
            await _context.SaveChangesAsync(ct);
        }
    }
}
