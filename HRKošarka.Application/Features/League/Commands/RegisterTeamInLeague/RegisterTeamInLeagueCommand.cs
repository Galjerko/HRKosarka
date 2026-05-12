using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.RegisterTeamInLeague
{
    public class RegisterTeamInLeagueCommand : IRequest<CommandResponse<Guid>>
    {
        public Guid LeagueId { get; set; }
        public Guid TeamId { get; set; }
        public DateTime RegistrationDate { get; set; } = DateTime.Today;
    }
}
