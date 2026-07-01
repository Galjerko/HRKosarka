namespace HRKošarka.Application.Features.Player.Queries.GetPlayerSeasonStats
{
    public class PlayerBestGameDTO
    {
        public Guid MatchId { get; set; }
        public int Points { get; set; }
        public int ThreePointers { get; set; }
        public string OpponentTeamName { get; set; } = string.Empty;
        public DateTime MatchDate { get; set; }
    }
}
