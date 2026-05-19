using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Leagues
{
    public partial class LeagueSchedule : PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ILeagueService LeagueService { get; set; } = default!;

        private LeagueDetailsDTO? _league;
        private List<LeagueRoundDTO> _schedule = new();
        private List<LeagueTeamDTO> _teams = new();
        private Dictionary<Guid, string> _teamLogos = new();
        private int _selectedRound = 1;
        private bool _isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await Task.WhenAll(LoadLeague(), LoadSchedule(), LoadTeams());
            BuildLogoCache();
            _selectedRound = DetermineActiveRound();
            _isLoading = false;
        }

        private async Task LoadLeague()
        {
            try
            {
                var response = await LeagueService.GetLeagueById(Id);
                if (response.IsSuccess && response.Data != null)
                    _league = response.Data;
            }
            catch (Exception ex) { Console.WriteLine($"Error loading league: {ex.Message}"); }
        }

        private async Task LoadSchedule()
        {
            try
            {
                var response = await LeagueService.GetLeagueSchedule(Id);
                if (response.IsSuccess)
                    _schedule = response.Data ?? new();
            }
            catch (Exception ex) { Console.WriteLine($"Error loading schedule: {ex.Message}"); }
        }

        private async Task LoadTeams()
        {
            try
            {
                var response = await LeagueService.GetLeagueTeams(Id);
                if (response.IsSuccess)
                    _teams = response.Data ?? new();
            }
            catch (Exception ex) { Console.WriteLine($"Error loading teams: {ex.Message}"); }
        }

        private void BuildLogoCache()
        {
            foreach (var team in _teams)
            {
                if (team.ClubImageBytes?.Length > 0 && !string.IsNullOrEmpty(team.ClubImageContentType))
                    _teamLogos[team.TeamId] = $"data:{team.ClubImageContentType};base64,{Convert.ToBase64String(team.ClubImageBytes)}";
            }
        }

        private int DetermineActiveRound()
        {
            if (!_schedule.Any()) return 1;
            var firstIncomplete = _schedule.FirstOrDefault(r =>
                r.Matches.Any(m => m.Status != MatchStatus._2 && m.Status != MatchStatus._3));
            return firstIncomplete?.Round ?? _schedule.Last().Round;
        }

        private LeagueRoundDTO? CurrentRoundData =>
            _schedule.FirstOrDefault(r => r.Round == _selectedRound);

        private LeagueTeamDTO? GetByeTeam(LeagueRoundDTO round)
        {
            var playing = new HashSet<Guid>();
            foreach (var m in round.Matches) { playing.Add(m.HomeTeamId); playing.Add(m.AwayTeamId); }
            return _teams.FirstOrDefault(t => !playing.Contains(t.TeamId));
        }

        private string? GetTeamLogo(Guid teamId) => _teamLogos.GetValueOrDefault(teamId);

        private List<int> GetRoundPageNumbers()
        {
            var rounds = _schedule.Select(r => r.Round).OrderBy(r => r).ToList();
            if (rounds.Count <= 9) return rounds;

            var show = new HashSet<int>();
            show.Add(rounds[0]);
            if (rounds.Count > 1) show.Add(rounds[1]);

            int idx = rounds.IndexOf(_selectedRound);
            if (idx > 0) show.Add(rounds[idx - 1]);
            show.Add(_selectedRound);
            if (idx < rounds.Count - 1) show.Add(rounds[idx + 1]);

            show.Add(rounds[rounds.Count - 2]);
            show.Add(rounds[rounds.Count - 1]);

            var sorted = show.OrderBy(r => r).ToList();
            var result = new List<int>();
            for (int i = 0; i < sorted.Count; i++)
            {
                if (i > 0 && sorted[i] - sorted[i - 1] > 1) result.Add(-1);
                result.Add(sorted[i]);
            }
            return result;
        }

        private void GoToFirstRound() => _selectedRound = _schedule.First().Round;
        private void GoToLastRound()  => _selectedRound = _schedule.Last().Round;

        private void GoToPrevRound()
        {
            var prev = _schedule.LastOrDefault(r => r.Round < _selectedRound);
            if (prev != null) _selectedRound = prev.Round;
        }

        private void GoToNextRound()
        {
            var next = _schedule.FirstOrDefault(r => r.Round > _selectedRound);
            if (next != null) _selectedRound = next.Round;
        }

        private bool IsFirstRound => _schedule.FirstOrDefault()?.Round == _selectedRound;
        private bool IsLastRound  => _schedule.LastOrDefault()?.Round == _selectedRound;

        private static Color MatchBorderColor(MatchStatus status) => status switch
        {
            MatchStatus._2 => Color.Success,
            MatchStatus._3 => Color.Secondary,
            MatchStatus._1 => Color.Warning,
            _ => Color.Default
        };
    }
}
