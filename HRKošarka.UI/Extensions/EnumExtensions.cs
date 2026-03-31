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
    }
}
