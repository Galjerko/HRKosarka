using HRKošarka.Domain;
using DomainMatch = HRKošarka.Domain.Match;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IPlayoffRepository : IGenericRepository<PlayoffSeries>
    {
        Task<PlayoffSeries?> GetSeriesWithMatchesAsync(Guid seriesId, CancellationToken ct = default);
        Task<List<PlayoffSeries>> GetSeriesByRoundAsync(Guid leagueId, int roundNumber, CancellationToken ct = default);
        Task<List<PlayoffSeries>> GetAllSeriesForLeagueAsync(Guid leagueId, CancellationToken ct = default);
        Task<bool> HasIncompleteSeriesAsync(Guid leagueId, CancellationToken ct = default);
        Task CreateInitialBracketAsync(List<PlayoffSeries> allSeries, League league, CancellationToken ct = default);
        Task UpdateSeriesAndActivateNextAsync(
            PlayoffSeries updatedSeries,
            List<PlayoffSeries> nextRoundStubsToActivate,
            List<DomainMatch> matchesToCreate,
            CancellationToken ct = default);
    }
}
