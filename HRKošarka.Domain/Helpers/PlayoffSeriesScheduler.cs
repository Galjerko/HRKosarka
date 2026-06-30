using HRKošarka.Domain.Common;

namespace HRKošarka.Domain.Helpers
{
    public record PlayoffGameSlot(int GameNumber, DateTime Date, Guid HomeTeamId, Guid AwayTeamId);

    public static class PlayoffSeriesScheduler
    {
        // true = home series team (lower seed number) hosts; false = away series team hosts
        // Bo3 (WinsNeeded=2): H A H
        // Bo5 (WinsNeeded=3): H H A A H
        // Bo7 (WinsNeeded=4): H H A A H A H
        private static readonly Dictionary<int, bool[]> VenuePatterns = new()
        {
            [2] = new[] { true, false, true },
            [3] = new[] { true, true, false, false, true },
            [4] = new[] { true, true, false, false, true, false, true }
        };

        // Days to add AFTER game N before game N+1 (odd game → 2 days, even → 3 days)
        public static int GapDaysAfterGame(int gameNumber) => gameNumber % 2 == 1 ? 2 : 3;

        public static bool HomeSeriesTeamHostsGame(int winsNeeded, int gameNumber)
            => VenuePatterns[winsNeeded][gameNumber - 1];

        /// <summary>
        /// Generates the first <paramref name="gameCount"/> games of a series starting at <paramref name="game1Date"/>.
        /// <paramref name="homeSeriesTeamId"/> is the lower-seeded team (always gets the H-pattern slots).
        /// </summary>
        public static List<PlayoffGameSlot> GenerateInitialGames(
            Guid homeSeriesTeamId, Guid awaySeriesTeamId, int winsNeeded, DateTime game1Date, int gameCount)
        {
            var slots = new List<PlayoffGameSlot>(gameCount);
            var currentDate = game1Date;
            for (int gameNumber = 1; gameNumber <= gameCount; gameNumber++)
            {
                if (gameNumber > 1)
                    currentDate = currentDate.AddDays(GapDaysAfterGame(gameNumber - 1));

                bool homeSideHosts = HomeSeriesTeamHostsGame(winsNeeded, gameNumber);
                slots.Add(new PlayoffGameSlot(
                    gameNumber,
                    currentDate,
                    homeSideHosts ? homeSeriesTeamId : awaySeriesTeamId,
                    homeSideHosts ? awaySeriesTeamId : homeSeriesTeamId));
            }
            return slots;
        }

        /// <summary>
        /// Generates the single next game following <paramref name="previousGameNumber"/> (at <paramref name="previousGameDate"/>).
        /// Used when a series exceeds its pre-generated minimum.
        /// </summary>
        public static PlayoffGameSlot GenerateNextGame(
            Guid homeSeriesTeamId, Guid awaySeriesTeamId, int winsNeeded,
            int previousGameNumber, DateTime previousGameDate)
        {
            var nextGameNumber = previousGameNumber + 1;
            var nextDate = previousGameDate.AddDays(GapDaysAfterGame(previousGameNumber));
            bool homeSideHosts = HomeSeriesTeamHostsGame(winsNeeded, nextGameNumber);
            return new PlayoffGameSlot(
                nextGameNumber,
                nextDate,
                homeSideHosts ? homeSeriesTeamId : awaySeriesTeamId,
                homeSideHosts ? awaySeriesTeamId : homeSeriesTeamId);
        }

        public static Match ToMatch(
            PlayoffGameSlot slot, Guid leagueId, int round, string roundName, string? venue, Guid seriesId)
        {
            return new Match
            {
                LeagueId = leagueId,
                HomeTeamId = slot.HomeTeamId,
                AwayTeamId = slot.AwayTeamId,
                Round = round,
                RoundName = roundName,
                DefaultScheduledDate = slot.Date,
                ActualScheduledDate = slot.Date,
                Status = MatchStatus.Scheduled,
                SchedulingStatus = SchedulingStatus.Default,
                LastSchedulingUpdate = DateTime.Now,
                VenueOverride = venue,
                PlayoffSeriesId = seriesId
            };
        }
    }
}
