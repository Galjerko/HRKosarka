using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.User.Queries.GetInactiveUsers
{
    public class GetInactiveUsersQueryHandler
        : IRequestHandler<GetInactiveUsersQuery, PaginatedResponse<InactiveUserDTO>>
    {
        private readonly IUserService _userService;
        private readonly IAppLogger<GetInactiveUsersQueryHandler> _logger;

        public GetInactiveUsersQueryHandler(IUserService userService, IAppLogger<GetInactiveUsersQueryHandler> logger)
        {
            _logger = logger;
            _userService = userService;
        }

        public async Task<PaginatedResponse<InactiveUserDTO>> Handle(GetInactiveUsersQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Retrieving inactive users - Page: {Page}, PageSize: {PageSize}",
                    request.Page, request.PageSize);

                var result = await _userService.GeInactiveUsersPagedAsync(request);

                _logger.LogInformation(
                    "Successfully retrieved {UserCount} users from page {Page} of {TotalPages}",
                    result.Data.Count,
                    result.Pagination.CurrentPage,
                    result.Pagination.TotalPages);

                return result;
            }
            catch (Exception ex)
            {
                var errors = new List<string>
            {
                "An error occurred while fetching inactive users.",
                ex.Message
            };

                _logger.LogError(ex, "Error retrieving inactive users");

                return PaginatedResponse<InactiveUserDTO>.Failure(
                    "Failed to retrieve inactive users",
                    errors);
            }
        }
    }
}