using HRKošarka.Application.Features.League.Queries.GetLeagueStandings;
using HRKošarka.Application.Features.Team.Queries.GetTeamLeaguePlayerStats;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IPlayerSeasonStatsRepository : IGenericRepository<PlayerSeasonStats>
    {
        Task<PlayerSeasonStats?> GetByPlayerAndLeagueAsync(Guid playerId, Guid leagueId, Guid seasonId, CancellationToken ct = default);
        Task<List<PlayerSeasonStats>> GetAllByPlayerAsync(Guid playerId, CancellationToken ct = default);
        Task<List<TeamPlayerStatDTO>> GetByTeamAndLeagueAsync(Guid teamId, Guid leagueId, CancellationToken ct = default);
        Task<LeagueLeadersDTO> GetLeagueLeadersAsync(Guid leagueId, CancellationToken ct = default);
    }
}
