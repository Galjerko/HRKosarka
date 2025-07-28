using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRKošarka.Domain
{
    public class Season : BaseEntity
    {
        [Required]
        [MaxLength(10)]
        public string Name { get; set; } = string.Empty; // "2024/25"

        [Required]
        public DateTime StartDate { get; set; }

        [Required]
        public DateTime EndDate { get; set; }

        public bool IsActive { get; set; } = true;

        public bool IsCompleted { get; set; } = false;

        public virtual ICollection<League> Leagues { get; set; } = new List<League>();
        public virtual ICollection<PlayerTeamHistory> PlayerHistory { get; set; } = new List<PlayerTeamHistory>();
    }
}
