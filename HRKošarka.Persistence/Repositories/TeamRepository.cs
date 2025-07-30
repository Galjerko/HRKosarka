// TeamRepository.cs
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;
using Microsoft.EntityFrameworkCore;

namespace HRKošarka.Persistence.Repositories
{
    public class TeamRepository : GenericRepository<Team>, ITeamRepository
    {
        public TeamRepository(HRDatabaseContext context) : base(context)
        {
        }

        public async Task<bool> IsTeamNameUniqueInClub(string name, Guid clubId, Guid ageCategoryId, Guid? excludeId = null)
        {
            var query = _context.Teams.Where(x => x.Name == name && x.ClubId == clubId && x.AgeCategoryId == ageCategoryId);

            if (excludeId.HasValue)
            {
                query = query.Where(x => x.Id != excludeId.Value);
            }

            return await query.AnyAsync() == false;
        }
    }
}
