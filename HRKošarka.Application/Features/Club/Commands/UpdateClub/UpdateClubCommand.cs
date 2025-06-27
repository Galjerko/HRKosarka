using MediatR;

namespace HRKošarka.Application.Features.Club.Commands.UpdateClub
{
    public class UpdateClubCommand : IRequest<Unit>
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? PostalCode { get; set; }
        public DateTime FoundedYear { get; set; }
        public string? LogoUrl { get; set; }
    }
}
