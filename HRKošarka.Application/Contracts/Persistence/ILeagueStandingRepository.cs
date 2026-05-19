using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface ILeagueStandingRepository : IGenericRepository<LeagueStanding>
    {
        Task<LeagueStanding?> GetByTeamAndLeagueAsync(Guid teamId, Guid leagueId, Guid seasonId, CancellationToken ct = default);
        Task<List<LeagueStanding>> GetByLeagueAsync(Guid leagueId, CancellationToken ct = default);
    }
}
