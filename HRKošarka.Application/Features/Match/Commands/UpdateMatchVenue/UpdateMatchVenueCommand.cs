using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.UpdateMatchVenue
{
    public class UpdateMatchVenueCommand : IRequest<CommandResponse<bool>>
    {
        public Guid MatchId { get; set; }
        public string? Venue { get; set; }
        public string? RequesterClubId { get; set; }
    }
}
