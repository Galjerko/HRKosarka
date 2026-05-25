using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.RemovePlayerFromTeam
{
    public class RemovePlayerFromTeamCommand : IRequest<Unit>
    {
        public Guid TeamId { get; set; }
        public Guid PlayerId { get; set; }
        public string? RequesterClubId { get; set; }
        public string? RequesterUserId { get; set; }
    }
}
