using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.LeaveType.Queries.GetAllLeaveTypes
{
    public class GetLeaveTypesQueryHandler : IRequestHandler<GetLeaveTypesQuery, PaginatedResponse<LeaveTypeDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ILeaveTypeRepository _leaveTypeRepository;
        private readonly IAppLogger<GetLeaveTypesQueryHandler> _logger;

        public GetLeaveTypesQueryHandler(IMapper mapper, ILeaveTypeRepository leaveTypeRepository, IAppLogger<GetLeaveTypesQueryHandler> logger)
        {
            _mapper = mapper;
            _leaveTypeRepository = leaveTypeRepository;
            _logger = logger;
        }

        public async Task<PaginatedResponse<LeaveTypeDTO>> Handle(GetLeaveTypesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving leave types - Page: {Page}, PageSize: {PageSize}",
                    request.Page, request.PageSize);

                var paginatedResult = await _leaveTypeRepository.GetPagedAsync(request);

                var mappedData = _mapper.Map<List<LeaveTypeDTO>>(paginatedResult.Data);

                return PaginatedResponse<LeaveTypeDTO>.Success(
                    mappedData,
                    paginatedResult.Pagination.CurrentPage,
                    paginatedResult.Pagination.PageSize,
                    paginatedResult.Pagination.TotalCount,
                    $"Retrieved {mappedData.Count} leave types from page {paginatedResult.Pagination.CurrentPage}"
                );
            }
            catch (Exception ex)
            {
                List<string> errors = new List<string>
                {
                    "An error occurred while deleting the leave type.",
                    ex.Message
                };
                _logger.LogError(ex, "Error retrieving leave types");
                return PaginatedResponse<LeaveTypeDTO>.Failure("Failed to retrieve leave types", errors);
            }
        }

    }

}
