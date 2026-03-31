using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.CreateLeague
{
    public class CreateLeagueCommand : IRequest<CommandResponse<Guid>>
    {
        public string Name { get; set; } = string.Empty;
        public Guid SeasonId { get; set; }
        public Guid AgeCategoryId { get; set; }
        public Gender Gender { get; set; }
        public CompetitionType CompetitionType { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int NumberOfRounds { get; set; } = 1;
        public bool IsActive { get; set; } = true;
        public string? ImageName { get; set; }
        public string? ImageContentType { get; set; }
        public byte[]? ImageBytes { get; set; }
    }
}
