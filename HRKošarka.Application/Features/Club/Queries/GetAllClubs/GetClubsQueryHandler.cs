using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Club.Queries.GetAllClubs
{
    public class GetClubsQueryHandler : IRequestHandler<GetClubsQuery, PaginatedResponse<ClubDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IClubRepository _clubRepository;
        private readonly IAppLogger<GetClubsQueryHandler> _logger;

        public GetClubsQueryHandler(IMapper mapper, IClubRepository clubRepository, IAppLogger<GetClubsQueryHandler> logger)
        {
            _mapper = mapper;
            _clubRepository = clubRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ClubDTO>> Handle(GetClubsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving clubs - Page: {Page}, PageSize: {PageSize}",
                    request.Page, request.PageSize);

                var paginatedResult = await _clubRepository.GetPagedAsync(request, cancellationToken);

                var mappedData = _mapper.Map<List<ClubDTO>>(paginatedResult.Data);

                _logger.LogInformation("Successfully retrieved {ClubCount} clubs from page {Page} of {TotalPages}",
                     mappedData.Count, paginatedResult.Pagination.CurrentPage, paginatedResult.Pagination.TotalPages);

                return PaginatedResponse<ClubDTO>.Success(
                    mappedData,
                    paginatedResult.Pagination.CurrentPage,
                    paginatedResult.Pagination.PageSize,
                    paginatedResult.Pagination.TotalCount,
                    $"Retrieved {mappedData.Count} clubs from page {paginatedResult.Pagination.CurrentPage}"
                );
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>
                {
                    "An error occurred while fetching clubs.",
                    ex.Message
                };
                _logger.LogError(ex, "Error retrieving clubs");
                return PaginatedResponse<ClubDTO>.Failure("Failed to retrieve clubs", errors);
            }
        }
    }
}
