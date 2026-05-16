namespace HRKošarka.Application.Features.League.Queries.GetLeagueTeams
{
    public class LeagueTeamDTO
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public Guid ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public byte[]? ClubImageBytes { get; set; }
        public string? ClubImageContentType { get; set; }
        public string AgeCategoryName { get; set; } = string.Empty;
        public DateTime RegistrationDate { get; set; }
    }
}
