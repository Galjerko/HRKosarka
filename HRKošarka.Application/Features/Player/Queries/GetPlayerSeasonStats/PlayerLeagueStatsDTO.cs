namespace HRKošarka.Application.Features.Player.Queries.GetPlayerSeasonStats
{
    public class PlayerLeagueStatsDTO
    {
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public string CompetitionType { get; set; } = string.Empty;  // "League" or "Cup"
        public string TeamName { get; set; } = string.Empty;
        public int GamesPlayed { get; set; }
        public decimal PPG { get; set; }
        public decimal ThreePG { get; set; }
        public decimal FPG { get; set; }
        public int TotalPoints { get; set; }
        public int TotalThreePointers { get; set; }
        public int TotalFouls { get; set; }
        public PlayerBestGameDTO? BestGame { get; set; }
    }
}
