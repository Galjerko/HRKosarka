using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.Reflection;
using System.Text.RegularExpressions;

namespace HRKošarka.Domain
{
    public class Team : BaseEntity
    {
        [Required]
        public Guid ClubId { get; set; }

        [Required]
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [Required]
        public Guid AgeCategoryId { get; set; }

        [Required]
        public Gender Gender { get; set; }

        public DateTime? DeactivateDate { get; set; }

        public bool IsActive => DeactivateDate == null;

        public virtual Club Club { get; set; } = null!;
        public virtual AgeCategory AgeCategory { get; set; } = null!;
        public virtual ICollection<LeagueTeam> LeagueTeams { get; set; } = new List<LeagueTeam>();
        public virtual ICollection<PlayerTeamHistory> PlayerHistory { get; set; } = new List<PlayerTeamHistory>();
        public virtual ICollection<TeamRepresentative> Representatives { get; set; } = new List<TeamRepresentative>();
        public virtual ICollection<Match> HomeMatches { get; set; } = new List<Match>();
        public virtual ICollection<Match> AwayMatches { get; set; } = new List<Match>();
    }
}
