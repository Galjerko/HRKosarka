using HRKošarka.Application.Features.Team.Queries.GetAllTeams;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using System.Threading;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        Task<bool> IsTeamNameUniqueInClub(string name, Guid clubId, Guid ageCategoryId, Guid? excludeId = null, CancellationToken cancellationToken = default);
        Task<PaginatedResponse<Team>> GetPagedWithIncludesAsync(GetTeamsQuery request, CancellationToken cancellationToken = default);
        Task<Team?> GetByIdWithIncludesAsync(Guid teamId, CancellationToken cancellationToken = default);

    }
}
