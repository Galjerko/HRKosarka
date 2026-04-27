using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Player.Queries.GetAllPlayers
{
    public class PlayerDTO
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public Position? Position { get; set; }
        public Gender Gender { get; set; }
        public string? Nationality { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public bool IsActive => DeactivateDate == null;
    }
}
