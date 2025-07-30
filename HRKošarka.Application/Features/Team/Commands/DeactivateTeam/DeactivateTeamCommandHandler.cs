using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.DeactivateTeam
{
    public class DeactivateTeamCommandHandler : IRequestHandler<DeactivateTeamCommand, Unit>
    {
        private readonly ITeamRepository _teamRepository;
        private IAppLogger<DeactivateTeamCommandHandler> _logger;

        public DeactivateTeamCommandHandler(ITeamRepository teamRepository, IAppLogger<DeactivateTeamCommandHandler> logger)
        {
            _teamRepository = teamRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeactivateTeamCommand request, CancellationToken cancellationToken)
        {
            var teamToDeactivate = await _teamRepository.GetByIdAsync(request.Id);

            if (teamToDeactivate == null)
            {
                _logger.LogWarning("Team with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Domain.Team), request.Id);
            }

            if (teamToDeactivate.IsActive == false)
            {
                _logger.LogInformation("Team with ID {Id} is already deactivated", request.Id);
                throw new BadRequestException("Team is already deactivated");
            }

            teamToDeactivate.DeactivateDate = DateTime.Now;
            await _teamRepository.UpdateAsync(teamToDeactivate);

            _logger.LogInformation("Team {TeamName} deactivated at {DeactivateDate}", teamToDeactivate.Name, teamToDeactivate.DeactivateDate);

            return Unit.Value;
        }
    }
}