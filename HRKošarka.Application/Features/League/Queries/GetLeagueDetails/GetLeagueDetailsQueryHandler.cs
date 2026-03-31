using AutoMapper;
using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetLeagueDetails
{
    public class GetLeagueDetailsQueryHandler : IRequestHandler<GetLeagueDetailsQuery, QueryResponse<LeagueDetailsDTO>>
    {
        private readonly IMapper _mapper;
        private readonly ILeagueRepository _leagueRepository;

        public GetLeagueDetailsQueryHandler(IMapper mapper, ILeagueRepository leagueRepository)
        {
            _mapper = mapper;
            _leagueRepository = leagueRepository;
        }

        public async Task<QueryResponse<LeagueDetailsDTO>> Handle(
            GetLeagueDetailsQuery request, CancellationToken cancellationToken)
        {
            var league = await _leagueRepository.GetLeagueWithDetailsAsync(request.Id, cancellationToken);

            if (league == null)
            {
                throw new NotFoundException(nameof(League), request.Id);
            }

            var dto = _mapper.Map<LeagueDetailsDTO>(league);
            dto.SeasonName = league.Season?.Name ?? string.Empty;
            dto.AgeCategoryCode = league.AgeCategory?.Code ?? string.Empty;

            return QueryResponse<LeagueDetailsDTO>.Success(dto);
        }
    }
}
