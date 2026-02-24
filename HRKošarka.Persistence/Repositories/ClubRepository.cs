using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Features.Club.Queries.GetClubsWIthoutManager;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class ClubRepository : GenericRepository<Club>, IClubRepository
    {
        public ClubRepository(HRDatabaseContext context) : base(context)
        {
        }

        public async Task<bool> IsClubNameUnique(string name, Guid? excludeId = null, CancellationToken cancellationToken = default)
        {
            var query = _context.Clubs.Where(x => x.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync(cancellationToken) == false;
        }

        public async Task<Club?> GetClubWithTeamsAsync(Guid clubId, CancellationToken cancellationToken = default)
        {
            return await _context.Clubs
                .Include(c => c.Teams.Where(t => t.DateDeleted == null))
                    .ThenInclude(t => t.AgeCategory)
                .FirstOrDefaultAsync(c => c.Id == clubId, cancellationToken);
        }

        public async Task<List<ClubWithoutManagerDTO>> GetClubsWithoutManagerAsync(
                List<Guid> managedClubIds,
                string? searchTerm,
                CancellationToken cancellationToken = default)
        {
            var query = _context.Clubs
                .AsNoTracking()
                .Where(c => c.DeactivateDate == null && !managedClubIds.Contains(c.Id));

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                var term = searchTerm.Trim().ToLower();
                query = query.Where(c => c.Name.ToLower().Contains(term));
            }

            return await query
                .OrderBy(c => c.Name)
                .Select(c => new ClubWithoutManagerDTO
                {
                    Id = c.Id,
                    Name = c.Name
                })
                .ToListAsync(cancellationToken);
        }
    }
}
