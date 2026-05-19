using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Responses;

namespace HRKošarka.UI.Contracts
{
    public interface IMatchService
    {
        Task<QueryResponse<List<PendingActionDTO>>> GetPendingActions();
        Task<QueryResponse<MatchWithStatsDTO>> GetMatchWithStats(Guid matchId);
        Task<CommandResponse<bool>> SaveMatchStats(Guid matchId, SaveMatchStatsCommand command);
        Task<CommandResponse<bool>> UpdateMatchVenue(Guid matchId, UpdateMatchVenueCommand command);
        Task<CommandResponse<bool>> SubmitHomeStats(Guid matchId);
        Task<CommandResponse<bool>> ConfirmMatchResult(Guid matchId);
        Task<CommandResponse<bool>> DisputeMatchResult(Guid matchId, DisputeMatchResultCommand command);
        Task<CommandResponse<bool>> ResetMatchResult(Guid matchId);
        Task<CommandResponse<bool>> RecordForfeit(Guid matchId, RecordForfeitCommand command);
        Task<CommandResponse<bool>> ProposeReschedule(Guid matchId, ProposeRescheduleCommand command);
        Task<CommandResponse<bool>> RespondToReschedule(Guid matchId, RespondToRescheduleCommand command);
    }
}
