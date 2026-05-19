using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerAssignments
{
    public class PlayerAssignmentDTO
    {
        public Guid Id { get; set; }
        public Guid TeamId { get; set; }
        public string TeamName { get; set; } = string.Empty;
        public Guid ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public string AgeCategoryName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public Guid SeasonId { get; set; }
        public string SeasonName { get; set; } = string.Empty;
        public DateTime JoinDate { get; set; }
        public DateTime? LeaveDate { get; set; }
        public int? JerseyNumber { get; set; }
        public bool IsActive { get; set; }
    }
}
