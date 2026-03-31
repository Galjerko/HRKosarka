using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Services
{
    public class LeagueService : BaseHttpService, ILeagueService
    {
        public LeagueService(IClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService)
        {
        }

        public async Task<PaginatedResponse<LeagueDTO>> GetLeagues(
            PaginationRequest request,
            Guid? seasonId = null,
            Guid? ageCategoryId = null,
            Gender? gender = null,
            CompetitionType? competitionType = null,
            bool? isActive = null)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetAllLeaguesAsync(
                    seasonId,
                    ageCategoryId,
                    gender,
                    competitionType,
                    isActive,
                    request.Page,
                    request.PageSize,
                    request.SortBy,
                    request.SortDirection,
                    request.SearchTerm,
                    null,
                    null);

                return new PaginatedResponse<LeagueDTO>
                {
                    Data = response.Data?.ToList() ?? new List<LeagueDTO>(),
                    Pagination = response.Pagination ?? new PaginationMetadata(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToPaginated<LeagueDTO>(ex);
            }
        }

        public async Task<QueryResponse<LeagueDetailsDTO>> GetLeagueById(Guid id)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetLeagueByIdAsync(id);

                return new QueryResponse<LeagueDetailsDTO>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<LeagueDetailsDTO>(ex);
            }
        }

        public async Task<CommandResponse<Guid>> CreateLeague(CreateLeagueCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.CreateLeagueAsync(command);

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

        public async Task<CommandResponse<bool>> UpdateLeague(Guid id, UpdateLeagueCommand command)
        {
            try
            {
                await AddBearerToken();
                command.Id = id;
                await _client.UpdateLeagueAsync(id, command);
                return CommandResponse<bool>.Success(true, "League updated successfully.");
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

        public async Task<CommandResponse<bool>> DeactivateLeague(Guid id)
        {
            try
            {
                await AddBearerToken();
                await _client.DeactivateLeagueAsync(id);
                return CommandResponse<bool>.Success(true, "League deactivated successfully.");
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

        public async Task<CommandResponse<bool>> ActivateLeague(Guid id)
        {
            try
            {
                await AddBearerToken();
                await _client.ActivateLeagueAsync(id);
                return CommandResponse<bool>.Success(true, "League activated successfully.");
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

        public async Task<CommandResponse<bool>> DeleteLeague(Guid id)
        {
            try
            {
                await AddBearerToken();
                await _client.DeleteLeagueAsync(id);
                return CommandResponse<bool>.Success(true, "League deleted successfully.");
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
