using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

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

        public bool IsActive { get; set; } = true;

        public virtual Season Season { get; set; } = null!;
        public virtual AgeCategory AgeCategory { get; set; } = null!;
        public virtual ICollection<LeagueTeam> LeagueTeams { get; set; } = new List<LeagueTeam>();
        public virtual ICollection<Match> Matches { get; set; } = new List<Match>();
        public virtual ICollection<LeagueStanding> Standings { get; set; } = new List<LeagueStanding>();
    }
}
