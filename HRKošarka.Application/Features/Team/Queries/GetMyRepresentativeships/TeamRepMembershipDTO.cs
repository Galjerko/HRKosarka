namespace HRKošarka.Application.Features.Team.Queries.GetMyRepresentativeships
{
    public class TeamRepMembershipDTO
    {
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public bool TeamIsActive { get; set; }
    }
}
