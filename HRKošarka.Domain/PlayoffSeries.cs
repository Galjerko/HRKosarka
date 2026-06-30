using System.ComponentModel.DataAnnotations;
using HRKošarka.Domain.Common;

namespace HRKošarka.Domain
{
    public class PlayoffSeries : BaseEntity
    {
        [Required]
        public Guid LeagueId { get; set; }

        [Required]
        [MaxLength(100)]
        public string RoundName { get; set; } = string.Empty; // "Quarter-Final", "Semi-Final", "Final", "3rd Place"

        [Required]
        public int RoundNumber { get; set; } // 1 = earliest round

        [Required]
        public int SeriesNumber { get; set; } // position within round (1-indexed)

        [Required]
        public int WinsNeeded { get; set; } // 2, 3, or 4

        public Guid? HomeTeamId { get; set; } // null = TBD (future round not yet seeded)
        public Guid? AwayTeamId { get; set; } // null = TBD

        public int? HomeSeedNumber { get; set; } // original standing position; filled when HomeTeamId is assigned
        public int? AwaySeedNumber { get; set; } // original standing position; filled when AwayTeamId is assigned

        public Guid? WinnerId { get; set; }
        public bool IsCompleted { get; set; } = false;

        // Fixed bracket wiring — set at generation time, never change
        public Guid? HomeFeederSeriesId { get; set; } // which series provides the Home slot team
        public Guid? AwayFeederSeriesId { get; set; } // which series provides the Away slot team

        public virtual League League { get; set; } = null!;
        public virtual Team? HomeTeam { get; set; }
        public virtual Team? AwayTeam { get; set; }
        public virtual PlayoffSeries? HomeFeederSeries { get; set; }
        public virtual PlayoffSeries? AwayFeederSeries { get; set; }
        public virtual ICollection<Match> Matches { get; set; } = new List<Match>();

        // Computed from Matches — counts wins for the series' designated home/away team
        // regardless of which team hosts individual games (venue alternates each game).
        public int HomeWins => Matches.Count(m =>
            (m.IsResultConfirmed || m.Status == MatchStatus.Forfeit)
            && m.HomeScore.HasValue && m.AwayScore.HasValue
            && ((m.HomeTeamId == HomeTeamId && m.HomeScore.Value > m.AwayScore.Value)
             || (m.AwayTeamId == HomeTeamId && m.AwayScore.Value > m.HomeScore.Value)));

        public int AwayWins => Matches.Count(m =>
            (m.IsResultConfirmed || m.Status == MatchStatus.Forfeit)
            && m.HomeScore.HasValue && m.AwayScore.HasValue
            && ((m.HomeTeamId == AwayTeamId && m.HomeScore.Value > m.AwayScore.Value)
             || (m.AwayTeamId == AwayTeamId && m.AwayScore.Value > m.HomeScore.Value)));
    }
}
