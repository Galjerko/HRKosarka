using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.UpdateTeam
{
    public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, Unit>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IAppLogger<UpdateTeamCommandHandler> _logger;

        public UpdateTeamCommandHandler(ITeamRepository teamRepository, IAppLogger<UpdateTeamCommandHandler> logger)
        {
            _teamRepository = teamRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(UpdateTeamCommand request, CancellationToken cancellationToken)
        {
            var validator = new UpdateTeamCommandValidator(_teamRepository);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);

            if (!validationResult.IsValid)
            {
                _logger.LogWarning("Validation errors in update request for {0} - {1}", nameof(Domain.Team), request.Id);
                throw new BadRequestException("Invalid Team", validationResult);
            }

            var teamToUpdate = await _teamRepository.GetByIdAsync(request.Id, cancellationToken);
            if (teamToUpdate == null)
            {
                throw new NotFoundException(nameof(Domain.Team), request.Id);
            }

            // Only update the name
            teamToUpdate.Name = request.Name;
            teamToUpdate.DateModified = DateTime.UtcNow;

            await _teamRepository.UpdateAsync(teamToUpdate, cancellationToken);

            _logger.LogInformation("Team {TeamName} (ID: {TeamId}) successfully updated", teamToUpdate.Name, teamToUpdate.Id);

            return Unit.Value;
        }
    }
}
