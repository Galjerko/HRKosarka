using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRKošarka.Domain
{
    public class LeagueTeam : BaseEntity
    {
        [Required]
        public Guid LeagueId { get; set; }

        [Required]
        public Guid TeamId { get; set; }

        [Required]
        public DateTime RegistrationDate { get; set; }

        public bool IsActive { get; set; } = true;

        public virtual League League { get; set; } = null!;
        public virtual Team Team { get; set; } = null!;
    }
}
