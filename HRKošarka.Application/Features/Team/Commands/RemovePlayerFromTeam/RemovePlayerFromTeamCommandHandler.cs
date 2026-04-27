using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using MediatR;

namespace HRKošarka.Application.Features.Team.Commands.RemovePlayerFromTeam
{
    public class RemovePlayerFromTeamCommandHandler : IRequestHandler<RemovePlayerFromTeamCommand, Unit>
    {
        private readonly IPlayerTeamHistoryRepository _historyRepository;

        public RemovePlayerFromTeamCommandHandler(IPlayerTeamHistoryRepository historyRepository)
        {
            _historyRepository = historyRepository;
        }

        public async Task<Unit> Handle(RemovePlayerFromTeamCommand request, CancellationToken cancellationToken)
        {
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
