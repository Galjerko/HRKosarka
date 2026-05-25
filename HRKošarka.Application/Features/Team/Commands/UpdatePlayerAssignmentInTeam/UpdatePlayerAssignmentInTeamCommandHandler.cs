using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.UpdatePlayerAssignmentInTeam
{
    public class UpdatePlayerAssignmentInTeamCommandHandler
        : IRequestHandler<UpdatePlayerAssignmentInTeamCommand, CommandResponse<bool>>
    {
        private readonly IPlayerTeamHistoryRepository _playerTeamHistoryRepository;
        private readonly IGenericRepository<Domain.Season> _seasonRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamRepresentativeRepository _repRepository;

        public UpdatePlayerAssignmentInTeamCommandHandler(
            IPlayerTeamHistoryRepository playerTeamHistoryRepository,
            IGenericRepository<Domain.Season> seasonRepository,
            ITeamRepository teamRepository,
            ITeamRepresentativeRepository repRepository)
        {
            _playerTeamHistoryRepository = playerTeamHistoryRepository;
            _seasonRepository = seasonRepository;
            _teamRepository = teamRepository;
            _repRepository = repRepository;
        }

        public async Task<CommandResponse<bool>> Handle(
            UpdatePlayerAssignmentInTeamCommand request,
            CancellationToken cancellationToken)
        {
            var validator = new UpdatePlayerAssignmentInTeamCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid assignment data", validationResult);

            bool isAdmin = string.IsNullOrEmpty(request.RequesterClubId) && string.IsNullOrEmpty(request.RequesterUserId);
            if (!isAdmin)
            {
                var team = await _teamRepository.GetByIdAsync(request.TeamId, cancellationToken)
                    ?? throw new NotFoundException(nameof(Domain.Team), request.TeamId);
                bool authorized = !string.IsNullOrEmpty(request.RequesterClubId) && team.ClubId.ToString() == request.RequesterClubId;
                if (!authorized && !string.IsNullOrEmpty(request.RequesterUserId))
                    authorized = await _repRepository.IsActiveRepForTeamAsync(request.RequesterUserId, request.TeamId, cancellationToken);
                if (!authorized)
                    throw new BadRequestException("You are not authorized to manage this team's roster.");
            }

            var assignment = await _playerTeamHistoryRepository.GetActiveByPlayerAndTeamAsync(
                request.PlayerId,
                request.TeamId,
                cancellationToken);

            if (assignment == null)
                throw new NotFoundException("Active player assignment", $"{request.TeamId}:{request.PlayerId}");

            var season = await _seasonRepository.GetByIdAsync(assignment.SeasonId, cancellationToken);
            if (season != null && (season.IsCompleted || season.EndDate.Date < DateTime.Now.Date))
                throw new BadRequestException("Cannot update an assignment that belongs to a season that has already ended.");

            if (request.JerseyNumber.HasValue)
            {
                var isJerseyAvailable = await _playerTeamHistoryRepository.IsJerseyNumberAvailableAsync(
                    request.TeamId,
                    request.JerseyNumber.Value,
                    assignment.Id,
                    cancellationToken);

                if (!isJerseyAvailable)
                    throw new BadRequestException($"Jersey number {request.JerseyNumber.Value} is already taken in this team.");
            }

            assignment.JerseyNumber = request.JerseyNumber;
            await _playerTeamHistoryRepository.UpdateAsync(assignment, cancellationToken);

            return CommandResponse<bool>.Success(true, "Player assignment updated successfully.");
        }
    }
}
