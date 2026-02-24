using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Features.Team.Commands.DeactivateTeam;
using MediatR;


namespace HRKošarka.Application.Features.Team.Commands.ActivateTeam
{
    internal class ActivateTeamCommandHandler : IRequestHandler<ActivateTeamCommand, Unit>
    {
        private readonly ITeamRepository _teamRepository;
        private IAppLogger<DeactivateTeamCommandHandler> _logger;

        public ActivateTeamCommandHandler(ITeamRepository teamRepository, IAppLogger<DeactivateTeamCommandHandler> logger)
        {
            _teamRepository = teamRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(ActivateTeamCommand request, CancellationToken cancellationToken)
        {
            var teamToActivate = await _teamRepository.GetByIdAsync(request.Id, cancellationToken);

            if (teamToActivate == null)
            {
                _logger.LogWarning("Team with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Domain.Team), request.Id);
            }

            if (teamToActivate.IsActive == true)
            {
                _logger.LogInformation("Team with ID {Id} is already activated", request.Id);
                throw new BadRequestException("Team is already activated");
            }

            teamToActivate.DeactivateDate = null;
            await _teamRepository.UpdateAsync(teamToActivate, cancellationToken);

            _logger.LogInformation("Team {TeamName} activated at {DeactivateDate}", teamToActivate.Name, DateTime.Now);

            return Unit.Value;
        }
    }
}
