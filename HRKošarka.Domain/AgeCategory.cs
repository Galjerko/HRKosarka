using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRKošarka.Domain
{
    public class AgeCategory : BaseEntity
    {
        [Required]
        [MaxLength(20)]
        public string Name { get; set; } = string.Empty;

        [Required]
        [MaxLength(10)]
        public string Code { get; set; } = string.Empty; // "U9", "U11", etc.

        public virtual ICollection<Team> Teams { get; set; } = new List<Team>();
        public virtual ICollection<League> Leagues { get; set; } = new List<League>();
    }
}
