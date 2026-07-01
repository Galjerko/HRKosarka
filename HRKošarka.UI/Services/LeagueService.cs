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

        public async Task<QueryResponse<List<LeagueTeamDTO>>> GetLeagueTeams(Guid leagueId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetLeagueTeamsAsync(leagueId);
                return new QueryResponse<List<LeagueTeamDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<LeagueTeamDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<LeagueTeamDTO>>(ex);
            }
        }

        public async Task<QueryResponse<List<AvailableTeamForLeagueDTO>>> GetAvailableTeamsForLeague(Guid leagueId, string? searchTerm = null)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetAvailableTeamsForLeagueAsync(leagueId, searchTerm);
                return new QueryResponse<List<AvailableTeamForLeagueDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<AvailableTeamForLeagueDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<AvailableTeamForLeagueDTO>>(ex);
            }
        }

        public async Task<CommandResponse<Guid>> RegisterTeamInLeague(Guid leagueId, RegisterTeamInLeagueCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.RegisterTeamInLeagueAsync(leagueId, command);
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

        public async Task<CommandResponse<bool>> RemoveTeamFromLeague(Guid leagueId, Guid teamId)
        {
            try
            {
                await AddBearerToken();
                await _client.RemoveTeamFromLeagueAsync(leagueId, teamId);
                return CommandResponse<bool>.Success(true, "Team removed from league.");
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

        public async Task<QueryResponse<List<LeagueBreakDTO>>> GetLeagueBreaks(Guid leagueId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetLeagueBreaksAsync(leagueId);
                return new QueryResponse<List<LeagueBreakDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<LeagueBreakDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<LeagueBreakDTO>>(ex);
            }
        }

        public async Task<CommandResponse<Guid>> AddLeagueBreak(Guid leagueId, AddLeagueBreakCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.AddLeagueBreakAsync(leagueId, command);
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

        public async Task<CommandResponse<bool>> RemoveLeagueBreak(Guid leagueId, Guid breakId)
        {
            try
            {
                await AddBearerToken();
                await _client.RemoveLeagueBreakAsync(leagueId, breakId);
                return CommandResponse<bool>.Success(true, "Break removed.");
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

        public async Task<CommandResponse<int>> GenerateLeagueSchedule(Guid leagueId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GenerateLeagueScheduleAsync(leagueId);
                return new CommandResponse<int>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex)
            {
                return ConvertApiExceptions<int>(ex);
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptions<int>(ex);
            }
        }

        public async Task<QueryResponse<List<LeagueRoundDTO>>> GetLeagueSchedule(Guid leagueId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetLeagueScheduleAsync(leagueId);
                return new QueryResponse<List<LeagueRoundDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<LeagueRoundDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<LeagueRoundDTO>>(ex);
            }
        }

        public async Task<QueryResponse<List<FeaturedLeagueRoundDTO>>> GetFeaturedLeagueMatches()
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetFeaturedLeagueMatchesAsync();
                return new QueryResponse<List<FeaturedLeagueRoundDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<FeaturedLeagueRoundDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<FeaturedLeagueRoundDTO>>(ex);
            }
        }

        public async Task<QueryResponse<LeagueStandingsDTO>> GetLeagueStandings(Guid leagueId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetLeagueStandingsAsync(leagueId);
                return new QueryResponse<LeagueStandingsDTO>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<LeagueStandingsDTO>(ex);
            }
        }

        public async Task<QueryResponse<List<LeaguePlayerStatDTO>>> GetLeagueLeaderboard(
            Guid leagueId, string? sortBy = null, string? sortDirection = null)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetLeagueLeaderboardAsync(leagueId, sortBy, sortDirection);
                return new QueryResponse<List<LeaguePlayerStatDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<LeaguePlayerStatDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<LeaguePlayerStatDTO>>(ex);
            }
        }

        public async Task<QueryResponse<PlayoffBracketDTO>> GetPlayoffBracket(Guid leagueId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetPlayoffBracketAsync(leagueId);
                return new QueryResponse<PlayoffBracketDTO>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<PlayoffBracketDTO>(ex);
            }
        }

        public async Task<QueryResponse<LeagueLeadersDTO>> GetPlayoffLeaders(Guid leagueId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetPlayoffLeadersAsync(leagueId);
                return new QueryResponse<LeagueLeadersDTO>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<LeagueLeadersDTO>(ex);
            }
        }

        public async Task<QueryResponse<List<LeaguePlayerStatDTO>>> GetPlayoffLeaderboard(
            Guid leagueId, string? sortBy = null, string? sortDirection = null)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetPlayoffLeaderboardAsync(leagueId, sortBy, sortDirection);
                return new QueryResponse<List<LeaguePlayerStatDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<LeaguePlayerStatDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex)
            {
                return ConvertApiExceptionsToQuery<List<LeaguePlayerStatDTO>>(ex);
            }
        }

        public async Task<CommandResponse<bool>> GeneratePlayoff(Guid leagueId, GeneratePlayoffCommand command)
        {
            try
            {
                await AddBearerToken();
                command.LeagueId = leagueId;
                var response = await _client.GeneratePlayoffAsync(leagueId, command);
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
    }
}
