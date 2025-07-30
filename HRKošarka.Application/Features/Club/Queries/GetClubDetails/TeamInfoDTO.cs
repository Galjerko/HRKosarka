using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Club.Queries.GetClubDetails
{
    public class TeamInfoDTO
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public Guid AgeCategoryId { get; set; }
        public string AgeCategoryName { get; set; } = string.Empty;
        public string AgeCategoryCode { get; set; } = string.Empty;
        public bool IsActive { get; set; }
    }
}
