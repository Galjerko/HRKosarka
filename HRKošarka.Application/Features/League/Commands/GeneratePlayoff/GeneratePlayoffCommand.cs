using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.GeneratePlayoff
{
    public class GeneratePlayoffCommand : IRequest<CommandResponse<bool>>
    {
        public Guid LeagueId { get; set; }

        public DateTime PlayoffStartDate { get; set; }

        // Index 0 = Round 1 (QF for 8 teams / SF for 4 teams / Final for 2 teams), etc.
        public List<int> RoundWinsNeeded { get; set; } = new();

        public bool Include3rdPlace { get; set; } = false;
    }
}
