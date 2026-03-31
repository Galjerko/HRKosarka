using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.League.Queries.GetAllLeagues
{
    public class LeagueDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Guid SeasonId { get; set; }
        public string SeasonName { get; set; } = string.Empty;
        public Guid AgeCategoryId { get; set; }
        public string AgeCategoryCode { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public CompetitionType CompetitionType { get; set; }
        public int NumberOfRounds { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public bool IsActive => DeactivateDate == null;
    }
}
