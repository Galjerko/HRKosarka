using HRKošarka.Application.Contracts.Identity;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.User.Queries.GetNonAdminUsers
{
    public class GetNonAdminUsersQueryHandler
        : IRequestHandler<GetNonAdminUsersQuery, PaginatedResponse<NonAdminUserDTO>>
    {
        private readonly IUserService _userService;
        private readonly IAppLogger<GetNonAdminUsersQueryHandler> _logger;

        public GetNonAdminUsersQueryHandler(
            IUserService userService,
            IAppLogger<GetNonAdminUsersQueryHandler> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        public async Task<PaginatedResponse<NonAdminUserDTO>> Handle(
            GetNonAdminUsersQuery request,
            CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation(
                    "Retrieving non-admin users - Page: {Page}, PageSize: {PageSize}",
                    request.Page, request.PageSize);

                var result = await _userService.GetNonAdminUsersPagedAsync(request);

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
                "An error occurred while fetching users.",
                ex.Message
            };

                _logger.LogError(ex, "Error retrieving non-admin users");

                return PaginatedResponse<NonAdminUserDTO>.Failure(
                    "Failed to retrieve users",
                    errors);
            }
        }
    }

}
