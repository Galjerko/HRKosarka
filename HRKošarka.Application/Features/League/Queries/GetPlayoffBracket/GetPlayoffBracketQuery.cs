using HRKošarka.Application.Models.Responses;
using MediatR;

namespace HRKošarka.Application.Features.League.Queries.GetPlayoffBracket
{
    public class GetPlayoffBracketQuery : IRequest<QueryResponse<PlayoffBracketDTO>>
    {
        public Guid LeagueId { get; set; }

        public GetPlayoffBracketQuery(Guid leagueId)
        {
            LeagueId = leagueId;
        }
    }
}
