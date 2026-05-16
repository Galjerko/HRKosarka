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
            HRKošarka.UI.Services.Base.MatchStatus._3 => "Cancelled",
            _ => status.ToString()
        };

        public static Color ToColor(this HRKošarka.UI.Services.Base.MatchStatus status) => status switch
        {
            HRKošarka.UI.Services.Base.MatchStatus._1 => Color.Warning,
            HRKošarka.UI.Services.Base.MatchStatus._2 => Color.Success,
            HRKošarka.UI.Services.Base.MatchStatus._3 => Color.Error,
            _ => Color.Default
        };
    }
}
