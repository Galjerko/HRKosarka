using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRKošarka.Domain
{
    /// <summary>
    /// League standings table - calculated via background jobs
    /// </summary>
    public class LeagueStanding : BaseEntity
    {
        [Required]
        public Guid LeagueId { get; set; }

        [Required]
        public Guid TeamId { get; set; }

        [Required]
        public Guid SeasonId { get; set; }

        public int Position { get; set; }

        public int MatchesPlayed { get; set; } = 0;

        public int Wins { get; set; } = 0;

        public int Losses { get; set; } = 0;

        public int PointsFor { get; set; } = 0;

        public int PointsAgainst { get; set; } = 0;

        public int PointsDifference { get; set; } = 0;

        public int LeaguePoints { get; set; } = 0; // 2 for win, 1 for loss

        public bool IsFinal { get; set; } = false;

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public virtual League League { get; set; } = null!;
        public virtual Team Team { get; set; } = null!;
        public virtual Season Season { get; set; } = null!;
    }
}
