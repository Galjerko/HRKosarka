using HRKošarka.Application.Features.Club.Queries.GetClubsWIthoutManager;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IClubRepository : IGenericRepository<Club>
    {
        Task<bool> IsClubNameUnique(string name, Guid? excludeId = null, CancellationToken cancellationToken = default);
        Task<Club?> GetClubWithTeamsAsync(Guid clubId, CancellationToken cancellationToken = default);

        Task<List<ClubWithoutManagerDTO>> GetClubsWithoutManagerAsync(
                List<Guid> managedClubIds,
                string? searchTerm,
                CancellationToken cancellationToken = default);
    }
}
