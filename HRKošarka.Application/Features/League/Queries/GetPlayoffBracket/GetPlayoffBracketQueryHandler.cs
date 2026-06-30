using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetPlayoffBracket
{
    public class GetPlayoffBracketQueryHandler : IRequestHandler<GetPlayoffBracketQuery, QueryResponse<PlayoffBracketDTO>>
    {
        private readonly ILeagueRepository _leagueRepository;
        private readonly IPlayoffRepository _playoffRepository;

        public GetPlayoffBracketQueryHandler(ILeagueRepository leagueRepository, IPlayoffRepository playoffRepository)
        {
            _leagueRepository = leagueRepository;
            _playoffRepository = playoffRepository;
        }

        public async Task<QueryResponse<PlayoffBracketDTO>> Handle(GetPlayoffBracketQuery request, CancellationToken ct)
        {
            var league = await _leagueRepository.GetByIdAsync(request.LeagueId, ct)
                ?? throw new NotFoundException(nameof(Domain.League), request.LeagueId);

            if (!league.HasPlayoff)
                throw new BadRequestException("This league is not configured for a playoff.");

            var bracket = await _playoffRepository.GetPlayoffBracketAsync(request.LeagueId, ct);

            return QueryResponse<PlayoffBracketDTO>.Success(bracket);
        }
    }
}
