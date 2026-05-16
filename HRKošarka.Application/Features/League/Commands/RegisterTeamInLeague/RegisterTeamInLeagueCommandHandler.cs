using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.RegisterTeamInLeague
{
    public class RegisterTeamInLeagueCommandHandler : IRequestHandler<RegisterTeamInLeagueCommand, CommandResponse<Guid>>
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly IGenericRepository<LeagueTeam> _leagueTeamRepository;

        public RegisterTeamInLeagueCommandHandler(
            ILeagueRepository leagueRepository,
            ITeamRepository teamRepository,
            IGenericRepository<LeagueTeam> leagueTeamRepository)
        {
            _leagueRepository = leagueRepository;
            _teamRepository = teamRepository;
            _leagueTeamRepository = leagueTeamRepository;
        }

        public async Task<CommandResponse<Guid>> Handle(RegisterTeamInLeagueCommand request, CancellationToken cancellationToken)
        {
            var validator = new RegisterTeamInLeagueCommandValidator();
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
                throw new BadRequestException("Invalid registration data", validationResult);

            var league = await _leagueRepository.GetByIdWithIncludesAsync(request.LeagueId, cancellationToken);
            if (league == null)
                throw new NotFoundException(nameof(League), request.LeagueId);
            if (!league.IsActive)
                throw new BadRequestException("Cannot register a team in an inactive league.");
            if (league.ScheduleGenerated)
                throw new BadRequestException("Schedule has already been generated. Team registration is locked.");

            var team = await _teamRepository.GetByIdWithIncludesAsync(request.TeamId, cancellationToken);
            if (team == null)
                throw new NotFoundException(nameof(Team), request.TeamId);
            if (!team.IsActive)
                throw new BadRequestException("Cannot register an inactive team.");

            if (team.Gender != league.Gender)
                throw new BadRequestException($"Team gender does not match league gender. This is a {league.Gender} league.");

            if (team.AgeCategoryId != league.AgeCategoryId)
                throw new BadRequestException("Team age category does not match the league age category.");

            var existing = await _leagueRepository.GetLeagueTeamAsync(request.LeagueId, request.TeamId, cancellationToken);
            if (existing != null)
            {
                if (existing.IsActive)
                    throw new BadRequestException("Team is already registered in this league.");

                existing.IsActive = true;
                existing.RegistrationDate = request.RegistrationDate;
                await _leagueTeamRepository.UpdateAsync(existing, cancellationToken);
                return CommandResponse<Guid>.Success(existing.Id, "Team registered in league successfully.");
            }

            var leagueTeam = new LeagueTeam
            {
                LeagueId = request.LeagueId,
                TeamId = request.TeamId,
                RegistrationDate = request.RegistrationDate,
                IsActive = true
            };

            await _leagueTeamRepository.CreateAsync(leagueTeam, cancellationToken);
            return CommandResponse<Guid>.Success(leagueTeam.Id, "Team registered in league successfully.");
        }
    }
}
