using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Season.Queries.GetAllSeasons
{
    public class GetSeasonsQueryHandler : IRequestHandler<GetSeasonsQuery, PaginatedResponse<SeasonDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Domain.Season> _seasonRepository;
        private readonly IAppLogger<GetSeasonsQueryHandler> _logger;

        public GetSeasonsQueryHandler(
            IMapper mapper,
            IGenericRepository<Domain.Season> seasonRepository,
            IAppLogger<GetSeasonsQueryHandler> logger)
        {
            _mapper = mapper;
            _seasonRepository = seasonRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<SeasonDTO>> Handle(
            GetSeasonsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving seasons - Page: {Page}, PageSize: {PageSize}",
                    request.Page, request.PageSize);

                var paginatedResult = await _seasonRepository.GetPagedAsync(request, cancellationToken);
                var mappedData = _mapper.Map<List<SeasonDTO>>(paginatedResult.Data);

                _logger.LogInformation("Successfully retrieved {Count} seasons from page {Page}",
                    mappedData.Count, paginatedResult.Pagination.CurrentPage);

                return PaginatedResponse<SeasonDTO>.Success(
                    mappedData,
                    paginatedResult.Pagination.CurrentPage,
                    paginatedResult.Pagination.PageSize,
                    paginatedResult.Pagination.TotalCount,
                    $"Retrieved {mappedData.Count} seasons from page {paginatedResult.Pagination.CurrentPage}"
                );
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving seasons");
                return PaginatedResponse<SeasonDTO>.Failure("Failed to retrieve seasons",
                    new List<string> { ex.Message });
            }
        }
    }
}