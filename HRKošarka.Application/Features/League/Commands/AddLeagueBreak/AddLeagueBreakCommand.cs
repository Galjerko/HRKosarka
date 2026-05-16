using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.AddLeagueBreak
{
    public class AddLeagueBreakCommand : IRequest<CommandResponse<Guid>>
    {
        public Guid LeagueId { get; set; }
        public string Name { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
