using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.AssignTeamRepresentative
{
    public class AssignTeamRepresentativeCommandHandler
        : IRequestHandler<AssignTeamRepresentativeCommand, CommandResponse<Guid>>
    {
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly ITeamRepository _teamRepository;

        public AssignTeamRepresentativeCommandHandler(
            ITeamRepresentativeRepository repRepository,
            ITeamRepository teamRepository)
        {
            _repRepository = repRepository;
            _teamRepository = teamRepository;
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

            var existing = await _repRepository.GetByUserAndTeamAsync(request.UserId, request.TeamId, ct);
            if (existing != null)
            {
                if (existing.IsActive)
                    throw new BadRequestException("This user is already a representative for this team.");

                existing.DeactivateDate = null;
                existing.AssignedDate = DateTime.Now;
                await _repRepository.UpdateAsync(existing, ct);
                return CommandResponse<Guid>.Success(existing.Id, "Team representative reactivated.");
            }

            var rep = new TeamRepresentative
            {
                TeamId = request.TeamId,
                UserId = request.UserId,
                AssignedDate = DateTime.Now
            };
            await _repRepository.CreateAsync(rep, ct);
            return CommandResponse<Guid>.Success(rep.Id, "Team representative assigned successfully.");
        }
    }
}
