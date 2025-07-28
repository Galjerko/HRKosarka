using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRKošarka.Domain
{
    public class Match : BaseEntity
    {
        [Required]
        public Guid LeagueId { get; set; }

        [Required]
        public Guid HomeTeamId { get; set; }

        [Required]
        public Guid AwayTeamId { get; set; }

        [Required]
        public int Round { get; set; }

        [MaxLength(100)]
        public string? RoundName { get; set; } // "Round 1", "Quarterfinal", etc.

        [MaxLength(200)]
        public string? VenueOverride { get; set; } // For neutral venues in cups

        [Required]
        public DateTime DefaultScheduledDate { get; set; } // Auto-generated Saturday 5:00 PM

        [Required]
        public DateTime ActualScheduledDate { get; set; } // After team agreement

        public int? HomeScore { get; set; }

        public int? AwayScore { get; set; }

        [MaxLength(500)]
        public string? QuarterResults { get; set; } // JSON: "Q1:22-32;Q2:21-32;Q3:18-25;Q4:15-20;OT1:8-10"

        [Required]
        public MatchStatus Status { get; set; } = MatchStatus.Scheduled;

        public bool IsResultConfirmed { get; set; } = false;

        [Required]
        public SchedulingStatus SchedulingStatus { get; set; } = SchedulingStatus.Default;

        [Column(TypeName = "varchar(450)")]
        public string? ConfirmedByUserId { get; set; }

        public DateTime? ConfirmedAt { get; set; }

        [Required]
        public DateTime LastSchedulingUpdate { get; set; } = DateTime.UtcNow;

        [MaxLength(1000)]
        public string? Notes { get; set; }

        // Navigation properties
        public virtual League League { get; set; } = null!;
        public virtual Team HomeTeam { get; set; } = null!;
        public virtual Team AwayTeam { get; set; } = null!;
        public virtual ICollection<MatchReschedulingRequest> ReschedulingRequests { get; set; } = new List<MatchReschedulingRequest>();
        public virtual ICollection<PlayerMatchStats> PlayerStats { get; set; } = new List<PlayerMatchStats>();
        public virtual ICollection<EmailNotification> EmailNotifications { get; set; } = new List<EmailNotification>();
    }
}
