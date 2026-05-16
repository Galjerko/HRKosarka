using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueSchedule
{
    public class LeagueRoundDTO
    {
        public int Round { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public List<LeagueMatchDTO> Matches { get; set; } = new();
    }

    public class LeagueMatchDTO
    {
        public Guid Id { get; set; }
        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public DateTime DefaultScheduledDate { get; set; }
        public DateTime ActualScheduledDate { get; set; }
        public MatchStatus Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? Venue { get; set; }
    }
}
