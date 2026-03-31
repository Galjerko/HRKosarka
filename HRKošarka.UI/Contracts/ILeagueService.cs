using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface ILeagueService
    {
        Task<PaginatedResponse<LeagueDTO>> GetLeagues(
            PaginationRequest request,
            Guid? seasonId = null,
            Guid? ageCategoryId = null,
            Gender? gender = null,
            CompetitionType? competitionType = null,
            bool? isActive = null);

        Task<QueryResponse<LeagueDetailsDTO>> GetLeagueById(Guid id);
        Task<CommandResponse<Guid>> CreateLeague(CreateLeagueCommand command);
        Task<CommandResponse<bool>> UpdateLeague(Guid id, UpdateLeagueCommand command);
        Task<CommandResponse<bool>> DeactivateLeague(Guid id);
        Task<CommandResponse<bool>> ActivateLeague(Guid id);
        Task<CommandResponse<bool>> DeleteLeague(Guid id);
    }
}
