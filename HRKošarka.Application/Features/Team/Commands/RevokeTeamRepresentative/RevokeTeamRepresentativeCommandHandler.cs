using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.RevokeTeamRepresentative
{
    public class RevokeTeamRepresentativeCommandHandler
        : IRequestHandler<RevokeTeamRepresentativeCommand, CommandResponse<bool>>
    {
        private readonly ITeamRepresentativeRepository _repRepository;

        public RevokeTeamRepresentativeCommandHandler(ITeamRepresentativeRepository repRepository)
        {
            _repRepository = repRepository;
        }

        public async Task<CommandResponse<bool>> Handle(
            RevokeTeamRepresentativeCommand request, CancellationToken ct)
        {
            var rep = await _repRepository.GetByIdAsync(request.RepresentativeId, ct)
                ?? throw new NotFoundException("TeamRepresentative", request.RepresentativeId);

            if (rep.TeamId != request.TeamId)
                throw new BadRequestException("Representative does not belong to this team.");

            if (!rep.IsActive)
                throw new BadRequestException("This representative is already revoked.");

            rep.DeactivateDate = DateTime.Now;
            await _repRepository.UpdateAsync(rep, ct);
            return CommandResponse<bool>.Success(true, "Team representative revoked.");
        }
    }
}
