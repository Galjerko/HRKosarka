using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Application.Services;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.RevokeTeamRepresentative
{
    public class RevokeTeamRepresentativeCommandHandler
        : IRequestHandler<RevokeTeamRepresentativeCommand, CommandResponse<bool>>
    {
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public RevokeTeamRepresentativeCommandHandler(
            ITeamRepresentativeRepository repRepository,
            ITeamRepository teamRepository,
            EmailNotificationService emailNotificationService)
        {
            _repRepository = repRepository;
            _teamRepository = teamRepository;
            _emailNotificationService = emailNotificationService;
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

            var team = await _teamRepository.GetByIdAsync(rep.TeamId, ct);
            await _emailNotificationService.SendNotificationAsync(
                new[] { rep.UserId },
                NotificationType.RepresentativeRevoked,
                "Your team representative role has been removed",
                $"Your representative role for {team?.Name ?? "your team"} has been removed.",
                matchId: null,
                linkPath: $"/teams/{request.TeamId}",
                linkText: "View team",
                ct: ct);

            return CommandResponse<bool>.Success(true, "Team representative revoked.");
        }
    }
}
