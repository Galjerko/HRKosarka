using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.RemovePlayerFromTeam
{
    public class RemovePlayerFromTeamCommandHandler : IRequestHandler<RemovePlayerFromTeamCommand, Unit>
    {
        private readonly IPlayerTeamHistoryRepository _historyRepository;
        private readonly ITeamRepository _teamRepository;
        private readonly ITeamRepresentativeRepository _repRepository;

        public RemovePlayerFromTeamCommandHandler(
            IPlayerTeamHistoryRepository historyRepository,
            ITeamRepository teamRepository,
            ITeamRepresentativeRepository repRepository)
        {
            _historyRepository = historyRepository;
            _teamRepository = teamRepository;
            _repRepository = repRepository;
        }

        public async Task<Unit> Handle(RemovePlayerFromTeamCommand request, CancellationToken cancellationToken)
        {
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

            var history = await _historyRepository.GetActiveByPlayerAndTeamAsync(
                request.PlayerId, request.TeamId, cancellationToken);

            if (history == null)
                throw new NotFoundException("Active team assignment", request.PlayerId);

            history.LeaveDate = DateTime.Now;
            history.IsActive = false;

            await _historyRepository.UpdateAsync(history, cancellationToken);

            return Unit.Value;
        }
    }
}
