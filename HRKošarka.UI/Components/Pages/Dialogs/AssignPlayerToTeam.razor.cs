using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Dialogs
{
    public partial class AssignPlayerToTeam : PermissionBaseComponent
    {
        [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = default!;
        [Parameter] public Guid TeamId { get; set; }

        [Inject] public IPlayerService PlayerService { get; set; } = default!;
        [Inject] public ISeasonService SeasonService { get; set; } = default!;
        [Inject] public ITeamService TeamService { get; set; } = default!;

        private List<AvailablePlayerDTO> _players = new();
        private List<SeasonDTO> _seasons = new();
        private AvailablePlayerDTO? _selectedPlayer;
        private SeasonDTO? _selectedSeason;
        private DateTime? _joinDate;
        private int? _jerseyNumber;
        private string _searchTerm = string.Empty;
        private bool _loadingPlayers;
        private bool _loadingSeasons;
        private bool _saving;

        private DateTime? MinJoinDate => _selectedSeason?.StartDate;
        private DateTime? MaxJoinDate => _selectedSeason?.EndDate;

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await Task.WhenAll(LoadPlayersAsync(), LoadSeasonsAsync());
        }

        private async Task OnSearchChanged(string value)
        {
            _searchTerm = value;
            await LoadPlayersAsync();
        }

        private async Task LoadPlayersAsync()
        {
            _loadingPlayers = true;
            StateHasChanged();

            try
            {
                var response = await PlayerService.GetAvailablePlayers(TeamId, _searchTerm);

                if (response.IsSuccess && response.Data != null)
                {
                    _players = response.Data;
                    if (_selectedPlayer is not null && !_players.Any(p => p.Id == _selectedPlayer.Id))
                        _selectedPlayer = null;
                }
                else
                {
                    _players.Clear();
                    _selectedPlayer = null;
                    Snackbar.Add(response.Message ?? "Failed to load available players.", Severity.Error);
                }
            }
            finally
            {
                _loadingPlayers = false;
                StateHasChanged();
            }
        }

        private async Task LoadSeasonsAsync()
        {
            _loadingSeasons = true;
            StateHasChanged();

            try
            {
                var response = await SeasonService.GetSeasons(new PaginationRequest { Page = 1, PageSize = 100 });

                if (response.IsSuccess && response.Data != null)
                    _seasons = response.Data
                        .Where(s => !s.IsCompleted && s.EndDate >= DateTime.Today)
                        .OrderByDescending(s => s.StartDate)
                        .ToList();
                else
                    Snackbar.Add(response.Message ?? "Failed to load seasons.", Severity.Error);
            }
            finally
            {
                _loadingSeasons = false;
                StateHasChanged();
            }
        }

        private void SelectPlayer(AvailablePlayerDTO player)
        {
            _selectedPlayer = _selectedPlayer?.Id == player.Id ? null : player;
        }

        private string PlayerRowClass(AvailablePlayerDTO player, int _)
            => _selectedPlayer?.Id == player.Id ? "picker-row-selected" : "picker-row";

        private void OnSeasonChanged(SeasonDTO? season)
        {
            _selectedSeason = season;
            if (season == null)
            {
                _joinDate = null;
            }
            else
            {
                // Auto-set to season start; user can still change it manually
                _joinDate = season.StartDate;
            }
        }

        private void Cancel() => MudDialog.Cancel();

        private bool CanSave => _selectedPlayer is not null
            && _selectedSeason is not null
            && _joinDate.HasValue
            && !_saving;

        private async Task Save()
        {
            if (!CanSave) return;

            _saving = true;
            StateHasChanged();

            try
            {
                var cmd = new AssignPlayerToTeamCommand
                {
                    TeamId = TeamId,
                    PlayerId = _selectedPlayer!.Id,
                    SeasonId = _selectedSeason!.Id,
                    JoinDate = _joinDate!.Value,
                    JerseyNumber = _jerseyNumber
                };

                var result = await TeamService.AssignPlayerToTeam(TeamId, cmd);

                if (!result.IsSuccess)
                {
                    foreach (var error in result.Errors?.Any() == true
                        ? result.Errors
                        : new List<string> { result.Message ?? "Failed to assign player." })
                        Snackbar.Add(error, Severity.Error);
                    return;
                }

                MudDialog.Close(DialogResult.Ok(true));
            }
            finally
            {
                _saving = false;
                StateHasChanged();
            }
        }

        private static string GetPositionLabel(HRKošarka.UI.Services.Base.Position? position) => position switch
        {
            HRKošarka.UI.Services.Base.Position._0 => "Point Guard",
            HRKošarka.UI.Services.Base.Position._1 => "Shooting Guard",
            HRKošarka.UI.Services.Base.Position._2 => "Small Forward",
            HRKošarka.UI.Services.Base.Position._3 => "Power Forward",
            HRKošarka.UI.Services.Base.Position._4 => "Center",
            _ => "-"
        };
    }
}
