using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Player.Commands.CreatePlayer
{
    public class CreatePlayerCommand : IRequest<CommandResponse<Guid>>
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
        public string? ImageName { get; set; }
        public string? ImageContentType { get; set; }
        public byte[]? ImageBytes { get; set; }
    }
}
