using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IClubRepository : IGenericRepository<Club>
    {
        Task<bool> IsClubNameUnique(string name, Guid? excludeId = null);
    }
}
