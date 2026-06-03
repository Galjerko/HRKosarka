using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;
using DomainTeam = HRKošarka.Domain.Team;
using FavoriteEntity = HRKošarka.Domain.UserFavoriteTeam;

namespace HRKošarka.Application.Features.UserFavoriteTeam.Commands.ToggleFavoriteTeam
{
    public class ToggleFavoriteTeamCommandHandler
        : IRequestHandler<ToggleFavoriteTeamCommand, CommandResponse<bool>>
    {
        private readonly IUserFavoriteTeamRepository _favoriteRepository;
        private readonly ITeamRepository _teamRepository;

        public ToggleFavoriteTeamCommandHandler(
            IUserFavoriteTeamRepository favoriteRepository,
            ITeamRepository teamRepository)
        {
            _favoriteRepository = favoriteRepository;
            _teamRepository = teamRepository;
        }

        public async Task<CommandResponse<bool>> Handle(
            ToggleFavoriteTeamCommand request, CancellationToken ct)
        {
            var team = await _teamRepository.GetByIdAsync(request.TeamId, ct)
                ?? throw new NotFoundException(nameof(DomainTeam), request.TeamId);

            var existing = await _favoriteRepository.GetByUserAndTeamAsync(request.UserId, request.TeamId, ct);

            if (existing != null && existing.DateDeleted == null)
            {
                await _favoriteRepository.DeleteAsync(existing.Id, ct);
                return CommandResponse<bool>.Success(false, "Team unfollowed.");
            }

            if (existing != null)
            {
                // Restore soft-deleted record
                existing.DateDeleted = null;
                existing.DeletedBy = null;
                await _favoriteRepository.UpdateAsync(existing, ct);
                return CommandResponse<bool>.Success(true, "Team followed.");
            }

            var favorite = new FavoriteEntity
            {
                UserId = request.UserId,
                TeamId = request.TeamId,
                NotifyByEmail = true
            };
            await _favoriteRepository.CreateAsync(favorite, ct);
            return CommandResponse<bool>.Success(true, "Team followed.");
        }
    }
}
