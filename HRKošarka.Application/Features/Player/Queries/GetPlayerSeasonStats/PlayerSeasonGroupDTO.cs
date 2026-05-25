namespace HRKošarka.Application.Features.Player.Queries.GetPlayerSeasonStats
{
    public class PlayerSeasonGroupDTO
    {
        public Guid SeasonId { get; set; }
        public string SeasonName { get; set; } = string.Empty;
        public int TotalGamesPlayed { get; set; }
        public decimal CombinedPPG { get; set; }
        public decimal Combined3PG { get; set; }
        public decimal CombinedFPG { get; set; }
        public List<PlayerLeagueStatsDTO> LeagueStats { get; set; } = new();
    }
}
