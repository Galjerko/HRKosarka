using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.League.Queries.GetPlayoffBracket
{
    public class PlayoffBracketDTO
    {
        public Guid LeagueId { get; set; }
        public List<PlayoffRoundDTO> Rounds { get; set; } = new();
    }

    public class PlayoffRoundDTO
    {
        public int RoundNumber { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public List<PlayoffSeriesDTO> Series { get; set; } = new();
    }

    public class PlayoffSeriesDTO
    {
        public Guid SeriesId { get; set; }
        public int SeriesNumber { get; set; }
        public Guid? HomeTeamId { get; set; }
        public string? HomeTeamName { get; set; }   // null = TBD
        public Guid? AwayTeamId { get; set; }
        public string? AwayTeamName { get; set; }   // null = TBD
        public int? HomeSeedNumber { get; set; }
        public int? AwaySeedNumber { get; set; }
        public int WinsNeeded { get; set; }
        public int HomeWins { get; set; }
        public int AwayWins { get; set; }
        public bool IsCompleted { get; set; }
        public Guid? WinnerId { get; set; }
        public string? WinnerName { get; set; }
        public List<PlayoffMatchSlimDTO> Matches { get; set; } = new();
    }

    public class PlayoffMatchSlimDTO
    {
        public Guid MatchId { get; set; }
        public int GameNumber { get; set; }         // 1-based index within series
        public DateTime ScheduledDate { get; set; }
        public MatchStatus Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public bool IsResultConfirmed { get; set; }
        public string? Venue { get; set; }
    }
}
