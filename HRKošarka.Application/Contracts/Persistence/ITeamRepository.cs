using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        Task<bool> IsTeamNameUniqueInClub(string name, Guid clubId, Guid ageCategoryId, Guid? excludeId = null);
    }
}
