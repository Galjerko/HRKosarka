namespace HRKošarka.Application.Features.UserFavoriteTeam.Queries.GetMyFavoriteTeams
{
    public class FavoriteTeamDTO
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public bool TeamIsActive { get; set; }
    }
}
