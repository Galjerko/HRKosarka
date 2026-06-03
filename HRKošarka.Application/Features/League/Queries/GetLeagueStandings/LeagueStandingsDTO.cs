namespace HRKošarka.Application.Features.League.Queries.GetLeagueStandings
{
    public class LeagueStandingsDTO
    {
        public List<TeamStandingRowDTO> Standings { get; set; } = new();
        public LeagueLeadersDTO? Leaders { get; set; }
    }

    public class TeamStandingRowDTO
    {
        public int Position { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointsFor { get; set; }
        public int PointsAgainst { get; set; }
        public int PointsDifference { get; set; }
        public int LeaguePoints { get; set; }
        public List<string> Last5 { get; set; } = new();
        public bool HasPlayed { get; set; }
    }

    public class LeagueLeadersDTO
    {
        public List<LeaderEntryDTO> TopScorers { get; set; } = new();
        public List<LeaderEntryDTO> TopThreePointers { get; set; } = new();
        public List<LeaderEntryDTO> TopFoulMakers { get; set; } = new();
    }

    public class LeaderEntryDTO
    {
        public Guid PlayerId { get; set; }
        public string PlayerName { get; set; } = string.Empty;
        public string TeamName { get; set; } = string.Empty;
        public decimal Value { get; set; }
        public int GamesPlayed { get; set; }
    }

    public class CompletedMatchSlimDTO
    {
        public Guid HomeTeamId { get; set; }
        public Guid AwayTeamId { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
    }
}
