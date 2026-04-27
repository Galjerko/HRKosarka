using HRKošarka.Application.Features.Player.Queries.GetAvailablePlayers;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IPlayerRepository : IGenericRepository<Player>
    {
        Task<bool> IsRegistrationNumberUnique(string registrationNumber, Guid? excludeId = null, CancellationToken cancellationToken = default);
        Task<bool> IsAlreadyActiveInTeamAsync(Guid playerId, Guid teamId, CancellationToken cancellationToken = default);
        Task<bool> HasAgeCategoryConflictAsync(Guid playerId, Guid ageCategoryId, CancellationToken cancellationToken = default);
        Task<List<AvailablePlayerDTO>> GetAvailablePlayersAsync(Guid teamId, string? searchTerm, CancellationToken cancellationToken = default);
    }
}
