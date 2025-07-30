using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.DeleteTeam
{
    public class DeleteTeamCommandHandler : IRequestHandler<DeleteTeamCommand, Unit>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly IAppLogger<DeleteTeamCommandHandler> _logger;

        public DeleteTeamCommandHandler(ITeamRepository teamRepository, IAppLogger<DeleteTeamCommandHandler> logger)
        {
            _teamRepository = teamRepository;
            _logger = logger;
        }

        public async Task<Unit> Handle(DeleteTeamCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Attempting to delete team with ID: {Id}", request.Id);

            var teamToDelete = await _teamRepository.GetByIdAsync(request.Id);

            if (teamToDelete == null)
            {
                _logger.LogWarning("Team with ID {Id} not found", request.Id);
                throw new NotFoundException(nameof(Team), request.Id);
            }

            if (teamToDelete.DateDeleted != null)
            {
                _logger.LogInformation("Team with ID {Id} is already deleted", request.Id);
                throw new BadRequestException("Team is already deleted");
            }

            await _teamRepository.DeleteAsync(teamToDelete.Id);

            _logger.LogInformation("Successfully deleted team with ID: {Id}", request.Id);

            return Unit.Value;
        }
    }
}