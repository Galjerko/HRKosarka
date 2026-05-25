using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Match.Commands.UpdateMatchVenue
{
    public class UpdateMatchVenueCommandHandler : IRequestHandler<UpdateMatchVenueCommand, CommandResponse<bool>>
    {
        private readonly IMatchRepository _matchRepository;
        private readonly ITeamRepresentativeRepository _repRepository;

        public UpdateMatchVenueCommandHandler(
            IMatchRepository matchRepository,
            ITeamRepresentativeRepository repRepository)
        {
            _matchRepository = matchRepository;
            _repRepository = repRepository;
        }

        public async Task<CommandResponse<bool>> Handle(UpdateMatchVenueCommand request, CancellationToken ct)
        {
            var match = await _matchRepository.GetByIdWithIncludesAsync(request.MatchId, ct)
                ?? throw new NotFoundException("Match", request.MatchId);

            if (match.IsResultConfirmed)
                throw new BadRequestException("Cannot change venue of a confirmed match.");

            bool isAdmin = string.IsNullOrEmpty(request.RequesterClubId) && string.IsNullOrEmpty(request.RequesterUserId);
            if (!isAdmin)
            {
                bool authorized = false;
                if (!string.IsNullOrEmpty(request.RequesterClubId))
                    authorized = match.HomeTeam.ClubId.ToString() == request.RequesterClubId;
                if (!authorized && !string.IsNullOrEmpty(request.RequesterUserId))
                    authorized = await _repRepository.IsActiveRepForTeamAsync(request.RequesterUserId, match.HomeTeamId, ct);
                if (!authorized)
                    throw new BadRequestException("Only the home team's manager or representative can set the venue.");
            }

            match.VenueOverride = string.IsNullOrWhiteSpace(request.Venue) ? null : request.Venue.Trim();
            await _matchRepository.UpdateAsync(match, ct);

            return CommandResponse<bool>.Success(true, "Venue updated.");
        }
    }
}
