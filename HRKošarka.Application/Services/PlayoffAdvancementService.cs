using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Application.Exceptions;
using HRKošarka.Domain;
using HRKošarka.Domain.Common;
using HRKošarka.Domain.Helpers;

namespace HRKošarka.Application.Services
{
    public class PlayoffAdvancementService
    {
        private readonly IPlayoffRepository _playoffRepository;
        private readonly ILeagueRepository _leagueRepository;

        public PlayoffAdvancementService(IPlayoffRepository playoffRepository, ILeagueRepository leagueRepository)
        {
            _playoffRepository = playoffRepository;
            _leagueRepository = leagueRepository;
        }

        public async Task AdvanceIfCompleteAsync(Match confirmedMatch, CancellationToken ct)
        {
            if (!confirmedMatch.PlayoffSeriesId.HasValue)
                return;

            var series = await _playoffRepository.GetSeriesWithMatchesAsync(confirmedMatch.PlayoffSeriesId.Value, ct);
            if (series == null || series.IsCompleted)
                return;

            // Order by scheduled date to derive each game's sequential game number
            var orderedMatches = series.Matches.OrderBy(m => m.DefaultScheduledDate).ToList();
            var confirmedGameIndex = orderedMatches.FindIndex(m => m.Id == confirmedMatch.Id);
            if (confirmedGameIndex < 0)
                return; // unexpected state

            var confirmedGameNumber = confirmedGameIndex + 1;

            var league = await _leagueRepository.GetLeagueWithDetailsAsync(series.LeagueId, ct);
            var capDate = (league!.PlayoffEndDate ?? league.EndDate).Date.AddHours(19);

            int homeWins = series.HomeWins;
            int awayWins = series.AwayWins;

            if (homeWins >= series.WinsNeeded || awayWins >= series.WinsNeeded)
            {
                bool homeWon = homeWins >= series.WinsNeeded;
                series.IsCompleted = true;
                series.WinnerId = homeWon ? series.HomeTeamId : series.AwayTeamId;
                var winnerId = series.WinnerId;
                var winnerSeed = homeWon ? series.HomeSeedNumber!.Value : series.AwaySeedNumber!.Value;
                var loserId = homeWon ? series.AwayTeamId : series.HomeTeamId;
                var loserSeed = homeWon ? series.AwaySeedNumber!.Value : series.HomeSeedNumber!.Value;

                // A completed series can feed both a next-round stub AND a 3rd-place stub
                var nextStubs = await FindNextRoundStubsAsync(series, ct);
                var stubsToActivate = new List<PlayoffSeries>();
                var matchesToCreate = new List<Match>();

                foreach (var stub in nextStubs)
                {
                    if (stub.IsCompleted)
                        continue;

                    bool isHomeSlot = stub.HomeFeederSeriesId == series.Id;
                    bool isThirdPlace = stub.RoundName == "3rd Place";
                    var advancingTeamId = isThirdPlace ? loserId : winnerId;
                    var advancingSeed = isThirdPlace ? loserSeed : winnerSeed;

                    if (isHomeSlot)
                    {
                        stub.HomeTeamId = advancingTeamId;
                        stub.HomeSeedNumber = advancingSeed;
                    }
                    else
                    {
                        stub.AwayTeamId = advancingTeamId;
                        stub.AwaySeedNumber = advancingSeed;
                    }

                    stubsToActivate.Add(stub);

                    // Both slots now filled — assign home court by seed and pre-generate WinsNeeded games
                    if (stub.HomeTeamId.HasValue && stub.AwayTeamId.HasValue)
                    {
                        // Lower seed number = home court for entire series
                        if (stub.HomeSeedNumber > stub.AwaySeedNumber)
                        {
                            (stub.HomeTeamId, stub.AwayTeamId) = (stub.AwayTeamId, stub.HomeTeamId);
                            (stub.HomeSeedNumber, stub.AwaySeedNumber) = (stub.AwaySeedNumber, stub.HomeSeedNumber);
                        }

                        // First game of next round: 3 days after the last confirmed game of the completing series
                        // (series.Matches includes all pre-generated games; Max gives the latest scheduled date)
                        var lastMatchDate = series.Matches.Max(m => m.DefaultScheduledDate);
                        var nextRoundGame1Date = lastMatchDate.AddDays(3);

                        var gameSlots = PlayoffSeriesScheduler.GenerateInitialGames(
                            stub.HomeTeamId!.Value, stub.AwayTeamId!.Value,
                            stub.WinsNeeded, nextRoundGame1Date, stub.WinsNeeded);

                        foreach (var slot in gameSlots)
                        {
                            if (slot.Date > capDate)
                                throw new BadRequestException(
                                    $"Playoff scheduling would exceed the configured end date ({capDate:dd.MM.yyyy}). " +
                                    "Adjust the playoff end date.");

                            matchesToCreate.Add(new Match
                            {
                                LeagueId = stub.LeagueId,
                                HomeTeamId = slot.HomeTeamId,
                                AwayTeamId = slot.AwayTeamId,
                                Round = stub.RoundNumber,
                                RoundName = stub.RoundName,
                                DefaultScheduledDate = slot.Date,
                                ActualScheduledDate = slot.Date,
                                Status = MatchStatus.Scheduled,
                                SchedulingStatus = SchedulingStatus.Default,
                                LastSchedulingUpdate = DateTime.Now,
                                VenueOverride = league.DefaultVenue,
                                PlayoffSeriesId = stub.Id
                            });
                        }
                    }
                }

                await _playoffRepository.UpdateSeriesAndActivateNextAsync(series, stubsToActivate, matchesToCreate, ct);
            }
            else
            {
                // Series continues — the next game may already be pre-generated
                if (confirmedGameNumber < orderedMatches.Count)
                    // Next game exists (pre-generated); nothing to persist
                    return;

                // Series went beyond its pre-generated minimum — create the next game on-the-fly
                var lastMatch = orderedMatches.Last();
                var slot = PlayoffSeriesScheduler.GenerateNextGame(
                    series.HomeTeamId!.Value, series.AwayTeamId!.Value,
                    series.WinsNeeded, orderedMatches.Count, lastMatch.DefaultScheduledDate);

                if (slot.Date > capDate)
                    throw new BadRequestException(
                        $"Playoff scheduling would exceed the configured end date ({capDate:dd.MM.yyyy}). " +
                        "Adjust the playoff end date.");

                var nextMatch = new Match
                {
                    LeagueId = series.LeagueId,
                    HomeTeamId = slot.HomeTeamId,
                    AwayTeamId = slot.AwayTeamId,
                    Round = series.RoundNumber,
                    RoundName = series.RoundName,
                    DefaultScheduledDate = slot.Date,
                    ActualScheduledDate = slot.Date,
                    Status = MatchStatus.Scheduled,
                    SchedulingStatus = SchedulingStatus.Default,
                    LastSchedulingUpdate = DateTime.Now,
                    VenueOverride = league.DefaultVenue,
                    PlayoffSeriesId = series.Id
                };

                await _playoffRepository.UpdateSeriesAndActivateNextAsync(
                    series, new List<PlayoffSeries>(), new List<Match> { nextMatch }, ct);
            }
        }

        private async Task<List<PlayoffSeries>> FindNextRoundStubsAsync(PlayoffSeries completedSeries, CancellationToken ct)
        {
            var allLeagueSeries = await _playoffRepository.GetAllSeriesForLeagueAsync(completedSeries.LeagueId, ct);
            return allLeagueSeries.Where(s =>
                s.HomeFeederSeriesId == completedSeries.Id ||
                s.AwayFeederSeriesId == completedSeries.Id).ToList();
        }
    }
}
