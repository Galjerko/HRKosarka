using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.DeactivateTeam
{
    public class DeactivateTeamCommandHandler : IRequestHandler<DeactivateTeamCommand, Unit>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IPlayerTeamHistoryRepository _historyRepository;
        private readonly ILeagueRepository _leagueRepository;
        private readonly IAppLogger<DeactivateTeamCommandHandler> _logger;

        public DeactivateTeamCommandHandler(
            ITeamRepository teamRepository,
            IPlayerTeamHistoryRepository historyRepository,
            ILeagueRepository leagueRepository,
            IAppLogger<DeactivateTeamCommandHandler> logger)
        {
            _teamRepository = teamRepository;
            _historyRepository = historyRepository;
            _leagueRepository = leagueRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeactivateTeamCommand request, CancellationToken cancellationToken)
        {
            var teamToDeactivate = await _teamRepository.GetByIdAsync(request.Id, cancellationToken);

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

            if (await _leagueRepository.HasActiveMatchesForTeamAsync(request.Id, cancellationToken))
                throw new BadRequestException("Cannot deactivate a team that has scheduled matches. Complete or cancel the matches first.");

            await _historyRepository.DeactivateAllForTeamAsync(request.Id, cancellationToken);
            await _leagueRepository.DeactivateAllForTeamAsync(request.Id, cancellationToken);

            teamToDeactivate.DeactivateDate = DateTime.Now;
            await _teamRepository.UpdateAsync(teamToDeactivate, cancellationToken);

            _logger.LogInformation("Team {TeamName} deactivated at {DeactivateDate}", teamToDeactivate.Name, teamToDeactivate.DeactivateDate);

            return Unit.Value;
        }
    }
}