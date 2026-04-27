using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Player.Queries.GetAvailablePlayers
{
    public class AvailablePlayerDTO
    {
        public Guid Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public Position? Position { get; set; }
        public Gender Gender { get; set; }
    }
}
