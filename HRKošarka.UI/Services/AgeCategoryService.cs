using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Services
{
    public class AgeCategoryService : BaseHttpService, IAgeCategoryService
    {
        public AgeCategoryService(IClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService)
        {
        }

        public async Task<QueryResponse<List<AgeCategoryDTO>>> GetAllAgeCategories()
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetAllAgeCategoriesAsync();

                return new QueryResponse<List<AgeCategoryDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<AgeCategoryDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<AgeCategoryDTO>>(ex);
            }
        }
    }
}
