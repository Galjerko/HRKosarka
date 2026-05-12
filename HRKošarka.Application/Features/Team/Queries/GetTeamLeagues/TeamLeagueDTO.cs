using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamLeagues
{
    public class TeamLeagueDTO
    {
        public Guid LeagueId { get; set; }
        public string LeagueName { get; set; } = string.Empty;
        public string SeasonName { get; set; } = string.Empty;
        public string AgeCategoryName { get; set; } = string.Empty;
        public Gender Gender { get; set; }
        public CompetitionType CompetitionType { get; set; }
        public DateTime RegistrationDate { get; set; }
        public bool IsLeagueActive { get; set; }
    }
}
