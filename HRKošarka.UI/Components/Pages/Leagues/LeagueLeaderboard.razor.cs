using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Leagues
{
    public partial class LeagueLeaderboard : PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ILeagueService LeagueService { get; set; } = default!;

        private LeagueDetailsDTO? _league;
        private List<LeaguePlayerStatDTO> _players = new();
        private bool _isLoading = true;
        private string _sortBy = nameof(LeaguePlayerStatDTO.Ppg);
        private bool _sortAsc = false;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await Task.WhenAll(LoadLeague(), LoadLeaderboard());
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

        private async Task LoadLeaderboard()
        {
            try
            {
                var direction = _sortAsc ? "asc" : "desc";
                var response = await LeagueService.GetLeagueLeaderboard(Id, _sortBy, direction);
                if (response.IsSuccess)
                    _players = response.Data ?? new();
            }
            catch (Exception ex) { Console.WriteLine($"Error loading leaderboard: {ex.Message}"); }
        }

        private async Task Sort(string column)
        {
            if (_sortBy == column)
                _sortAsc = !_sortAsc;
            else
            {
                _sortBy = column;
                _sortAsc = column == nameof(LeaguePlayerStatDTO.PlayerName)
                        || column == nameof(LeaguePlayerStatDTO.TeamName);
            }
            await LoadLeaderboard();
        }

        private string SortIcon(string column)
        {
            if (_sortBy != column) return Icons.Material.Filled.UnfoldMore;
            return _sortAsc ? Icons.Material.Filled.ArrowUpward : Icons.Material.Filled.ArrowDownward;
        }
    }
}
