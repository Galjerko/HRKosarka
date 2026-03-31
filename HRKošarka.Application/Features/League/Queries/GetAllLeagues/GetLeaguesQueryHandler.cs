using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetAllLeagues
{
    public class GetLeaguesQueryHandler : IRequestHandler<GetLeaguesQuery, PaginatedResponse<LeagueDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ILeagueRepository _leagueRepository;
        private readonly IAppLogger<GetLeaguesQueryHandler> _logger;

        public GetLeaguesQueryHandler(
            IMapper mapper,
            ILeagueRepository leagueRepository,
            IAppLogger<GetLeaguesQueryHandler> logger)
        {
            _mapper = mapper;
            _leagueRepository = leagueRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<LeagueDTO>> Handle(
            GetLeaguesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving leagues - Page: {Page}, PageSize: {PageSize}",
                    request.Page, request.PageSize);

                var paginatedResult = await _leagueRepository.GetPagedWithIncludesAsync(request, cancellationToken);

                var mappedData = _mapper.Map<List<LeagueDTO>>(paginatedResult.Data);

                _logger.LogInformation("Successfully retrieved {Count} leagues from page {Page} of {TotalPages}",
                    mappedData.Count, paginatedResult.Pagination.CurrentPage, paginatedResult.Pagination.TotalPages);

                return PaginatedResponse<LeagueDTO>.Success(
                    mappedData,
                    paginatedResult.Pagination.CurrentPage,
                    paginatedResult.Pagination.PageSize,
                    paginatedResult.Pagination.TotalCount,
                    $"Retrieved {mappedData.Count} leagues from page {paginatedResult.Pagination.CurrentPage}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving leagues");
                return PaginatedResponse<LeagueDTO>.Failure(
                    "Failed to retrieve leagues",
                    new List<string> { "An error occurred while fetching leagues.", ex.Message });
            }
        }
    }
}
