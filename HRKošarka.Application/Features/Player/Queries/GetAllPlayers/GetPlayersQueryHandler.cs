using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Player.Queries.GetAllPlayers
{
    public class GetPlayersQueryHandler : IRequestHandler<GetPlayersQuery, PaginatedResponse<PlayerDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IPlayerRepository _playerRepository;
        private readonly IAppLogger<GetPlayersQueryHandler> _logger;

        public GetPlayersQueryHandler(IMapper mapper, IPlayerRepository playerRepository, IAppLogger<GetPlayersQueryHandler> logger)
        {
            _mapper = mapper;
            _playerRepository = playerRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<PlayerDTO>> Handle(GetPlayersQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving players - Page: {Page}, PageSize: {PageSize}",
                request.Page, request.PageSize);

            var paginatedResult = await _playerRepository.GetPagedAsync(request, cancellationToken);

            var mappedData = _mapper.Map<List<PlayerDTO>>(paginatedResult.Data);

            _logger.LogInformation("Successfully retrieved {PlayerCount} players from page {Page} of {TotalPages}",
                mappedData.Count, paginatedResult.Pagination.CurrentPage, paginatedResult.Pagination.TotalPages);

            return PaginatedResponse<PlayerDTO>.Success(
                mappedData,
                paginatedResult.Pagination.CurrentPage,
                paginatedResult.Pagination.PageSize,
                paginatedResult.Pagination.TotalCount,
                $"Retrieved {mappedData.Count} players from page {paginatedResult.Pagination.CurrentPage}"
            );
        }
    }
}
