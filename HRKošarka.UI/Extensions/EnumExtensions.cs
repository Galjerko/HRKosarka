using MudBlazor;

namespace HRKošarka.UI.Extensions
{
    public static class EnumExtensions
    {
        public static string ToDisplayString(this HRKošarka.UI.Services.Base.Gender gender) => gender switch
        {
            HRKošarka.UI.Services.Base.Gender._0 => "Male",
            HRKošarka.UI.Services.Base.Gender._1 => "Female",
            _ => gender.ToString()
        };

        public static string ToDisplayString(this HRKošarka.UI.Services.Base.CompetitionType type) => type switch
        {
            HRKošarka.UI.Services.Base.CompetitionType._0 => "League",
            HRKošarka.UI.Services.Base.CompetitionType._1 => "Cup",
            _ => type.ToString()
        };

        public static string ToDisplayString(this HRKošarka.UI.Services.Base.MatchStatus status) => status switch
        {
            HRKošarka.UI.Services.Base.MatchStatus._0 => "Scheduled",
            HRKošarka.UI.Services.Base.MatchStatus._1 => "Rescheduled",
            HRKošarka.UI.Services.Base.MatchStatus._2 => "Completed",
            HRKošarka.UI.Services.Base.MatchStatus._3 => "Forfeit",
            _ => status.ToString()
        };

        public static Color ToColor(this HRKošarka.UI.Services.Base.MatchStatus status) => status switch
        {
            HRKošarka.UI.Services.Base.MatchStatus._1 => Color.Warning,
            HRKošarka.UI.Services.Base.MatchStatus._2 => Color.Success,
            HRKošarka.UI.Services.Base.MatchStatus._3 => Color.Secondary,
            _ => Color.Default
        };

        public static string ToDisplayString(this HRKošarka.UI.Services.Base.ResultSubmissionStatus status) => status switch
        {
            HRKošarka.UI.Services.Base.ResultSubmissionStatus._0 => "Not Submitted",
            HRKošarka.UI.Services.Base.ResultSubmissionStatus._1 => "Pending Confirmation",
            HRKošarka.UI.Services.Base.ResultSubmissionStatus._2 => "Confirmed",
            HRKošarka.UI.Services.Base.ResultSubmissionStatus._3 => "Disputed",
            _ => status.ToString()
        };

        public static Color ToColor(this HRKošarka.UI.Services.Base.ResultSubmissionStatus status) => status switch
        {
            HRKošarka.UI.Services.Base.ResultSubmissionStatus._1 => Color.Warning,
            HRKošarka.UI.Services.Base.ResultSubmissionStatus._2 => Color.Success,
            HRKošarka.UI.Services.Base.ResultSubmissionStatus._3 => Color.Error,
            _ => Color.Default
        };

        public static string ToAbbr(this HRKošarka.UI.Services.Base.Position? position) => position switch
        {
            HRKošarka.UI.Services.Base.Position._0 => "PG",
            HRKošarka.UI.Services.Base.Position._1 => "SG",
            HRKošarka.UI.Services.Base.Position._2 => "SF",
            HRKošarka.UI.Services.Base.Position._3 => "PF",
            HRKošarka.UI.Services.Base.Position._4 => "C",
            _ => "–"
        };

        public static string ToAbbr(this HRKošarka.UI.Services.Base.Position position)
            => ((HRKošarka.UI.Services.Base.Position?)position).ToAbbr();

        public static string ToDisplayString(this HRKošarka.UI.Services.Base.NotificationType type) => type switch
        {
            HRKošarka.UI.Services.Base.NotificationType._0 => "Match Scheduled",
            HRKošarka.UI.Services.Base.NotificationType._1 => "Match Rescheduled",
            HRKošarka.UI.Services.Base.NotificationType._2 => "Match Cancelled",
            HRKošarka.UI.Services.Base.NotificationType._3 => "Match Result",
            HRKošarka.UI.Services.Base.NotificationType._4 => "Representative Assigned",
            HRKošarka.UI.Services.Base.NotificationType._5 => "Representative Revoked",
            HRKošarka.UI.Services.Base.NotificationType._6 => "Club Manager Assigned",
            HRKošarka.UI.Services.Base.NotificationType._7 => "Club Manager Removed",
            HRKošarka.UI.Services.Base.NotificationType._8 => "Stats Submitted",
            HRKošarka.UI.Services.Base.NotificationType._9 => "Match Disputed",
            HRKošarka.UI.Services.Base.NotificationType._10 => "Match Reset",
            HRKošarka.UI.Services.Base.NotificationType._11 => "Reschedule Proposed",
            HRKošarka.UI.Services.Base.NotificationType._12 => "Reschedule Accepted",
            HRKošarka.UI.Services.Base.NotificationType._13 => "Reschedule Rejected",
            HRKošarka.UI.Services.Base.NotificationType._14 => "Venue Changed",
            HRKošarka.UI.Services.Base.NotificationType._15 => "Forfeit Recorded",
            HRKošarka.UI.Services.Base.NotificationType._16 => "Cup Round Advanced",
            _ => type.ToString()
        };
    }
}
