using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain;
using HRKošarka.Persistence.DatabaseContext;

namespace HRKošarka.Persistence.Repositories
{
    public class AgeCategoryRepository : GenericRepository<AgeCategory>, IAgeCategoryRepository
    {
        public AgeCategoryRepository(HRDatabaseContext context) : base(context)
        {
        }
    }
}
