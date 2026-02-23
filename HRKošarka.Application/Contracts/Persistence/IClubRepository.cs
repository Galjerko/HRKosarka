using HRKošarka.Application.Features.Club.Queries.GetClubsWIthoutManager;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IClubRepository : IGenericRepository<Club>
    {
        Task<bool> IsClubNameUnique(string name, Guid? excludeId = null);
        Task<Club?> GetClubWithTeamsAsync(Guid clubId);

        Task<List<ClubWithoutManagerDTO>> GetClubsWithoutManagerAsync(
                List<Guid> managedClubIds,
                string? searchTerm,
                CancellationToken cancellationToken = default);
    }
}
