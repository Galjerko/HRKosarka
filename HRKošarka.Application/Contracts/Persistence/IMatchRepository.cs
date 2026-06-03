using HRKošarka.Application.Features.League.Queries.GetLeagueStandings;
using HRKošarka.Application.Features.Match.Queries.GetPendingActions;
using HRKošarka.Application.Features.Team.Queries.GetTeamMatchHistory;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IMatchRepository : IGenericRepository<Match>
    {
        Task<Match?> GetByIdWithIncludesAsync(Guid id, CancellationToken ct = default);
        Task<Match?> GetMatchWithFullDetailsAsync(Guid id, CancellationToken ct = default);
        Task<List<TeamMatchHistoryItemDTO>> GetTeamMatchHistoryAsync(Guid teamId, CancellationToken ct = default);
        Task<List<PendingActionDTO>> GetPendingActionsAsync(Guid? clubId, bool isAdmin, string? teamRepUserId = null, CancellationToken ct = default);
        Task<List<CompletedMatchSlimDTO>> GetCompletedMatchesByLeagueAsync(Guid leagueId, CancellationToken ct = default);
        Task<List<Match>> GetRoundMatchesAsync(Guid leagueId, int round, CancellationToken ct = default);
    }
}
