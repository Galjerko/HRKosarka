using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueDetails
{
    public class LeagueDetailsDTO : BaseDTO
    {
        public string Name { get; set; } = string.Empty;
        public Guid SeasonId { get; set; }
        public string SeasonName { get; set; } = string.Empty;
        public Guid AgeCategoryId { get; set; }
        public string AgeCategoryCode { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public CompetitionType CompetitionType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfRounds { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public bool IsActive => DeactivateDate == null;
        public string? ImageName { get; set; }
        public string? ImageContentType { get; set; }
        public byte[]? ImageBytes { get; set; }
    }
}
