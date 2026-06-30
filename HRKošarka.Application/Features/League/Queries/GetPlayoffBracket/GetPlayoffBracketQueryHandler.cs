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

            var allSeries = await _playoffRepository.GetAllSeriesForLeagueAsync(request.LeagueId, ct);

            var rounds = allSeries
                .GroupBy(s => s.RoundNumber)
                .OrderBy(g => g.Key)
                .Select(g => new PlayoffRoundDTO
                {
                    RoundNumber = g.Key,
                    RoundName = g.First().RoundName,
                    Series = g.OrderBy(s => Math.Min(s.HomeSeedNumber ?? int.MaxValue, s.AwaySeedNumber ?? int.MaxValue))
                              .ThenBy(s => s.SeriesNumber)
                              .Select(s =>
                    {
                        var winnerName = s.WinnerId.HasValue
                            ? (s.WinnerId == s.HomeTeamId ? s.HomeTeam?.Name : s.AwayTeam?.Name)
                            : null;

                        return new PlayoffSeriesDTO
                        {
                            SeriesId = s.Id,
                            SeriesNumber = s.SeriesNumber,
                            HomeTeamId = s.HomeTeamId,
                            HomeTeamName = s.HomeTeam?.Name,
                            AwayTeamId = s.AwayTeamId,
                            AwayTeamName = s.AwayTeam?.Name,
                            HomeSeedNumber = s.HomeSeedNumber,
                            AwaySeedNumber = s.AwaySeedNumber,
                            WinsNeeded = s.WinsNeeded,
                            HomeWins = s.HomeWins,
                            AwayWins = s.AwayWins,
                            IsCompleted = s.IsCompleted,
                            WinnerId = s.WinnerId,
                            WinnerName = winnerName,
                            // Match home/away alternates per game venue pattern, but scores are
                            // reported in series-Home/series-Away order so the columns stay
                            // consistent with the series header across all games.
                            Matches = s.Matches
                                .OrderBy(m => m.DefaultScheduledDate)
                                .Select((m, idx) => new PlayoffMatchSlimDTO
                                {
                                    MatchId = m.Id,
                                    GameNumber = idx + 1,
                                    ScheduledDate = m.ActualScheduledDate,
                                    Status = m.Status,
                                    HomeScore = m.HomeTeamId == s.HomeTeamId ? m.HomeScore : m.AwayScore,
                                    AwayScore = m.HomeTeamId == s.HomeTeamId ? m.AwayScore : m.HomeScore,
                                    IsResultConfirmed = m.IsResultConfirmed,
                                    Venue = m.VenueOverride ?? m.HomeTeam?.Club?.VenueName
                                }).ToList()
                        };
                    }).ToList()
                }).ToList();

            return QueryResponse<PlayoffBracketDTO>.Success(new PlayoffBracketDTO
            {
                LeagueId = request.LeagueId,
                Rounds = rounds
            });
        }
    }
}
