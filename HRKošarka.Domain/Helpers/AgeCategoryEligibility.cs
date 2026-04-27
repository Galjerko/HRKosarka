namespace HRKošarka.Domain.Helpers
{
    public static class AgeCategoryEligibility
    {
        // Age is calculated by year only: currentYear - birthYear <= maxAge.
        private static int? GetMaxAge(string code)
        {
            var upper = code.ToUpperInvariant();

            if (upper.StartsWith("U") && int.TryParse(upper[1..], out var uMaxAge))
            {
                return uMaxAge;
            }


            if (upper == "JUNIORI" || upper == "JUNIORKE")
            {
                return 19;
            }

            return null;
        }

        // Returns the minimum birth year a player must be born in to be eligible.
        // Returns null when there is no restriction (Seniors, etc.).
        public static int? GetMinBirthYear(string categoryCode)
        {
            var maxAge = GetMaxAge(categoryCode);
            return maxAge.HasValue ? DateTime.Today.Year - maxAge.Value : null;
        }

        public static bool IsEligible(string categoryCode, DateTime dateOfBirth)
        {
            var minBirthYear = GetMinBirthYear(categoryCode);
            return !minBirthYear.HasValue || dateOfBirth.Year >= minBirthYear.Value;
        }

        public static string GetAgeRequirementDescription(string categoryCode)
        {
            var maxAge = GetMaxAge(categoryCode);
            return maxAge.HasValue
                ? $"born in {DateTime.Today.Year - maxAge.Value} or later (max age {maxAge} — year-based)"
                : "no age restriction";
        }
    }
}
