using HRKošarka.Application.Models.Responses;
using MediatR;
using Swashbuckle.AspNetCore.Annotations;

namespace HRKošarka.Application.Features.Club.Commands.CreateClub
{
    public class CreateClubCommand : IRequest<CommandResponse<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? PostalCode { get; set; }

        [SwaggerSchema(Format = "date")]
        public DateTime FoundedYear { get; set; }
        public string? LogoUrl { get; set; }
    }
}

