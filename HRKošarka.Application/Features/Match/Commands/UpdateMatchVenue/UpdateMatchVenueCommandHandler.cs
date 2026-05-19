using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.UpdateMatchVenue
{
    public class UpdateMatchVenueCommandHandler : IRequestHandler<UpdateMatchVenueCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;

        public UpdateMatchVenueCommandHandler(IMatchRepository matchRepository)
        {
            _matchRepository = matchRepository;
        }

        public async Task<CommandResponse<bool>> Handle(UpdateMatchVenueCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Cannot change venue of a confirmed match.");

            if (!string.IsNullOrEmpty(request.RequesterClubId) &&
                match.HomeTeam.ClubId.ToString() != request.RequesterClubId)
                throw new BadRequestException("Only the home team's club manager can set the venue.");

            match.VenueOverride = string.IsNullOrWhiteSpace(request.Venue) ? null : request.Venue.Trim();
            await _matchRepository.UpdateAsync(match, ct);

            return CommandResponse<bool>.Success(true, "Venue updated.");
        }
    }
}
