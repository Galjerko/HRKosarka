using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueLeaderboard
{
    public class LeaguePlayerStatDTO
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public Position? PlayerPosition { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public int GamesPlayed { get; set; }
        public decimal PPG { get; set; }
        public int TotalPoints { get; set; }
        public decimal ThreePointsPerGame { get; set; }
        public int TotalThreePoints { get; set; }
        public decimal FoulsPerGame { get; set; }
        public int TotalFouls { get; set; }
    }
}
