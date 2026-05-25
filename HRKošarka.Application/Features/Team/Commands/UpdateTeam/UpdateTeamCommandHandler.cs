using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.UpdateTeam
{
    public class UpdateTeamCommandHandler : IRequestHandler<UpdateTeamCommand, Unit>
    {
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamRepresentativeRepository _repRepository;
        private readonly IAppLogger<UpdateTeamCommandHandler> _logger;

        public UpdateTeamCommandHandler(
            ITeamRepository teamRepository,
            ITeamRepresentativeRepository repRepository,
            IAppLogger<UpdateTeamCommandHandler> logger)
        {
            _teamRepository = teamRepository;
            _repRepository = repRepository;
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
                throw new NotFoundException(nameof(Domain.Team), request.Id);

            bool isAdmin = string.IsNullOrEmpty(request.RequesterClubId) && string.IsNullOrEmpty(request.RequesterUserId);
            if (!isAdmin)
            {
                bool authorized = !string.IsNullOrEmpty(request.RequesterClubId) && teamToUpdate.ClubId.ToString() == request.RequesterClubId;
                if (!authorized && !string.IsNullOrEmpty(request.RequesterUserId))
                    authorized = await _repRepository.IsActiveRepForTeamAsync(request.RequesterUserId, request.Id, cancellationToken);
                if (!authorized)
                    throw new BadRequestException("You are not authorized to manage this team.");
            }

            // Only update the name
            teamToUpdate.Name = request.Name;
            teamToUpdate.DateModified = DateTime.Now;

            await _teamRepository.UpdateAsync(teamToUpdate, cancellationToken);

            _logger.LogInformation("Team {TeamName} (ID: {TeamId}) successfully updated", teamToUpdate.Name, teamToUpdate.Id);

            return Unit.Value;
        }
    }
}
