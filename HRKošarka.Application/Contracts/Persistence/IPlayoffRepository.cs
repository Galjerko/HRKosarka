using HRKošarka.Application.Features.League.Queries.GetPlayoffBracket;
using HRKošarka.Domain;
using DomainMatch = HRKošarka.Domain.Match;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface IPlayoffRepository : IGenericRepository<PlayoffSeries>
    {
        Task<PlayoffSeries?> GetSeriesWithMatchesAsync(Guid seriesId, CancellationToken ct = default);
        Task<PlayoffBracketDTO> GetPlayoffBracketAsync(Guid leagueId, CancellationToken ct = default);
        Task<List<PlayoffSeries>> GetUpcomingSeriesPopulatedByThisSeriesAsync(Guid completedSeriesId, CancellationToken ct = default);
        Task CreateInitialBracketAsync(List<PlayoffSeries> allSeries, League league, CancellationToken ct = default);
        Task UpdateSeriesAndActivateNextAsync(
            PlayoffSeries updatedSeries,
            List<PlayoffSeries> nextRoundStubsToActivate,
            List<DomainMatch> matchesToCreate,
            CancellationToken ct = default);
    }
}
