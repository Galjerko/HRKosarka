using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRKošarka.Domain
{
    public class League : BaseEntity
    {
        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid SeasonId { get; set; }

        [Required]
        public Guid AgeCategoryId { get; set; }

        [Required]
        public Gender Gender { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        [Required]
        public CompetitionType CompetitionType { get; set; }

        public int NumberOfRounds { get; set; } = 1; // For double round-robin = 2

        public bool ScheduleGenerated { get; set; } = false;

        public bool IsFeatured { get; set; } = false;

        public bool HasPlayoff { get; set; } = false;

        public int? PlayoffTeamCount { get; set; } // 2, 4, or 8

        public bool PlayoffGenerated { get; set; } = false;

        public bool PlayoffHas3rdPlace { get; set; } = false;

        public DateTime? PlayoffEndDate { get; set; }

        [MaxLength(200)]
        public string? PlayoffRoundWinsNeeded { get; set; } // JSON {"1":3,"2":2} stored at generation

        public DateTime PlayoffCapDate => (PlayoffEndDate ?? EndDate).Date.AddHours(19);

        public bool IsActive { get; set; } = true;

        public DateTime? DeactivateDate { get; set; }

        [MaxLength(255)]
        public string? DefaultVenue { get; set; }

        [MaxLength(255)]
        public string? ImageName { get; set; }

        [MaxLength(100)]
        public string? ImageContentType { get; set; }

        public byte[]? ImageBytes { get; set; }

        public virtual Season Season { get; set; } = null!;
        public virtual AgeCategory AgeCategory { get; set; } = null!;
        public virtual ICollection<LeagueTeam> LeagueTeams { get; set; } = new List<LeagueTeam>();
        public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
        public virtual ICollection<LeagueStanding> Standings { get; set; } = new List<LeagueStanding>();
        public virtual ICollection<LeagueBreak> Breaks { get; set; } = new List<LeagueBreak>();
        public virtual ICollection<PlayoffSeries> PlayoffSeries { get; set; } = new List<PlayoffSeries>();
    }
}
