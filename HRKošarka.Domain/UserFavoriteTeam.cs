using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRKošarka.Domain
{
    public class UserFavoriteTeam : BaseEntity
    {
        [Required]
        [Column(TypeName = "varchar(450)")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public Guid TeamId { get; set; }

        public bool NotifyByEmail { get; set; } = true;

        // Navigation properties
        public virtual Team Team { get; set; } = null!;
    }
}
