using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetAvailablePlayers
{
    public class GetAvailablePlayersQueryHandler
        : IRequestHandler<GetAvailablePlayersQuery, QueryResponse<List<AvailablePlayerDTO>>>
    {
        private readonly IPlayerRepository _playerRepository;

        public GetAvailablePlayersQueryHandler(IPlayerRepository playerRepository)
        {
            _playerRepository = playerRepository;
        }

        public async Task<QueryResponse<List<AvailablePlayerDTO>>> Handle(
            GetAvailablePlayersQuery request, CancellationToken cancellationToken)
        {
            var players = await _playerRepository.GetAvailablePlayersAsync(request.TeamId, request.SearchTerm, cancellationToken);
            return QueryResponse<List<AvailablePlayerDTO>>.Success(players);
        }
    }
}
