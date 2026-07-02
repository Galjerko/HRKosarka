using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Application.Services;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.AssignTeamRepresentative
{
    public class AssignTeamRepresentativeCommandHandler
        : IRequestHandler<AssignTeamRepresentativeCommand, CommandResponse<Guid>>
    {
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly EmailNotificationService _emailNotificationService;

        public AssignTeamRepresentativeCommandHandler(
            ITeamRepresentativeRepository repRepository,
            ITeamRepository teamRepository,
            EmailNotificationService emailNotificationService)
        {
            _repRepository = repRepository;
            _teamRepository = teamRepository;
            _emailNotificationService = emailNotificationService;
        }

        public async Task<CommandResponse<Guid>> Handle(
            AssignTeamRepresentativeCommand request, CancellationToken ct)
        {
            var validationResult = await new AssignTeamRepresentativeCommandValidator().ValidateAsync(request, ct);
            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid data.", validationResult);

            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct)
                ?? throw new NotFoundException(nameof(Team), request.TeamId);

            if (!team.IsActive)
                throw new BadRequestException("Cannot assign a representative to an inactive team.");

            Guid repId;
            string resultMessage;

            var existing = await _repRepository.GetByUserAndTeamAsync(request.UserId, request.TeamId, ct);
            if (existing != null)
            {
                if (existing.IsActive)
                    throw new BadRequestException("This user is already a representative for this team.");

                existing.DeactivateDate = null;
                existing.AssignedDate = DateTime.Now;
                await _repRepository.UpdateAsync(existing, ct);
                repId = existing.Id;
                resultMessage = "Team representative reactivated.";
            }
            else
            {
                var rep = new TeamRepresentative
                {
                    TeamId = request.TeamId,
                    UserId = request.UserId,
                    AssignedDate = DateTime.Now
                };
                await _repRepository.CreateAsync(rep, ct);
                repId = rep.Id;
                resultMessage = "Team representative assigned successfully.";
            }

            await _emailNotificationService.SendNotificationAsync(
                new[] { request.UserId },
                NotificationType.RepresentativeAssigned,
                "You have been assigned as a team representative",
                $"You have been assigned as a representative for {team.Name}.",
                matchId: null,
                linkPath: $"/teams/{team.Id}",
                linkText: "View team",
                ct: ct);

            return CommandResponse<Guid>.Success(repId, resultMessage);
        }
    }
}
