using HRKošarka.Application.Contracts.Persistence;
using HRKošarka.Domain;
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

            int homeWins = series.HomeWins;
            int awayWins = series.AwayWins;

            if (homeWins >= series.WinsNeeded || awayWins >= series.WinsNeeded)
            {
                var league = await _leagueRepository.GetLeagueWithDetailsAsync(series.LeagueId, ct);
                var capDate = league!.PlayoffCapDate;

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
                            PlayoffSchedulingGuard.EnsureWithinCapDate(slot.Date, capDate);
                            matchesToCreate.Add(PlayoffSeriesScheduler.ToMatch(
                                slot, stub.LeagueId, stub.RoundNumber, stub.RoundName, league.DefaultVenue, stub.Id));
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
                var league = await _leagueRepository.GetLeagueWithDetailsAsync(series.LeagueId, ct);
                var capDate = league!.PlayoffCapDate;

                var lastMatch = orderedMatches.Last();
                var slot = PlayoffSeriesScheduler.GenerateNextGame(
                    series.HomeTeamId!.Value, series.AwayTeamId!.Value,
                    series.WinsNeeded, orderedMatches.Count, lastMatch.DefaultScheduledDate);

                PlayoffSchedulingGuard.EnsureWithinCapDate(slot.Date, capDate);

                var nextMatch = PlayoffSeriesScheduler.ToMatch(
                    slot, series.LeagueId, series.RoundNumber, series.RoundName, league.DefaultVenue, series.Id);

                await _playoffRepository.UpdateSeriesAndActivateNextAsync(
                    series, new List<PlayoffSeries>(), new List<Match> { nextMatch }, ct);
            }
        }

        private Task<List<PlayoffSeries>> FindNextRoundStubsAsync(PlayoffSeries completedSeries, CancellationToken ct)
            => _playoffRepository.GetUpcomingSeriesPopulatedByThisSeriesAsync(completedSeries.Id, ct);
    }
}
