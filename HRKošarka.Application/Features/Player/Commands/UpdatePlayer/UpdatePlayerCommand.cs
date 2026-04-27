using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.UpdatePlayer
{
    public class UpdatePlayerCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public int? Height { get; set; }
        public int? Weight { get; set; }
        public Position? Position { get; set; }
        public string? Nationality { get; set; }
        public string? ImageName { get; set; }
        public string? ImageContentType { get; set; }
        public byte[]? ImageBytes { get; set; }
    }
}
