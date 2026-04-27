using HRKošarka.Domain.Common;

namespace HRKošarka.Application.Features.Team.Queries.GetTeamRoster
{
    public class TeamRosterPlayerDTO
    {
        public Guid Id { get; set; }
        public Guid PlayerId { get; set; }
        public string PlayerFirstName { get; set; } = string.Empty;
        public string PlayerLastName { get; set; } = string.Empty;
        public string RegistrationNumber { get; set; } = string.Empty;
        public DateTime DateOfBirth { get; set; }
        public Position? Position { get; set; }
        public int? JerseyNumber { get; set; }
        public DateTime JoinDate { get; set; }
        public Guid SeasonId { get; set; }
        public string SeasonName { get; set; } = string.Empty;
    }
}
