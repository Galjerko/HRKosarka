using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerDetails
{
    public class PlayerDetailsDTO : BaseDTO
    {
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public Position? Position { get; set; }
        public Gender Gender { get; set; }
        public string? Nationality { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public bool IsActive => DeactivateDate == null;
        public byte[]? ImageBytes { get; set; }
        public string? ImageContentType { get; set; }
        public string? ImageName { get; set; }
    }
}
