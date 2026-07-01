using HRKošarka.Application.Features.League.Queries.GetLeagueLeaderboard;

namespace HRKošarka.Persistence.Repositories
{
    internal static class RepositorySortHelper
    {
        public static List<T> ApplySort<T>(
            List<T> list,
            string? sortBy,
            bool ascending,
            Dictionary<string, Func<T, object>> selectors,
            Func<T, object> defaultSelector)
        {
            if (sortBy != null && selectors.TryGetValue(sortBy, out var selector))
                return ascending ? list.OrderBy(selector).ToList() : list.OrderByDescending(selector).ToList();

            return list.OrderByDescending(defaultSelector).ToList();
        }

        public static readonly Dictionary<string, Func<LeaguePlayerStatDTO, object>> LeaguePlayerStatSortSelectors =
            new(StringComparer.OrdinalIgnoreCase)
            {
                [nameof(LeaguePlayerStatDTO.PlayerName)] = x => x.PlayerName,
                [nameof(LeaguePlayerStatDTO.TeamName)] = x => x.TeamName,
                [nameof(LeaguePlayerStatDTO.GamesPlayed)] = x => x.GamesPlayed,
                [nameof(LeaguePlayerStatDTO.PPG)] = x => x.PPG,
                [nameof(LeaguePlayerStatDTO.TotalPoints)] = x => x.TotalPoints,
                [nameof(LeaguePlayerStatDTO.ThreePointsPerGame)] = x => x.ThreePointsPerGame,
                [nameof(LeaguePlayerStatDTO.TotalThreePoints)] = x => x.TotalThreePoints,
                [nameof(LeaguePlayerStatDTO.FoulsPerGame)] = x => x.FoulsPerGame,
                [nameof(LeaguePlayerStatDTO.TotalFouls)] = x => x.TotalFouls,
            };
    }
}
