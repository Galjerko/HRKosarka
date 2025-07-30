using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.AgeCategory.Queries.GetAllAgeCategories
{
    public class GetAllAgeCategoriesQueryHandler : IRequestHandler<GetAllAgeCategoriesQuery, QueryResponse<List<AgeCategoryDTO>>>
    {
        private readonly IMapper _mapper;
        private readonly IAgeCategoryRepository _ageCategoryRepository;
        private readonly IAppLogger<GetAllAgeCategoriesQueryHandler> _logger;

        public GetAllAgeCategoriesQueryHandler(IMapper mapper, IAgeCategoryRepository ageCategoryRepository, IAppLogger<GetAllAgeCategoriesQueryHandler> logger)
        {
            _mapper = mapper;
            _ageCategoryRepository = ageCategoryRepository;
            _logger = logger;
        }

        public async Task<QueryResponse<List<AgeCategoryDTO>>> Handle(GetAllAgeCategoriesQuery request, CancellationToken cancellationToken)
        {
            try
            {
                _logger.LogInformation("Retrieving all age categories");

                var ageCategories = await _ageCategoryRepository.GetAsync();
                var ageCategoryDtos = _mapper.Map<List<AgeCategoryDTO>>(ageCategories);

                _logger.LogInformation("Successfully retrieved {Count} age categories", ageCategoryDtos.Count);

                return QueryResponse<List<AgeCategoryDTO>>.Success(ageCategoryDtos);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving age categories");
                return QueryResponse<List<AgeCategoryDTO>>.Failure("Failed to retrieve age categories");
            }
        }
    }
}