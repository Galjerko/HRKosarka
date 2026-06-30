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
        public bool ScheduleGenerated { get; set; }
        public bool IsFeatured { get; set; }
        public bool HasPlayoff { get; set; }
        public int? PlayoffTeamCount { get; set; }
        public bool PlayoffGenerated { get; set; }
        public bool PlayoffHas3rdPlace { get; set; }
        public DateTime? PlayoffEndDate { get; set; }
        public bool AllRegularMatchesComplete { get; set; }
        public string? DefaultVenue { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public bool IsActive => DeactivateDate == null;
        public string? ImageName { get; set; }
        public string? ImageContentType { get; set; }
        public byte[]? ImageBytes { get; set; }
    }
}
