namespace HRKošarka.Application.Features.League.Queries.GetAvailableTeamsForLeague
{
    public class AvailableTeamForLeagueDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public string AgeCategoryName { get; set; } = string.Empty;
    }
}
