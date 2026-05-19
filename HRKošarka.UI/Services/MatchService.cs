using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Services
{
    public class MatchService : BaseHttpService, IMatchService
    {
        public MatchService(IClient client, ILocalStorageService localStorageService)
            : base(client, localStorageService) { }

        public async Task<QueryResponse<List<PendingActionDTO>>> GetPendingActions()
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetPendingActionsAsync();
                return new QueryResponse<List<PendingActionDTO>>
                {
                    Data = response.Data?.ToList() ?? new List<PendingActionDTO>(),
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex) { return ConvertApiExceptionsToQuery<List<PendingActionDTO>>(ex); }
        }

        public async Task<QueryResponse<MatchWithStatsDTO>> GetMatchWithStats(Guid matchId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.GetMatchWithStatsAsync(matchId);
                return new QueryResponse<MatchWithStatsDTO>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException ex) { return ConvertApiExceptionsToQuery<MatchWithStatsDTO>(ex); }
        }

        public async Task<CommandResponse<bool>> SaveMatchStats(Guid matchId, SaveMatchStatsCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.SaveMatchStatsAsync(matchId, command);
                return new CommandResponse<bool>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex) { return ConvertApiExceptions<bool>(ex); }
            catch (ApiException ex) { return ConvertApiExceptions<bool>(ex); }
        }

        public async Task<CommandResponse<bool>> UpdateMatchVenue(Guid matchId, UpdateMatchVenueCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.UpdateMatchVenueAsync(matchId, command);
                return new CommandResponse<bool>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex) { return ConvertApiExceptions<bool>(ex); }
            catch (ApiException ex) { return ConvertApiExceptions<bool>(ex); }
        }

        public async Task<CommandResponse<bool>> SubmitHomeStats(Guid matchId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.SubmitHomeStatsAsync(matchId);
                return new CommandResponse<bool>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex) { return ConvertApiExceptions<bool>(ex); }
            catch (ApiException ex) { return ConvertApiExceptions<bool>(ex); }
        }

        public async Task<CommandResponse<bool>> ConfirmMatchResult(Guid matchId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.ConfirmMatchResultAsync(matchId);
                return new CommandResponse<bool>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex) { return ConvertApiExceptions<bool>(ex); }
            catch (ApiException ex) { return ConvertApiExceptions<bool>(ex); }
        }

        public async Task<CommandResponse<bool>> DisputeMatchResult(Guid matchId, DisputeMatchResultCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.DisputeMatchResultAsync(matchId, command);
                return new CommandResponse<bool>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex) { return ConvertApiExceptions<bool>(ex); }
            catch (ApiException ex) { return ConvertApiExceptions<bool>(ex); }
        }

        public async Task<CommandResponse<bool>> ResetMatchResult(Guid matchId)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.ResetMatchResultAsync(matchId);
                return new CommandResponse<bool>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex) { return ConvertApiExceptions<bool>(ex); }
            catch (ApiException ex) { return ConvertApiExceptions<bool>(ex); }
        }

        public async Task<CommandResponse<bool>> RecordForfeit(Guid matchId, RecordForfeitCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.RecordForfeitAsync(matchId, command);
                return new CommandResponse<bool>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex) { return ConvertApiExceptions<bool>(ex); }
            catch (ApiException ex) { return ConvertApiExceptions<bool>(ex); }
        }

        public async Task<CommandResponse<bool>> ProposeReschedule(Guid matchId, ProposeRescheduleCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.ProposeRescheduleAsync(matchId, command);
                return new CommandResponse<bool>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex) { return ConvertApiExceptions<bool>(ex); }
            catch (ApiException ex) { return ConvertApiExceptions<bool>(ex); }
        }

        public async Task<CommandResponse<bool>> RespondToReschedule(Guid matchId, RespondToRescheduleCommand command)
        {
            try
            {
                await AddBearerToken();
                var response = await _client.RespondToRescheduleAsync(matchId, command);
                return new CommandResponse<bool>
                {
                    Data = response.Data,
                    IsSuccess = response.IsSuccess,
                    Message = response.Message,
                    Errors = response.Errors?.ToList() ?? new List<string>()
                };
            }
            catch (ApiException<CustomProblemDetails> ex) { return ConvertApiExceptions<bool>(ex); }
            catch (ApiException ex) { return ConvertApiExceptions<bool>(ex); }
        }
    }
}
