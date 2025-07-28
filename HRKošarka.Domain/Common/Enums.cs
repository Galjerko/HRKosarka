namespace HRKošarka.Domain.Common
{
    public enum Gender
    {
        Male,
        Female
    }

    public enum CompetitionType
    {
        League,
        Cup
    }

    public enum Position
    {
        PointGuard,
        ShootingGuard,
        SmallForward,
        PowerForward,
        Center
    }

    public enum MatchStatus
    {
        Scheduled,   
        Rescheduled,  
        Completed, 
        Cancelled 
    }

    public enum SchedulingStatus
    {
        Default,        
        ProposalPending, 
        Agreed
    }

    public enum RequestStatus
    {
        Pending,
        Accepted,
        Rejected,
        Expired
    }

    public enum NotificationType
    {
        MatchScheduled,  
        MatchRescheduled,
        MatchCancelled, 
        MatchResult
    }

}
