using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IPlayerTeamHistoryRepository : IGenericRepository<PlayerTeamHistory>
    {
        Task<PlayerTeamHistory?> GetActiveByPlayerAndTeamAsync(Guid playerId, Guid teamId, CancellationToken cancellationToken = default);
        Task<List<PlayerTeamHistory>> GetActiveByPlayerAsync(Guid playerId, CancellationToken cancellationToken = default);
        Task<List<PlayerTeamHistory>> GetAllByPlayerAsync(Guid playerId, CancellationToken cancellationToken = default);
        Task<bool> IsJerseyNumberAvailableAsync(
            Guid teamId,
            int jerseyNumber,
            Guid? excludeHistoryId = null,
            CancellationToken cancellationToken = default);
        Task<bool> HasActiveAssignmentsForPlayerAsync(Guid playerId, CancellationToken cancellationToken = default);
        Task<bool> HasActiveAssignmentsForTeamAsync(Guid teamId, CancellationToken cancellationToken = default);
        Task DeactivateAllForPlayerAsync(Guid playerId, CancellationToken cancellationToken = default);
        Task DeactivateAllForTeamAsync(Guid teamId, CancellationToken cancellationToken = default);
        Task<List<PlayerTeamHistory>> GetRosterAsync(Guid teamId, Guid seasonId, CancellationToken cancellationToken = default);
    }
}
