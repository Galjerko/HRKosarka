using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Match.Queries.GetPendingActions
{
    public class PendingActionDTO
    {
        public Guid MatchId { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public string RoundName { get; set; } = string.Empty;
        public string HomeTeamName { get; set; } = string.Empty;
        public string AwayTeamName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public PendingActionType ActionType { get; set; }
    }

    public enum PendingActionType
    {
        SubmitHomeStats     = 0,  // home manager: enter + submit stats
        EnterAwayStats      = 1,  // away manager: enter away player stats
        ConfirmResult       = 2,  // away manager: home submitted, pending confirmation
        RespondToProposal   = 3,  // other team proposed a reschedule, you must accept/reject
        ProposalPending     = 4,  // you proposed, waiting for other team
        ResolveDispute      = 5,  // admin: disputed match needs resolution
    }
}
