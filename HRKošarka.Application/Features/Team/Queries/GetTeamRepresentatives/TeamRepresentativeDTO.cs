namespace HRKošarka.Application.Features.Team.Queries.GetTeamRepresentatives
{
    public class TeamRepresentativeDTO
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = string.Empty;
        public string UserEmail { get; set; } = string.Empty;
        public DateTime AssignedDate { get; set; }
        public bool IsActive { get; set; }
    }
}
