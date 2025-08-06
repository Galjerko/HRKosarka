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
}
