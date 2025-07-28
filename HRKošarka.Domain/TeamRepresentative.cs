using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRKošarka.Domain
{
    public class TeamRepresentative : BaseEntity
    {
        [Required]
        [Column(TypeName = "varchar(450)")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public Guid TeamId { get; set; }

        public DateTime? DeactivateDate { get; set; }

        public bool IsActive => DeactivateDate == null;

        public DateTime AssignedDate { get; set; } = DateTime.UtcNow;

        public virtual Team Team { get; set; } = null!;
    }
}
