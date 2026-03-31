using HRKošarka.Application.Features.League.Queries.GetAllLeagues;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface ILeagueRepository : IGenericRepository<League>
    {
        Task<League?> GetByIdWithIncludesAsync(Guid id, CancellationToken cancellationToken = default);
        Task<League?> GetLeagueWithDetailsAsync(Guid id, CancellationToken cancellationToken = default);
        Task<PaginatedResponse<League>> GetPagedWithIncludesAsync(GetLeaguesQuery request, CancellationToken cancellationToken = default);
    }
}
