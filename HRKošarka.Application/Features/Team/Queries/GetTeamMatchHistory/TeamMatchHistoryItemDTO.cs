using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamMatchHistory
{
    public class TeamMatchHistoryItemDTO
    {
        public Guid MatchId { get; set; }
        public int Round { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public string SeasonName { get; set; } = string.Empty;
        public CompetitionType CompetitionType { get; set; }
        public bool IsHome { get; set; }
        public Guid OpponentTeamId { get; set; }
        public string OpponentTeamName { get; set; } = string.Empty;
        public DateTime ScheduledDate { get; set; }
        public MatchStatus Status { get; set; }
        public int? TeamScore { get; set; }
        public int? OpponentScore { get; set; }
        public string? Venue { get; set; }
    }
}
