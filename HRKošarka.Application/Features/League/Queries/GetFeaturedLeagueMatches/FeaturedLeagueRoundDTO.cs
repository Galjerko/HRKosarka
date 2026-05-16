using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.League.Queries.GetFeaturedLeagueMatches
{
    public class FeaturedLeagueRoundDTO
    {
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public byte[]? LeagueImageBytes { get; set; }
        public string? LeagueImageContentType { get; set; }
        public int RoundNumber { get; set; }
        public string RoundName { get; set; } = string.Empty;
        public List<FeaturedMatchDTO> Matches { get; set; } = new();
    }

    public class FeaturedMatchDTO
    {
        public Guid Id { get; set; }
        public Guid HomeTeamId { get; set; }
        public string HomeTeamName { get; set; } = string.Empty;
        public byte[]? HomeTeamLogoBytes { get; set; }
        public string? HomeTeamLogoContentType { get; set; }
        public Guid AwayTeamId { get; set; }
        public string AwayTeamName { get; set; } = string.Empty;
        public byte[]? AwayTeamLogoBytes { get; set; }
        public string? AwayTeamLogoContentType { get; set; }
        public DateTime ActualScheduledDate { get; set; }
        public MatchStatus Status { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
    }
}
