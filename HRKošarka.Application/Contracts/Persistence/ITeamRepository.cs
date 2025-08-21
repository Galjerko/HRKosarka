using HRKošarka.Application.Features.Team.Queries.GetAllTeams;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;

namespace HRKošarka.Application.Contracts.Persistence
{
    public interface ITeamRepository : IGenericRepository<Team>
    {
        Task<bool> IsTeamNameUniqueInClub(string name, Guid clubId, Guid ageCategoryId, Guid? excludeId = null);
        Task<PaginatedResponse<Team>> GetPagedWithIncludesAsync(GetTeamsQuery request);
        Task<Team?> GetByIdWithIncludesAsync(Guid teamId);

    }
}
