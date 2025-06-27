using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRKošarka.Domain
{
    public class Club : BaseEntity
    {
        [MaxLength(150)]
        public string Name { get; set; } = string.Empty;

        [MaxLength(1000)] 
        public string? Description { get; set; }

        [MaxLength(200)]
        public string? Address { get; set; }

        [MaxLength(100)]
        public string? City { get; set; }

        [MaxLength(20)] 
        public string? PhoneNumber { get; set; }

        [MaxLength(255)] 
        public string? Email { get; set; }

        [MaxLength(255)]  
        public string? Website { get; set; }

        [MaxLength(20)] 
        public string? PostalCode { get; set; }

        public DateTime FoundedYear { get; set; }

        public DateTime? DeactivateDate { get; set; }

        public bool IsActive => DeactivateDate == null;

        [MaxLength(500)]
        public string? LogoUrl { get; set; }
    }
}
