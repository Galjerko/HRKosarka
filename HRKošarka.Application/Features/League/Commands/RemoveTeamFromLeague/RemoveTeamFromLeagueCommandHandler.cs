using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using HRKošarka.Domain;
using MediatR;

namespace HRKošarka.Application.Features.League.Commands.RemoveTeamFromLeague
{
    public class RemoveTeamFromLeagueCommandHandler : IRequestHandler<RemoveTeamFromLeagueCommand, CommandResponse<bool>>
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly IGenericRepository<LeagueTeam> _leagueTeamRepository;

        public RemoveTeamFromLeagueCommandHandler(
            ILeagueRepository leagueRepository,
            IGenericRepository<LeagueTeam> leagueTeamRepository)
        {
            _leagueRepository = leagueRepository;
            _leagueTeamRepository = leagueTeamRepository;
        }

        public async Task<CommandResponse<bool>> Handle(RemoveTeamFromLeagueCommand request, CancellationToken cancellationToken)
        {
            var existing = await _leagueRepository.GetLeagueTeamAsync(request.LeagueId, request.TeamId, cancellationToken);
            if (existing == null || !existing.IsActive)
                throw new NotFoundException("LeagueTeam", $"{request.LeagueId}/{request.TeamId}");

            existing.IsActive = false;
            await _leagueTeamRepository.UpdateAsync(existing, cancellationToken);

            return CommandResponse<bool>.Success(true, "Team removed from league successfully.");
        }
    }
}
