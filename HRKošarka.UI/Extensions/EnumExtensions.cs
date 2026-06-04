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
    }
}
