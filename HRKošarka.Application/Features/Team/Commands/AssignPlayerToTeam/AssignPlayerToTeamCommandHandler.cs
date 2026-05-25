using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using HRKošarka.Domain.Helpers;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.AssignPlayerToTeam
{
    public class AssignPlayerToTeamCommandHandler : IRequestHandler<AssignPlayerToTeamCommand, CommandResponse<Guid>>
    {
        private readonly IGenericRepository<PlayerTeamHistory> _historyRepository;
        private readonly IPlayerTeamHistoryRepository _playerTeamHistoryRepository;
        private readonly IGenericRepository<Domain.Season> _seasonRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamRepresentativeRepository _repRepository;

        public AssignPlayerToTeamCommandHandler(
            IGenericRepository<PlayerTeamHistory> historyRepository,
            IPlayerTeamHistoryRepository playerTeamHistoryRepository,
            IGenericRepository<Domain.Season> seasonRepository,
            IPlayerRepository playerRepository,
            ITeamRepository teamRepository,
            ITeamRepresentativeRepository repRepository)
        {
            _historyRepository = historyRepository;
            _playerTeamHistoryRepository = playerTeamHistoryRepository;
            _seasonRepository = seasonRepository;
            _playerRepository = playerRepository;
            _teamRepository = teamRepository;
            _repRepository = repRepository;
        }

        public async Task<CommandResponse<Guid>> Handle(AssignPlayerToTeamCommand request, CancellationToken cancellationToken)
        {
            var validator = new AssignPlayerToTeamCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid assignment data", validationResult);

            var team = await _teamRepository.GetByIdWithIncludesAsync(request.TeamId, cancellationToken);
            if (team == null)
                throw new NotFoundException(nameof(Team), request.TeamId);

            bool isAdmin = string.IsNullOrEmpty(request.RequesterClubId) && string.IsNullOrEmpty(request.RequesterUserId);
            if (!isAdmin)
            {
                bool authorized = !string.IsNullOrEmpty(request.RequesterClubId) && team.ClubId.ToString() == request.RequesterClubId;
                if (!authorized && !string.IsNullOrEmpty(request.RequesterUserId))
                    authorized = await _repRepository.IsActiveRepForTeamAsync(request.RequesterUserId, request.TeamId, cancellationToken);
                if (!authorized)
                    throw new BadRequestException("You are not authorized to manage this team's roster.");
            }

            if (!team.IsActive)
                throw new BadRequestException("Cannot assign a player to an inactive team.");

            var player = await _playerRepository.GetByIdAsync(request.PlayerId, cancellationToken);
            if (player == null)
                throw new NotFoundException(nameof(Player), request.PlayerId);
            if (!player.IsActive)
                throw new BadRequestException("Cannot assign an inactive player to a team.");

            var alreadyInTeam = await _playerRepository.IsAlreadyActiveInTeamAsync(request.PlayerId, request.TeamId, cancellationToken);
            if (alreadyInTeam)
                throw new BadRequestException("Player is already an active member of this team.");

            if (player.Gender != team.Gender)
                throw new BadRequestException($"Player gender does not match the team gender. This is a {(team.Gender == Gender.Male ? "male" : "female")} team.");

            var hasAgeCategoryConflict = await _playerRepository.HasAgeCategoryConflictAsync(request.PlayerId, team.AgeCategoryId, cancellationToken);
            if (hasAgeCategoryConflict)
                throw new BadRequestException("Player is already assigned to a team in this age category. A player can only be in one team per age category.");

            if (!AgeCategoryEligibility.IsEligible(team.AgeCategory.Code, player.DateOfBirth))
                throw new BadRequestException(
                    $"Player's age does not meet the requirements for the {team.AgeCategory.Name} category ({AgeCategoryEligibility.GetAgeRequirementDescription(team.AgeCategory.Code)}).");

            var season = await _seasonRepository.GetByIdAsync(request.SeasonId, cancellationToken);
            if (season == null)
                throw new NotFoundException(nameof(Domain.Season), request.SeasonId);

            if (season.IsCompleted || season.EndDate.Date < DateTime.Now.Date)
                throw new BadRequestException("Cannot assign a player to a season that has already ended.");

            if (request.JoinDate.Date < season.StartDate.Date || request.JoinDate.Date > season.EndDate.Date)
                throw new BadRequestException(
                    $"Join date must be within the season period ({season.StartDate:dd/MM/yyyy} – {season.EndDate:dd/MM/yyyy}).");

            if (request.JerseyNumber.HasValue)
            {
                var isJerseyNumberAvailable = await _playerTeamHistoryRepository.IsJerseyNumberAvailableAsync(
                    request.TeamId,
                    request.JerseyNumber.Value,
                    cancellationToken: cancellationToken);

                if (!isJerseyNumberAvailable)
                    throw new BadRequestException($"Jersey number {request.JerseyNumber.Value} is already taken in this team.");
            }

            var history = new PlayerTeamHistory
            {
                PlayerId = request.PlayerId,
                TeamId = request.TeamId,
                SeasonId = request.SeasonId,
                JoinDate = request.JoinDate,
                JerseyNumber = request.JerseyNumber,
                IsActive = true
            };

            await _historyRepository.CreateAsync(history, cancellationToken);

            return CommandResponse<Guid>.Success(history.Id, "Player assigned to team successfully");
        }
    }
}
