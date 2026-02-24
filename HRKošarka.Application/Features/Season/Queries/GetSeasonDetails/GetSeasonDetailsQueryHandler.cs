using AutoMapper;
using HRKošarka.Application.Contracts.Logging;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.Season.Queries.GetSeasonDetails
{
    public class GetSeasonDetailsQueryHandler : IRequestHandler<GetSeasonDetailsQuery, QueryResponse<SeasonDetailsDTO>>
    {
        private readonly IMapper _mapper;
        private readonly IGenericRepository<Domain.Season> _seasonRepository;
        private readonly IAppLogger<GetSeasonDetailsQueryHandler> _logger;

        public GetSeasonDetailsQueryHandler(
            IMapper mapper,
            IGenericRepository<Domain.Season> seasonRepository,
            IAppLogger<GetSeasonDetailsQueryHandler> logger)
        {
            _mapper = mapper;
            _seasonRepository = seasonRepository;
            _logger = logger;
        }

        public async Task<QueryResponse<SeasonDetailsDTO>> Handle(
            GetSeasonDetailsQuery request, CancellationToken cancellationToken)
        {
            var season = await _seasonRepository.GetByIdAsync(request.Id, cancellationToken)
                ?? throw new NotFoundException(nameof(Domain.Season), request.Id);

            var dto = _mapper.Map<SeasonDetailsDTO>(season);
            return QueryResponse<SeasonDetailsDTO>.Success(dto, "Season retrieved successfully.");
        }
    }
}