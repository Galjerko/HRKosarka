using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetPlayerDetails
{
    public class GetPlayerDetailsQueryHandler : IRequestHandler<GetPlayerDetailsQuery, QueryResponse<PlayerDetailsDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepository;

        public GetPlayerDetailsQueryHandler(IMapper mapper, IPlayerRepository playerRepository)
        {
            _mapper = mapper;
            _playerRepository = playerRepository;
        }

        public async Task<QueryResponse<PlayerDetailsDTO>> Handle(GetPlayerDetailsQuery request, CancellationToken cancellationToken)
        {
            var player = await _playerRepository.GetByIdAsync(request.Id, cancellationToken);

            if (player == null)
            {
                throw new NotFoundException(nameof(Domain.Player), request.Id);
            }

            var dto = _mapper.Map<PlayerDetailsDTO>(player);
            return QueryResponse<PlayerDetailsDTO>.Success(dto);
        }
    }
}
