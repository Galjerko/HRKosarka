using FluentValidation;
using HRKošarka.Application.Contracts.Persistence;

namespace HRKošarka.Application.Features.Team.Commands.UpdateTeam
{
    public class UpdateTeamCommandValidator : AbstractValidator<UpdateTeamCommand>
    {
        private readonly ITeamRepository _teamRepository;

        public UpdateTeamCommandValidator(ITeamRepository teamRepository)
        {
            _teamRepository = teamRepository;

            RuleFor(t => t.Id)
                .NotEmpty().WithMessage("Team ID is required.")
                .MustAsync(TeamMustExist).WithMessage("Team not found.")
                .MustAsync(TeamIsNotDeactivatedOrDeleted).WithMessage("Team is already deactivated or deleted.");

            RuleFor(t => t.Name)
                .NotEmpty().WithMessage("Name is required.")
                .MaximumLength(150).WithMessage("Name must not exceed 150 characters.");

            RuleFor(t => t)
                .MustAsync(TeamNameUniqueInSameContext)
                .WithMessage("Team with that name already exists in this club for this age category");
        }

        private async Task<bool> TeamMustExist(Guid id, CancellationToken token)
        {
            var team = await _teamRepository.GetByIdAsync(id, token);
            return team != null;
        }

        private async Task<bool> TeamIsNotDeactivatedOrDeleted(Guid id, CancellationToken token)
        {
            var team = await _teamRepository.GetByIdAsync(id, token);
            return team != null && team.IsActive && !team.DateDeleted.HasValue;
        }

        private async Task<bool> TeamNameUniqueInSameContext(UpdateTeamCommand command, CancellationToken token)
        {
            var existingTeam = await _teamRepository.GetByIdAsync(command.Id, token);
            if (existingTeam == null) return false;

            return await _teamRepository.IsTeamNameUniqueInClub(
                command.Name,
                existingTeam.ClubId,
                existingTeam.AgeCategoryId,
                command.Id,
                token);
        }
    }
}
