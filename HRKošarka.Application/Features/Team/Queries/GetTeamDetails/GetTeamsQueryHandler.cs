using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Team.Queries.GetAllTeams
{
    public class GetTeamsQueryHandler : IRequestHandler<GetTeamsQuery, PaginatedResponse<TeamDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ITeamRepository _teamRepository;
        private readonly IAppLogger<GetTeamsQueryHandler> _logger;

        public GetTeamsQueryHandler(IMapper mapper, ITeamRepository teamRepository, IAppLogger<GetTeamsQueryHandler> logger)
        {
            _mapper = mapper;
            _teamRepository = teamRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<TeamDTO>> Handle(GetTeamsQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving teams - Page: {Page}, PageSize: {PageSize}",
                    request.Page, request.PageSize);

                var paginatedResult = await _teamRepository.GetPagedAsync(request);

                var mappedData = _mapper.Map<List<TeamDTO>>(paginatedResult.Data);

                _logger.LogInformation("Successfully retrieved {TeamCount} teams from page {Page} of {TotalPages}",
                     mappedData.Count, paginatedResult.Pagination.CurrentPage, paginatedResult.Pagination.TotalPages);

                return PaginatedResponse<TeamDTO>.Success(
                    mappedData,
                    paginatedResult.Pagination.CurrentPage,
                    paginatedResult.Pagination.PageSize,
                    paginatedResult.Pagination.TotalCount,
                    $"Retrieved {mappedData.Count} teams from page {paginatedResult.Pagination.CurrentPage}"
                );
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>
                {
                    "An error occurred while fetching teams.",
                    ex.Message
                };
                _logger.LogError(ex, "Error retrieving teams");
                return PaginatedResponse<TeamDTO>.Failure("Failed to retrieve teams", errors);
            }
        }
    }
}