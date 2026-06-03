namespace HRKošarka.Domain.Helpers
{
    public static class CupBracketScheduler
    {
        /// <summary>
        /// Generates round 1 match slots for a cup bracket.
        /// Teams with byes (when N is not a power of 2) are excluded from round 1 —
        /// they are identified later by their absence from all round 1 matches.
        /// </summary>
        public static List<ScheduledMatchSlot> GenerateRound1(
            IReadOnlyList<Guid> teamIds,
            DateTime startDate,
            IReadOnlyList<(DateTime Start, DateTime End)> breaks)
        {
            var shuffled = teamIds.ToList();
            var rng = new Random();
            for (int i = shuffled.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (shuffled[i], shuffled[j]) = (shuffled[j], shuffled[i]);
            }

            int n = shuffled.Count;
            int nextPow2 = NextPowerOfTwo(n);
            int byes = nextPow2 - n;

            // First `byes` teams get a bye — remaining teams play round 1
            var playingTeams = shuffled.Skip(byes).ToList();

            var roundDate = FindNextValidSaturday(startDate, breaks.ToList());
            var roundName = GetCupRoundName(nextPow2);

            var slots = new List<ScheduledMatchSlot>();
            for (int i = 0; i < playingTeams.Count / 2; i++)
            {
                slots.Add(new ScheduledMatchSlot(
                    HomeTeamId: playingTeams[i],
                    AwayTeamId: playingTeams[playingTeams.Count - 1 - i],
                    Round: 1,
                    RoundName: roundName,
                    Date: roundDate
                ));
            }

            return slots;
        }

        public static string GetCupRoundName(int teamsInRound) => teamsInRound switch
        {
            2 => "Final",
            4 => "Semi-finals",
            8 => "Quarter-finals",
            16 => "Round of 16",
            32 => "Round of 32",
            _ => $"Round of {teamsInRound}"
        };

        public static DateTime FindNextValidSaturday(DateTime from, List<(DateTime Start, DateTime End)> breaks)
        {
            int daysUntilSaturday = ((int)DayOfWeek.Saturday - (int)from.DayOfWeek + 7) % 7;
            if (daysUntilSaturday == 0 && from.TimeOfDay >= TimeSpan.FromHours(19))
                daysUntilSaturday = 7;
            var candidate = from.Date.AddDays(daysUntilSaturday).AddHours(19);
            while (IsWeekInBreak(candidate, breaks))
                candidate = candidate.AddDays(7);
            return candidate;
        }

        private static bool IsWeekInBreak(DateTime saturday, List<(DateTime Start, DateTime End)> breaks)
        {
            var weekStart = saturday.Date.AddDays(-5);
            var weekEnd = saturday.Date.AddDays(1);
            return breaks.Any(b => b.Start.Date <= weekEnd && b.End.Date >= weekStart);
        }

        private static int NextPowerOfTwo(int n)
        {
            int pow = 1;
            while (pow < n) pow <<= 1;
            return pow;
        }
    }
}
