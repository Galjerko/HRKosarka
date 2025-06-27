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

    }
}
