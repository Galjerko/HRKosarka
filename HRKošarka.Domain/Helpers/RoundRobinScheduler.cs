namespace HRKošarka.Domain.Helpers
{
    public static class RoundRobinScheduler
    {
        public static List<ScheduledMatchSlot> Generate(
            IReadOnlyList<Guid> teamIds,
            DateTime startDate,
            int numberOfRounds,
            IReadOnlyList<(DateTime Start, DateTime End)> breaks)
        {
            var slots = teamIds.Select(id => (Guid?)id).ToList();

            var rng = new Random();
            for (int i = slots.Count - 1; i > 0; i--)
            {
                int j = rng.Next(i + 1);
                (slots[i], slots[j]) = (slots[j], slots[i]);
            }

            if (slots.Count % 2 != 0)
                slots.Add(null);

            int n = slots.Count;
            int roundsPerHalf = n - 1;

            var allRounds = new List<List<(Guid Home, Guid Away)>>();
            var current = slots.ToList();

            for (int round = 0; round < roundsPerHalf; round++)
            {
                var pairings = new List<(Guid Home, Guid Away)>();
                for (int i = 0; i < n / 2; i++)
                {
                    var home = current[i];
                    var away = current[n - 1 - i];
                    if (home.HasValue && away.HasValue)
                        pairings.Add((home.Value, away.Value));
                }
                allRounds.Add(pairings);

                var last = current[n - 1];
                for (int i = n - 1; i > 1; i--)
                    current[i] = current[i - 1];
                current[1] = last;
            }

            if (numberOfRounds == 2)
            {
                var secondHalf = allRounds
                    .Select(r => r.Select(m => (m.Away, m.Home)).ToList())
                    .ToList();
                allRounds.AddRange(secondHalf);
            }

            var result = new List<ScheduledMatchSlot>();
            var breakList = breaks.ToList();
            var date = startDate;

            for (int roundIndex = 0; roundIndex < allRounds.Count; roundIndex++)
            {
                var roundDate = FindNextValidSaturday(date, breakList);
                date = roundDate.AddDays(1);

                int roundNumber = roundIndex + 1;
                int roundWithinHalf = (roundIndex % roundsPerHalf) + 1;
                bool swap = roundWithinHalf % 2 == 0;

                foreach (var (home, away) in allRounds[roundIndex])
                {
                    result.Add(new ScheduledMatchSlot(
                        HomeTeamId: swap ? away : home,
                        AwayTeamId: swap ? home : away,
                        Round: roundNumber,
                        RoundName: $"Round {roundNumber}",
                        Date: roundDate
                    ));
                }
            }

            return result;
        }

        private static DateTime FindNextValidSaturday(DateTime from, List<(DateTime Start, DateTime End)> breaks)
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
    }

    public record ScheduledMatchSlot(
        Guid HomeTeamId,
        Guid AwayTeamId,
        int Round,
        string RoundName,
        DateTime Date
    );
}
