using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Team.Queries.GetAvailableTeamsForPlayer
{
    public class AvailableTeamDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string ClubName { get; set; } = string.Empty;
        public string AgeCategoryName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
    }
}
