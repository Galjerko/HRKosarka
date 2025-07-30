using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamDetails
{
    public class TeamDetailsDTO : BaseDTO
    {
        public string Name { get; set; } = string.Empty;
        public Guid ClubId { get; set; }
        public string ClubName { get; set; } = string.Empty;
        public Guid AgeCategoryId { get; set; }
        public string AgeCategoryName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public DateTime? DeactivateDate { get; set; }
        public bool IsActive => DeactivateDate == null;
    }
}
