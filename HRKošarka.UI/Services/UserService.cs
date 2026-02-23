using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Services
{
    public class UserService : BaseHttpService, IUserService
    {
        public UserService(IClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService)
        {
        }

        public async Task<PaginatedResponse<NonAdminUserDTO>> GetUsers(PaginationRequest request)
        {
            try
            {
                await AddBearerToken();

                var response = await _client.GetAllUsersAsync(
                    request.Page,
                    request.PageSize,
                    request.SortBy,
                    request.SortDirection,
                    request.SearchTerm,
                    null,
                    null
                );

                return new PaginatedResponse<NonAdminUserDTO>
                {
                    Data = response.Data?.ToList() ?? new List<NonAdminUserDTO>(),
                    Pagination = response.Pagination ?? new PaginationMetadata(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToPaginated<NonAdminUserDTO>(ex);
            }
        }

        public async Task<SimpleResponse> LockUser(string userId)
        {
            try
            {
                await AddBearerToken();

                await _client.LockUserAsync(userId, CancellationToken.None);

                return new SimpleResponse
                {
                    IsSuccess = true,
                    Message = "User locked successfully."
                };
            }
            catch (ApiException<ProblemDetails> ex)
            {
                return ConvertApiExceptionsToSimple(ex);
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToSimple(ex);
            }
        }


        public async Task<CommandResponse<bool>> RemoveClubManager(string userId)
        {
            try
            {
                await AddBearerToken();

                await _client.RemoveClubManagerAsync(userId, CancellationToken.None);

                return CommandResponse<bool>.Success(true, "Club manager removed successfully.");
            }
            catch (ApiException<ProblemDetails> ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
        }

        public async Task<CommandResponse<bool>> AssignClubManager(AssignClubManagerCommand command)
        {
            try
            {
                await AddBearerToken();

                await _client.AssignClubManagerAsync(command);

                return CommandResponse<bool>.Success(true, "Club manager assigned successfully.");
            }
            catch (ApiException<ProblemDetails> ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
        }

        public async Task<SimpleResponse> UnlockUser(string userId)
        {
            try
            {
                await AddBearerToken();

                await _client.UnlockUserAsync(userId, CancellationToken.None);

                return new SimpleResponse
                {
                    IsSuccess = true,
                    Message = "User unlocked successfully."
                };
            }
            catch (ApiException<ProblemDetails> ex)
            {
                return ConvertApiExceptionsToSimple(ex);
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToSimple(ex);
            }
        }

        public async Task<PaginatedResponse<InactiveUserDTO>> GetInactiveUsers(PaginationRequest request)
        {
            try
            {
                await AddBearerToken();

                var response = await _client.GetInactiveUsersAsync(
                    request.Page,
                    request.PageSize,
                    request.SortBy,
                    request.SortDirection,
                    request.SearchTerm,
                    null,
                    null
                );

                return new PaginatedResponse<InactiveUserDTO>
                {
                    Data = response.Data?.ToList() ?? new List<InactiveUserDTO>(),
                    Pagination = response.Pagination ?? new PaginationMetadata(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToPaginated<InactiveUserDTO>(ex);
            }
        }

    }
}