using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;

public class TeamService : BaseHttpService, ITeamService
{
    public TeamService(IClient client, ILocalStorageService localStorageService)
        : base(client, localStorageService)
    {
    }

    public async Task<PaginatedResponse<TeamDTO>> GetTeams(TeamPaginationRequest request)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetAllTeamsAsync(
                ageCategoryId: request.AgeCategoryId,
                gender: request.Gender,
                isActive: request.IsActive,
                page: request.Page,
                pageSize: request.PageSize,
                sortBy: request.SortBy,
                sortDirection: request.SortDirection,
                searchTerm: request.SearchTerm,
                searchableProperties: request.SearchableProperties,
                sortableProperties: request.SortableProperties
            );
            return new PaginatedResponse<TeamDTO>
            {
                Data = response.Data?.ToList() ?? new List<TeamDTO>(),
                Pagination = response.Pagination ?? new PaginationMetadata(),
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToPaginated<TeamDTO>(ex);
        }
    }

    public async Task<QueryResponse<TeamDetailsDTO>> GetTeamDetails(Guid id)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetTeamByIdAsync(id);
            return new QueryResponse<TeamDetailsDTO>
            {
                Data = response.Data,
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<TeamDetailsDTO>(ex);
        }
    }

    public async Task<CommandResponse<Guid>> CreateTeam(CreateTeamCommand team)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.CreateTeamAsync(team);
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

    public async Task<CommandResponse<bool>> UpdateTeam(Guid id, UpdateTeamCommand team)
    {
        try
        {
            await AddBearerToken();
            team.Id = id;
            await _client.UpdateTeamAsync(id, team);
            return CommandResponse<bool>.Success(true, "Team updated successfully");
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

    public async Task<CommandResponse<bool>> DeactivateTeam(Guid id)
    {
        try
        {
            await AddBearerToken();
            await _client.DeactivateTeamAsync(id);
            return CommandResponse<bool>.Success(true, "Team deactivated successfully");
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

    public async Task<CommandResponse<bool>> ActivateTeam(Guid id)
    {
        try
        {
            await AddBearerToken();
            await _client.ActivateTeamAsync(id);
            return CommandResponse<bool>.Success(true, "Team activated successfully");
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

    public async Task<CommandResponse<bool>> DeleteTeam(Guid id)
    {
        try
        {
            await AddBearerToken();
            await _client.DeleteTeamAsync(id);
            return CommandResponse<bool>.Success(true, "Team deleted successfully");
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

    public async Task<QueryResponse<List<TeamRosterPlayerDTO>>> GetTeamRoster(Guid teamId)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetTeamRosterAsync(teamId);
            return new QueryResponse<List<TeamRosterPlayerDTO>>
            {
                Data = response.Data?.ToList() ?? new List<TeamRosterPlayerDTO>(),
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<List<TeamRosterPlayerDTO>>(ex);
        }
    }

    public async Task<QueryResponse<List<TeamLeagueDTO>>> GetTeamLeagues(Guid teamId)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetTeamLeaguesAsync(teamId);
            return new QueryResponse<List<TeamLeagueDTO>>
            {
                Data = response.Data?.ToList() ?? new List<TeamLeagueDTO>(),
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<List<TeamLeagueDTO>>(ex);
        }
    }

    public async Task<CommandResponse<Guid>> AssignPlayerToTeam(Guid teamId, AssignPlayerToTeamCommand command)
    {
        try
        {
            await AddBearerToken();
            command.TeamId = teamId;
            var response = await _client.AssignPlayerToTeamAsync(teamId, command);
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

    public async Task<CommandResponse<bool>> UpdatePlayerAssignmentInTeam(Guid teamId, Guid playerId, UpdatePlayerAssignmentInTeamCommand command)
    {
        try
        {
            await AddBearerToken();
            command.TeamId = teamId;
            command.PlayerId = playerId;
            await _client.UpdatePlayerAssignmentInTeamAsync(teamId, playerId, command);
            return CommandResponse<bool>.Success(true, "Player assignment updated successfully");
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

    public async Task<CommandResponse<bool>> RemovePlayerFromTeam(Guid teamId, Guid playerId)
    {
        try
        {
            await AddBearerToken();
            await _client.RemovePlayerFromTeamAsync(teamId, playerId);
            return CommandResponse<bool>.Success(true, "Player removed from team successfully");
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

    public async Task<QueryResponse<List<TeamMatchHistoryItemDTO>>> GetTeamMatchHistory(Guid teamId)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetTeamMatchHistoryAsync(teamId);
            return new QueryResponse<List<TeamMatchHistoryItemDTO>>
            {
                Data = response.Data?.ToList() ?? new List<TeamMatchHistoryItemDTO>(),
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<List<TeamMatchHistoryItemDTO>>(ex);
        }
    }

    public async Task<QueryResponse<List<TeamRepresentativeDTO>>> GetTeamRepresentatives(Guid teamId)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetTeamRepresentativesAsync(teamId);
            return new QueryResponse<List<TeamRepresentativeDTO>>
            {
                Data = response.Data?.ToList() ?? new List<TeamRepresentativeDTO>(),
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<List<TeamRepresentativeDTO>>(ex);
        }
    }

    public async Task<CommandResponse<Guid>> AssignTeamRepresentative(Guid teamId, AssignTeamRepresentativeCommand command)
    {
        try
        {
            await AddBearerToken();
            command.TeamId = teamId;
            var response = await _client.AssignTeamRepresentativeAsync(teamId, command);
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

    public async Task<CommandResponse<bool>> RevokeTeamRepresentative(Guid teamId, Guid repId)
    {
        try
        {
            await AddBearerToken();
            await _client.RevokeTeamRepresentativeAsync(teamId, repId);
            return CommandResponse<bool>.Success(true, "Team representative revoked.");
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

    public async Task<QueryResponse<List<TeamRepMembershipDTO>>> GetMyRepresentativeships()
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetMyRepresentativeshipsAsync();
            return new QueryResponse<List<TeamRepMembershipDTO>>
            {
                Data = response.Data?.ToList() ?? new List<TeamRepMembershipDTO>(),
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<List<TeamRepMembershipDTO>>(ex);
        }
    }

    public async Task<QueryResponse<List<TeamPlayerStatDTO>>> GetTeamLeaguePlayerStats(Guid teamId, Guid leagueId)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetTeamLeaguePlayerStatsAsync(teamId, leagueId);
            return new QueryResponse<List<TeamPlayerStatDTO>>
            {
                Data = response.Data?.ToList() ?? new List<TeamPlayerStatDTO>(),
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<List<TeamPlayerStatDTO>>(ex);
        }
    }

    public async Task<QueryResponse<TeamLeagueStandingDTO?>> GetTeamLeagueStanding(Guid teamId, Guid leagueId)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetTeamLeagueStandingAsync(teamId, leagueId);
            return new QueryResponse<TeamLeagueStandingDTO?>
            {
                Data = response.Data,
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<TeamLeagueStandingDTO?>(ex);
        }
    }

    public async Task<QueryResponse<bool>> GetFavoriteStatus(Guid teamId)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetFavoriteStatusAsync(teamId);
            return new QueryResponse<bool>
            {
                Data = response.Data,
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<bool>(ex);
        }
    }

    public async Task<CommandResponse<bool>> ToggleFavoriteTeam(Guid teamId)
    {
        try
        {
            await AddBearerToken();
            var response = await _client.ToggleFavoriteTeamAsync(teamId);
            return new CommandResponse<bool>
            {
                Data = response.Data,
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
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

    public async Task<QueryResponse<List<FavoriteTeamDTO>>> GetMyFavoriteTeams()
    {
        try
        {
            await AddBearerToken();
            var response = await _client.GetMyFavoriteTeamsAsync();
            return new QueryResponse<List<FavoriteTeamDTO>>
            {
                Data = response.Data?.ToList() ?? new List<FavoriteTeamDTO>(),
                IsSuccess = response.IsSuccess,
                Message = response.Message,
                Errors = response.Errors?.ToList() ?? new List<string>()
            };
        }
        catch (ApiException ex)
        {
            return ConvertApiExceptionsToQuery<List<FavoriteTeamDTO>>(ex);
        }
    }
}
