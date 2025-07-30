using HRKošarka.Application.Contracts.Persistence;
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

        public async Task<bool> IsClubNameUnique(string name, Guid? excludeId = null)
        {
            var query = _context.Clubs.Where(x => x.Name == name);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync() == false;
        }

        public async Task<Club?> GetClubWithTeamsAsync(Guid clubId)
        {
            return await _context.Clubs
                .Include(c => c.Teams.Where(t => t.DateDeleted == null))
                    .ThenInclude(t => t.AgeCategory)
                .FirstOrDefaultAsync(c => c.Id == clubId);
        }
    }
}
