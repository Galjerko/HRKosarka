using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Services
{
    public class ClubService : BaseHttpService, IClubService
    {
        public ClubService(IClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService)
        {
        }

        public async Task<PaginatedResponse<ClubDTO>> GetClubs(PaginationRequest request)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetAllClubsAsync(
                    request.Page,
                    request.PageSize,
                    request.SortBy,
                    request.SortDirection,
                    request.SearchTerm,
                    null,
                    null
                );

                return new PaginatedResponse<ClubDTO>
                {
                    Data = response.Data.ToList(),
                    Pagination = response.Pagination,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors == null ? new List<string>() : new List<string>(response.Errors)
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToPaginated<ClubDTO>(ex);
            }
        }

        public async Task<QueryResponse<ClubDetailsDTO>> GetClubDetails(Guid id)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetClubByIdAsync(id);

                return new QueryResponse<ClubDetailsDTO>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors == null ? new List<string>() : new List<string>(response.Errors)
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<ClubDetailsDTO>(ex);
            }
        }

        public async Task<CommandResponse<Guid>> CreateClub(CreateClubCommand club)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.CreateClubAsync(club);

                return new CommandResponse<Guid>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors == null ? new List<string>() : new List<string>(response.Errors)
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<Guid>(ex);
            }
        }

        public async Task<CommandResponse<bool>> UpdateClub(Guid id, UpdateClubCommand club)
        {
            try
            {
                await AddBearerToken();
                club.Id = id;
                await _client.UpdateClubAsync(id, club);

                return CommandResponse<bool>.Success(true, "Club updated successfully");
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
        }

        public async Task<CommandResponse<bool>> DeactivateClub(Guid id)
        {
            try
            {
                await AddBearerToken();
                await _client.DeactivateClubAsync(id);

                return CommandResponse<bool>.Success(true, "Club deactivated successfully");
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
        }

        public async Task<CommandResponse<bool>> DeleteClub(Guid id)
        {
            try
            {
                await AddBearerToken();
                await _client.DeleteClubAsync(id);

                return CommandResponse<bool>.Success(true, "Club deleted successfully");
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<bool>(ex);
            }
        }
    }
}
