namespace HRKošarka.Application.Features.Club.Queries.GetClubDetails
{
    public class ClubDetailsDTO : BaseDTO
    {
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? Address { get; set; }
        public string? City { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? Website { get; set; }
        public string? PostalCode { get; set; }
        public int FoundedYear { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public bool IsActive => DeactivateDate == null;
        public byte[]? ImageBytes { get; set; }
        public string? ImageContentType { get; set; }
        public string? ImageName { get; set; }
        public string? VenueName { get; set; }
        public int? VenueCapacity { get; set; }

        public List<TeamInfoDTO> Teams { get; set; } = new List<TeamInfoDTO>();
    }
}
