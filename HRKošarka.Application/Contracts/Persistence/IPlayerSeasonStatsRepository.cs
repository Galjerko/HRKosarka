using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IPlayerSeasonStatsRepository : IGenericRepository<PlayerSeasonStats>
    {
        Task<PlayerSeasonStats?> GetByPlayerAndLeagueAsync(Guid playerId, Guid leagueId, Guid seasonId, CancellationToken ct = default);
        Task<List<PlayerSeasonStats>> GetAllByPlayerAsync(Guid playerId, CancellationToken ct = default);
    }
}
