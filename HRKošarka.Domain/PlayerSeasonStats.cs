using HRKošarka.Domain.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HRKošarka.Domain
{
    /// <summary>
    /// Aggregated player statistics for entire seasons - calculated via background jobs
    /// </summary>
    public class PlayerSeasonStats : BaseEntity
    {
        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        public Guid LeagueId { get; set; }

        [Required]
        public Guid SeasonId { get; set; }

        [Required]
        public Guid TeamId { get; set; } // For history tracking

        public int TotalPoints { get; set; } = 0;

        public int TotalFouls { get; set; } = 0;

        public int TotalThreePointers { get; set; } = 0;

        public int MatchesPlayed { get; set; } = 0;

        public decimal AveragePoints { get; set; } = 0;

        public decimal AverageThreePointers { get; set; } = 0;

        public bool IsFinal { get; set; } = false;

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        public virtual Player Player { get; set; } = null!;
        public virtual League League { get; set; } = null!;
        public virtual Season Season { get; set; } = null!;
        public virtual Team Team { get; set; } = null!;
    }
}
