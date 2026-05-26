namespace HRKošarka.Application.Features.Team.Queries.GetTeamLeaguePlayerStats
{
    public class TeamPlayerStatDTO
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int GamesPlayed { get; set; }
        public decimal PPG { get; set; }
        public decimal ThreePG { get; set; }
        public decimal FPG { get; set; }
        public int TotalPoints { get; set; }
        public int TotalThreePointers { get; set; }
        public int TotalFouls { get; set; }
    }
}
