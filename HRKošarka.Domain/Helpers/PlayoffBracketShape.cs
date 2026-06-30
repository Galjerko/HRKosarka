namespace HRKošarka.Domain.Helpers
{
    public record PlayoffSeedEntry(Guid TeamId, int Seed);

    public static class PlayoffBracketShape
    {
        // NBA-style bracket pairings for round 1 — (lowerSeedNumber, higherSeedNumber) in bracket order
        private static readonly Dictionary<int, List<(int Top, int Bottom)>> Round1Pairings = new()
        {
            [2] = new() { (1, 2) },
            [4] = new() { (1, 4), (2, 3) },
            [8] = new() { (1, 8), (4, 5), (3, 6), (2, 7) }
        };

        public static bool IsValidTeamCount(int count) => Round1Pairings.ContainsKey(count);

        public static int GetRoundCount(int teamCount) => (int)Math.Log2(teamCount);

        public static string GetRoundName(int roundNumber, int totalRounds)
        {
            int fromEnd = totalRounds - roundNumber;
            return fromEnd switch
            {
                0 => "Final",
                1 => "Semi-Final",
                2 => "Quarter-Final",
                _ => $"Round {roundNumber}"
            };
        }

        public static List<PlayoffSeries> BuildFullBracket(
            List<PlayoffSeedEntry> seeds,
            int teamCount,
            List<int> winsNeededPerRound,
            bool include3rdPlace,
            Guid leagueId,
            DateTime firstDate,
            string? defaultVenue)
        {
            var allSeries = new List<PlayoffSeries>();
            int totalRounds = GetRoundCount(teamCount);
            var round1Pairings = Round1Pairings[teamCount];
            var round1Series = new List<PlayoffSeries>();
            int winsNeeded = winsNeededPerRound[0];

            for (int i = 0; i < round1Pairings.Count; i++)
            {
                var (topSeed, bottomSeed) = round1Pairings[i];
                var homeTeam = seeds.First(s => s.Seed == topSeed);
                var awayTeam = seeds.First(s => s.Seed == bottomSeed);

                var series = new PlayoffSeries
                {
                    LeagueId = leagueId,
                    RoundNumber = 1,
                    RoundName = GetRoundName(1, totalRounds),
                    SeriesNumber = i + 1,
                    WinsNeeded = winsNeeded,
                    HomeTeamId = homeTeam.TeamId,
                    AwayTeamId = awayTeam.TeamId,
                    HomeSeedNumber = topSeed,
                    AwaySeedNumber = bottomSeed,
                    HomeFeederSeriesId = null,
                    AwayFeederSeriesId = null
                };

                // Pre-generate the minimum guaranteed games (WinsNeeded) with alternating venues and +2/+3 day spacing
                var gameSlots = PlayoffSeriesScheduler.GenerateInitialGames(
                    homeTeam.TeamId, awayTeam.TeamId, winsNeeded, firstDate, winsNeeded);

                foreach (var slot in gameSlots)
                    series.Matches.Add(PlayoffSeriesScheduler.ToMatch(
                        slot, leagueId, round: 1, series.RoundName, defaultVenue, series.Id));

                round1Series.Add(series);
                allSeries.Add(series);
            }

            if (totalRounds == 1)
                return allSeries;

            // Build subsequent round stubs — teams unknown, no matches yet
            var previousRoundSeries = round1Series;

            for (int round = 2; round <= totalRounds; round++)
            {
                int roundWins = winsNeededPerRound[round - 1];
                string roundName = GetRoundName(round, totalRounds);
                var thisRoundSeries = new List<PlayoffSeries>();
                int seriesCount = previousRoundSeries.Count / 2;

                for (int i = 0; i < seriesCount; i++)
                {
                    var homeFeeder = previousRoundSeries[i * 2];
                    var awayFeeder = previousRoundSeries[i * 2 + 1];

                    var stub = new PlayoffSeries
                    {
                        LeagueId = leagueId,
                        RoundNumber = round,
                        RoundName = roundName,
                        SeriesNumber = i + 1,
                        WinsNeeded = roundWins,
                        HomeTeamId = null,
                        AwayTeamId = null,
                        HomeFeederSeriesId = homeFeeder.Id,
                        AwayFeederSeriesId = awayFeeder.Id
                    };

                    thisRoundSeries.Add(stub);
                    allSeries.Add(stub);
                }

                previousRoundSeries = thisRoundSeries;
            }

            if (include3rdPlace && totalRounds >= 2)
            {
                var semifinals = allSeries
                    .Where(s => s.RoundNumber == totalRounds - 1)
                    .OrderBy(s => s.SeriesNumber)
                    .ToList();

                if (semifinals.Count == 2)
                {
                    var thirdPlace = new PlayoffSeries
                    {
                        LeagueId = leagueId,
                        RoundNumber = totalRounds + 1,
                        RoundName = "3rd Place",
                        SeriesNumber = 1,
                        WinsNeeded = winsNeededPerRound.Last(),
                        HomeTeamId = null,
                        AwayTeamId = null,
                        HomeFeederSeriesId = semifinals[0].Id,
                        AwayFeederSeriesId = semifinals[1].Id
                    };
                    allSeries.Add(thirdPlace);
                }
            }

            return allSeries;
        }
    }
}
