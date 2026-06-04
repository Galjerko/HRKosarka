namespace HRKošarka.Application.Features.Player.Queries.GetPlayerCareer
{
    public class PlayerCareerLeagueStatDTO
    {
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public string CompetitionType { get; set; } = string.Empty;
        public int GamesPlayed { get; set; }
        public decimal PPG { get; set; }
        public decimal ThreePG { get; set; }
        public decimal FPG { get; set; }
    }
}
