using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRKošarka.Domain
{
    public class PlayerTeamHistory : BaseEntity
    {
        [Required]
        public Guid PlayerId { get; set; }

        [Required]
        public Guid TeamId { get; set; }

        [Required]
        public Guid SeasonId { get; set; }

        [Required]
        public DateTime JoinDate { get; set; }

        public DateTime? LeaveDate { get; set; }

        public int? JerseyNumber { get; set; }

        public bool IsActive { get; set; } = true;
        public virtual Player Player { get; set; } = null!;
        public virtual Team Team { get; set; } = null!;
        public virtual Season Season { get; set; } = null!;
    }
}
