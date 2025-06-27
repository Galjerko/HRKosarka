namespace HRKošarka.Application.Features.Club.Queries.GetAllClubs
{
    public class ClubDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? City { get; set; }
        public DateTime FoundedYear { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public bool IsActive => DeactivateDate == null;
        public string? LogoUrl { get; set; }

    }
}
