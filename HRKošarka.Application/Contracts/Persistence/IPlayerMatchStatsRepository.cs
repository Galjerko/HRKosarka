using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IPlayerMatchStatsRepository : IGenericRepository<PlayerMatchStats>
    {
        Task<PlayerMatchStats?> GetByMatchAndPlayerAsync(Guid matchId, Guid playerId, CancellationToken ct = default);
        Task<List<PlayerMatchStats>> GetPlayedStatsForMatchAsync(Guid matchId, CancellationToken ct = default);
        Task DeleteAllForMatchAsync(Guid matchId, CancellationToken ct = default);
        Task<int> CountByMatchAndTeamAsync(Guid matchId, Guid teamId, CancellationToken ct = default);
        Task<List<PlayerMatchStats>> GetAllByPlayerWithMatchAsync(Guid playerId, CancellationToken ct = default);
        Task<List<PlayerMatchStats>> GetAllByPlayerPlayoffWithMatchAsync(Guid playerId, CancellationToken ct = default);
    }
}
