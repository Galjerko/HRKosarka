using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Services
{
    public class SeasonService : BaseHttpService, ISeasonService
    {
        private readonly IClient _client;

        public SeasonService(IClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService)
        {
            _client = client;
        }

        public async Task<PaginatedResponse<SeasonDTO>> GetSeasons(PaginationRequest request)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetAllSeasonsAsync(
                    request.Page,
                    request.PageSize,
                    request.SortBy,
                    request.SortDirection,
                    request.SearchTerm,
                    request.SearchableProperties,
                    request.SortableProperties);

                return new PaginatedResponse<SeasonDTO>
                {
                    Data = response.Data?.ToList() ?? new List<SeasonDTO>(),
                    Pagination = response.Pagination ?? new PaginationMetadata(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToPaginated<SeasonDTO>(ex);
            }
        }

        public async Task<QueryResponse<SeasonDetailsDTO>> GetSeasonById(Guid id)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetSeasonByIdAsync(id);
                return new QueryResponse<SeasonDetailsDTO>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<SeasonDetailsDTO>(ex);
            }
        }

        public async Task<CommandResponse<Guid>> CreateSeason(CreateSeasonCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.CreateSeasonAsync(command);
                return new CommandResponse<Guid>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex)
            {
                return ConvertApiExceptions<Guid>(ex);
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<Guid>(ex);
            }
        }

        public async Task<CommandResponse<bool>> UpdateSeason(Guid id, UpdateSeasonCommand command)
        {
            try
            {
                await AddBearerToken();
                command.Id = id;
                await _client.UpdateSeasonAsync(id, command);
                return CommandResponse<bool>.Success(true, "Season updated successfully");
            }
            catch (ApiException<CustomProblemDetails> ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
        }

        public async Task<CommandResponse<bool>> DeleteSeason(Guid id)
        {
            try
            {
                await AddBearerToken();
                await _client.DeleteSeasonAsync(id);
                return CommandResponse<bool>.Success(true, "Season deleted successfully");
            }
            catch (ApiException<CustomProblemDetails> ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
        }
    }
}
