using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface IAgeCategoryService
    {
        Task<QueryResponse<List<AgeCategoryDTO>>> GetAllAgeCategories();
    }
}
