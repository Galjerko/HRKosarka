using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Services
{
    public class PlayerService : BaseHttpService, IPlayerService
    {
        public PlayerService(IClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService)
        {
        }

        public async Task<PaginatedResponse<PlayerDTO>> GetPlayers(PaginationRequest request)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetAllPlayersAsync(
                    request.Page,
                    request.PageSize,
                    request.SortBy,
                    request.SortDirection,
                    request.SearchTerm,
                    null,
                    null
                );

                return new PaginatedResponse<PlayerDTO>
                {
                    Data = response.Data?.ToList() ?? new List<PlayerDTO>(),
                    Pagination = response.Pagination ?? new PaginationMetadata(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToPaginated<PlayerDTO>(ex);
            }
        }

        public async Task<QueryResponse<PlayerDetailsDTO>> GetPlayerDetails(Guid id)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetPlayerByIdAsync(id);

                return new QueryResponse<PlayerDetailsDTO>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<PlayerDetailsDTO>(ex);
            }
        }

        public async Task<QueryResponse<List<AvailablePlayerDTO>>> GetAvailablePlayers(Guid teamId, string? searchTerm)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetAvailablePlayersAsync(teamId, searchTerm ?? string.Empty);
                return new QueryResponse<List<AvailablePlayerDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<AvailablePlayerDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<AvailablePlayerDTO>>(ex);
            }
        }

        public async Task<QueryResponse<List<AvailableTeamDTO>>> GetAvailableTeamsForPlayer(Guid playerId, string? searchTerm)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetAvailableTeamsForPlayerAsync(playerId, searchTerm ?? string.Empty);
                return new QueryResponse<List<AvailableTeamDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<AvailableTeamDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<AvailableTeamDTO>>(ex);
            }
        }

        public async Task<QueryResponse<List<PlayerAssignmentDTO>>> GetPlayerAssignments(Guid playerId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetPlayerAssignmentsAsync(playerId);
                return new QueryResponse<List<PlayerAssignmentDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<PlayerAssignmentDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<PlayerAssignmentDTO>>(ex);
            }
        }

        public async Task<CommandResponse<Guid>> CreatePlayer(CreatePlayerCommand player)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.CreatePlayerAsync(player);

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

        public async Task<CommandResponse<bool>> UpdatePlayer(Guid id, UpdatePlayerCommand player)
        {
            try
            {
                await AddBearerToken();
                player.Id = id;
                await _client.UpdatePlayerAsync(id, player);

                return CommandResponse<bool>.Success(true, "Player updated successfully");
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

        public async Task<CommandResponse<bool>> DeactivatePlayer(Guid id)
        {
            try
            {
                await AddBearerToken();
                await _client.DeactivatePlayerAsync(id);

                return CommandResponse<bool>.Success(true, "Player deactivated successfully");
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

        public async Task<CommandResponse<bool>> ActivatePlayer(Guid id)
        {
            try
            {
                await AddBearerToken();
                await _client.ActivatePlayerAsync(id);

                return CommandResponse<bool>.Success(true, "Player activated successfully");
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

        public async Task<CommandResponse<bool>> DeletePlayer(Guid id)
        {
            try
            {
                await AddBearerToken();
                await _client.DeletePlayerAsync(id);

                return CommandResponse<bool>.Success(true, "Player deleted successfully");
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
