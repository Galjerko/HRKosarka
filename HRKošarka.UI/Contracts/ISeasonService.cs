using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface ISeasonService
    {
        Task<PaginatedResponse<SeasonDTO>> GetSeasons(PaginationRequest request);
        Task<QueryResponse<SeasonDetailsDTO>> GetSeasonById(Guid id);
        Task<CommandResponse<Guid>> CreateSeason(CreateSeasonCommand command);
        Task<CommandResponse<bool>> UpdateSeason(Guid id, UpdateSeasonCommand command);
        Task<CommandResponse<bool>> DeleteSeason(Guid id);
    }
}
