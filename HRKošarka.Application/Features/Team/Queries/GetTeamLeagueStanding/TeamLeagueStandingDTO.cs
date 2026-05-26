namespace HRKošarka.Application.Features.Team.Queries.GetTeamLeagueStanding
{
    public class TeamLeagueStandingDTO
    {
        public int Position { get; set; }
        public int GamesPlayed { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int PointsFor { get; set; }
        public int PointsAgainst { get; set; }
        public int PointsDifference { get; set; }
        public int LeaguePoints { get; set; }
    }
}
