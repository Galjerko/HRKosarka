using HRKošarka.Domain.Common;
using System.ComponentModel.DataAnnotations;

namespace HRKošarka.Domain
{
    public class Player : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        public string RegistrationNumber { get; set; } = string.Empty;

        [Required]
        public DateTime DateOfBirth { get; set; }

        public int? Height { get; set; } // in cm

        public int? Weight { get; set; } // in kg

        public Position? Position { get; set; }

        [MaxLength(50)]
        public string? Nationality { get; set; }

        public DateTime? DeactivateDate { get; set; }

        public bool IsActive => DeactivateDate == null;

        [MaxLength(255)]
        public string? ImageName { get; set; }

        [MaxLength(100)]
        public string? ImageContentType { get; set; }

        public byte[]? ImageBytes { get; set; }

        public virtual ICollection<PlayerTeamHistory> TeamHistory { get; set; } = new List<PlayerTeamHistory>();
        public virtual ICollection<PlayerMatchStats> MatchStats { get; set; } = new List<PlayerMatchStats>();
        public virtual ICollection<PlayerSeasonStats> SeasonStats { get; set; } = new List<PlayerSeasonStats>();
    }
}
