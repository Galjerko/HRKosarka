namespace HRKošarka.Domain.Helpers
{
    public static class RoundRobinScheduler
    {
        private const int Attempts = 15;

        public static List<ScheduledMatchSlot> Generate(
            IReadOnlyList<Guid> teamIds,
            DateTime startDate,
            int numberOfRounds,
            IReadOnlyList<(DateTime Start, DateTime End)> breaks)
        {
            // Run multiple random shuffles and return the schedule with the fewest
            // adjacent-week same-venue pairs. Each attempt is ~O(n²) so 15 runs is negligible.
            List<ScheduledMatchSlot>? best = null;
            int bestBreaks = int.MaxValue;

            for (int attempt = 0; attempt < Attempts; attempt++)
            {
                var candidate = GenerateSingle(teamIds, startDate, numberOfRounds, breaks);
                int breaks_ = CountAdjacentWeekBreaks(candidate);
                if (breaks_ < bestBreaks)
                {
                    bestBreaks = breaks_;
                    best = candidate;
                }
            }

            return best!;
        }

        private static List<ScheduledMatchSlot> GenerateSingle(
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

            return OptimizeBreaks(result);
        }

        // Counts adjacent-week same-venue pairs across all teams (the metric we minimise).
        // Gaps from byes or league breaks (≥14 days between a team's consecutive matches)
        // are excluded — a rest week resets the streak burden.
        private static int CountAdjacentWeekBreaks(List<ScheduledMatchSlot> slots)
        {
            var byTeam = new Dictionary<Guid, List<ScheduledMatchSlot>>();
            foreach (var s in slots)
            {
                AddSlot(byTeam, s.HomeTeamId, s);
                AddSlot(byTeam, s.AwayTeamId, s);
            }

            int total = 0;
            foreach (var (teamId, matches) in byTeam)
            {
                matches.Sort((a, b) => a.Date.CompareTo(b.Date));
                bool? prev = null;
                DateTime? prevDate = null;
                foreach (var m in matches)
                {
                    bool isHome = m.HomeTeamId == teamId;
                    if (prev == isHome && prevDate.HasValue && (m.Date - prevDate.Value).TotalDays <= 8)
                        total++;
                    prev = isHome;
                    prevDate = m.Date;
                }
            }
            return total;
        }

        // Hill-climbing local search: swap individual match H/A assignments until no single
        // swap reduces the total adjacent-week same-venue pair count.
        private static List<ScheduledMatchSlot> OptimizeBreaks(List<ScheduledMatchSlot> slots)
        {
            var teamIndices = new Dictionary<Guid, List<int>>();
            for (int i = 0; i < slots.Count; i++)
            {
                AddIndex(teamIndices, slots[i].HomeTeamId, i);
                AddIndex(teamIndices, slots[i].AwayTeamId, i);
            }
            foreach (var list in teamIndices.Values)
                list.Sort((x, y) => slots[x].Round.CompareTo(slots[y].Round));

            int CountBreaks()
            {
                int total = 0;
                foreach (var (teamId, indices) in teamIndices)
                {
                    bool? prev = null;
                    DateTime? prevDate = null;
                    foreach (var idx in indices)
                    {
                        bool isHome = slots[idx].HomeTeamId == teamId;
                        var d = slots[idx].Date;
                        if (prev == isHome && prevDate.HasValue && (d - prevDate.Value).TotalDays <= 8)
                            total++;
                        prev = isHome;
                        prevDate = d;
                    }
                }
                return total;
            }

            bool improved = true;
            while (improved)
            {
                improved = false;
                int current = CountBreaks();
                for (int i = 0; i < slots.Count; i++)
                {
                    var orig = slots[i];
                    slots[i] = orig with { HomeTeamId = orig.AwayTeamId, AwayTeamId = orig.HomeTeamId };
                    int after = CountBreaks();
                    if (after < current)
                    {
                        improved = true;
                        current = after;
                    }
                    else
                    {
                        slots[i] = orig;
                    }
                }
            }

            return slots;
        }

        private static void AddIndex(Dictionary<Guid, List<int>> dict, Guid key, int index)
        {
            if (!dict.TryGetValue(key, out var list))
                dict[key] = list = new List<int>();
            list.Add(index);
        }

        private static void AddSlot(Dictionary<Guid, List<ScheduledMatchSlot>> dict, Guid key, ScheduledMatchSlot slot)
        {
            if (!dict.TryGetValue(key, out var list))
                dict[key] = list = new List<ScheduledMatchSlot>();
            list.Add(slot);
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
