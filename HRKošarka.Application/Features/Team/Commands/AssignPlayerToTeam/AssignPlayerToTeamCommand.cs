using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.AssignPlayerToTeam
{
    public class AssignPlayerToTeamCommand : IRequest<CommandResponse<Guid>>
    {
        public Guid TeamId { get; set; }
        public Guid PlayerId { get; set; }
        public Guid SeasonId { get; set; }
        public DateTime JoinDate { get; set; }
        public int? JerseyNumber { get; set; }
        public string? RequesterClubId { get; set; }
        public string? RequesterUserId { get; set; }
    }
}
