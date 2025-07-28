using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRKošarka.Domain
{
    public class PlayerMatchStats : BaseEntity
    {
        [Required]
        public Guid MatchId { get; set; }

        [Required]
        public Guid PlayerId { get; set; }

        public int Points { get; set; } = 0;

        public int Fouls { get; set; } = 0;

        public int ThreePointers { get; set; } = 0;

        public virtual Match Match { get; set; } = null!;
        public virtual Player Player { get; set; } = null!;
    }
}
