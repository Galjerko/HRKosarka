using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRKošarka.Domain
{
    public class EmailNotification : BaseEntity
    {
        [Required]
        [Column(TypeName = "varchar(450)")]
        public string UserId { get; set; } = string.Empty;

        [Required]
        public Guid MatchId { get; set; }

        [Required]
        public NotificationType NotificationType { get; set; }

        [Required]
        public DateTime SentAt { get; set; }

        public bool IsSuccessful { get; set; }

        // Navigation properties
        public virtual Match Match { get; set; } = null!;
    }
}
