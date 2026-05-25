using HRKošarka.Application.Features.League.Queries.GetLeagueBreaks;
using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Match.Queries.GetMatchWithStats
{
    public class MatchWithStatsDTO
    {
        public Guid Id { get; set; }
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public int Round { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public Guid HomeTeamClubId { get; set; }
        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public Guid AwayTeamClubId { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public string? QuarterResults { get; set; }
        public MatchStatus Status { get; set; }
        public ResultSubmissionStatus ResultSubmissionStatus { get; set; }
        public bool IsResultConfirmed { get; set; }
        public DateTime ActualScheduledDate { get; set; }
        public string? Venue { get; set; }
        public DateTime LeagueStartDate { get; set; }
        public DateTime LeagueEndDate { get; set; }
        public string? DisputeReason { get; set; }
        public List<LeagueBreakDTO> LeagueBreaks { get; set; } = new();
        public RescheduleRequestDTO? PendingReschedule { get; set; }
        public List<PlayerMatchStatDTO> HomeTeamStats { get; set; } = new();
        public List<PlayerMatchStatDTO> AwayTeamStats { get; set; } = new();
    }

    public class RescheduleRequestDTO
    {
        public Guid Id { get; set; }
        public DateTime ProposedDate { get; set; }
        public string Reason { get; set; } = string.Empty;
        public Guid ProposerClubId { get; set; }
        public bool ProposerIsHome { get; set; }
        public DateTime ExpiresAt { get; set; }
    }

    public class PlayerMatchStatDTO
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public int Points { get; set; }
        public int ThreePointers { get; set; }
        public int Fouls { get; set; }
        public bool DidNotPlay { get; set; }
        public bool StatsEntered { get; set; }
    }
}
