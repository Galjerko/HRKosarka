namespace HRKošarka.Application.Features.Player.Queries.GetPlayerCareer
{
    public class PlayerCareerItemDTO
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public string SeasonName { get; set; } = string.Empty;
        public int? JerseyNumber { get; set; }
        public DateTime JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public bool IsActive { get; set; }
        public List<PlayerCareerLeagueStatDTO> CompetitionStats { get; set; } = new();
    }
}
