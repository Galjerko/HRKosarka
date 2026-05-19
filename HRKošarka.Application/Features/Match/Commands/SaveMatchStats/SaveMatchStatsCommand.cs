using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.SaveMatchStats
{
    public class SaveMatchStatsCommand : IRequest<CommandResponse<bool>>
    {
        public Guid MatchId { get; set; }
        public Guid TeamId { get; set; }
        public int? HomeScore { get; set; }
        public int? AwayScore { get; set; }
        public List<PlayerStatEntry> PlayerStats { get; set; } = new();
        public string? SubmitterClubId { get; set; }
    }

    public class PlayerStatEntry
    {
        public Guid PlayerId { get; set; }
        public int Points { get; set; }
        public int ThreePointers { get; set; }
        public int Fouls { get; set; }
        public bool DidNotPlay { get; set; }
    }
}
