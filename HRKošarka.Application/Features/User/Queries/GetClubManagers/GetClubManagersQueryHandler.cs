using AutoMapper;
using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.User.Queries.GetClubManagers
{
    public class GetClubManagersQueryHandler : IRequestHandler<GetClubManagersQuery, PaginatedResponse<ClubManagerDTO>>
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;
        private readonly IAppLogger<GetClubManagersQueryHandler> _logger;

        public GetClubManagersQueryHandler(
            IUserService userService,
            IMapper mapper,
            IAppLogger<GetClubManagersQueryHandler> logger)
        {
            _userService = userService;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<PaginatedResponse<ClubManagerDTO>> Handle(GetClubManagersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving club managers - Page: {Page}, PageSize: {PageSize}",
                    request.Page, request.PageSize);

                var clubManagers = await _userService.GetClubManagers();

                var query = clubManagers.AsQueryable();

                if (!string.IsNullOrWhiteSpace(request.SearchTerm))
                {
                    var searchTerm = request.SearchTerm.ToLower();
                    query = query.Where(u =>
                        u.UserName.ToLower().Contains(searchTerm) ||
                        u.Email.ToLower().Contains(searchTerm) ||
                        u.FirstName.ToLower().Contains(searchTerm) ||
                        u.LastName.ToLower().Contains(searchTerm)
                    );
                }
                var totalCount = query.Count();

                var paginatedUsers = query
                    .Skip((request.Page - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                var mappedData = _mapper.Map<List<ClubManagerDTO>>(paginatedUsers);

                _logger.LogInformation("Successfully retrieved {ManagerCount} club managers from page {Page}",
                    mappedData.Count, request.Page);

                return PaginatedResponse<ClubManagerDTO>.Success(
                    mappedData,
                    request.Page,
                    request.PageSize,
                    totalCount,
                    $"Retrieved {mappedData.Count} club managers"
                );
            }
            catch (Exception ex)
            {
                var errors = new List<string>
                {
                    "An error occurred while fetching club managers.",
                    ex.Message
                };
                _logger.LogError(ex, "Error retrieving club managers");
                return PaginatedResponse<ClubManagerDTO>.Failure("Failed to retrieve club managers", errors);
            }
        }
    }
}
