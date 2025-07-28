using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRKošarka.Domain
{
    public class MatchReschedulingRequest : BaseEntity
    {
        [Required]
        public Guid MatchId { get; set; }

        [Required]
        [Column(TypeName = "varchar(450)")]
        public string RequestedByUserId { get; set; } = string.Empty;

        [Required]
        public DateTime ProposedDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;

        [Required]
        public RequestStatus Status { get; set; } = RequestStatus.Pending;

        [Column(TypeName = "varchar(450)")]
        public string? ResponseByUserId { get; set; }

        [Required]
        public DateTime ExpiresAt { get; set; } // Auto-expire after 48 hours

        public DateTime? RespondedAt { get; set; }

        // Navigation properties
        public virtual Match Match { get; set; } = null!;
    }
}
