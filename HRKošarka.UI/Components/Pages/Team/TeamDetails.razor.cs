using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Components.Pages.Dialogs;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Team
{
    public partial class TeamDetails : PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }

        [Inject] private ITeamService TeamService { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;

        private TeamDetailsDTO? _team;
        private List<TeamRosterPlayerDTO> _roster = new();
        private List<TeamLeagueDTO> _leagues = new();
        private List<TeamMatchHistoryItemDTO> _matchHistory = new();
        private List<TeamRepresentativeDTO> _representatives = new();
        private List<TeamPlayerStatDTO> _playerStats = new();
        private TeamLeagueStandingDTO? _teamStanding;
        private string? _statsSelectedSeason;
        private Guid? _statsSelectedLeagueId;
        private bool _isRepForThisTeam = false;
        private bool _isFavorited = false;
        private bool _isTogglingFavorite = false;
        private bool _isLoading = true;
        private bool _isLoadingRoster = false;
        private bool _isLoadingLeagues = false;
        private bool _isLoadingMatchHistory = false;
        private bool _isLoadingReps = false;
        private bool _isLoadingPlayerStats = false;
        private bool _isProcessing = false;
        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private bool _showDeleteDialog = false;
        private bool _showRenameDialog = false;
        private bool _showRemovePlayerDialog = false;
        private string _newTeamName = string.Empty;
        private bool _isRenaming = false;
        private TeamRosterPlayerDTO? _playerToRemove;

        private string DeactivateMessage =>
            _team is null
                ? string.Empty
                : $"Are you sure you want to deactivate <strong>{_team.Name}</strong>?";

        private string DeleteMessage =>
            _team is null
                ? string.Empty
                : $"Are you sure you want to permanently delete <strong>{_team.Name}</strong>?";

        private string ActivateMessage =>
            _team is null
                ? string.Empty
                : $"Are you sure you want to activate <strong>{_team.Name}</strong>?";

        private readonly DialogOptions _dialogOptions = new()
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await LoadTeamDetails();
        }

        private async Task LoadTeamDetails()
        {
            _isLoading = true;

            try
            {
                var response = await TeamService.GetTeamDetails(Id);

                if (response.IsSuccess && response.Data != null)
                {
                    _team = response.Data;
                    _newTeamName = _team.Name;

                    await SetClubPermissions(_team.ClubId);

                    bool isAdmin = CurrentUser?.IsInRole("Administrator") == true;
                    bool isClubManager = CurrentUser?.IsInRole("ClubManager") == true;
                    bool isUser = CurrentUser?.IsInRole("RegularUser") == true;

                    if (!isAdmin && !isClubManager)
                    {
                        var repResponse = await TeamService.GetMyRepresentativeships();
                        if (repResponse.IsSuccess && repResponse.Data != null)
                            _isRepForThisTeam = repResponse.Data.Any(r => r.TeamId == Id);
                    }

                    if (isUser)
                    {
                        var favResponse = await TeamService.GetFavoriteStatus(Id);
                        if (favResponse.IsSuccess)
                            _isFavorited = favResponse.Data;
                    }

                    await Task.WhenAll(LoadRoster(), LoadLeagues(), LoadMatchHistory(), LoadRepresentatives());
                }
                else
                {
                    _team = null;

                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                        {
                            Snackbar.Add(error, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to load team details", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while loading team details.", Severity.Error);
                Console.WriteLine($"Error loading team details: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private async Task LoadRoster()
        {
            _isLoadingRoster = true;

            try
            {
                var response = await TeamService.GetTeamRoster(Id);

                if (response.IsSuccess && response.Data != null)
                {
                    _roster = response.Data;
                }
                else
                {
                    _roster = new();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading roster: {ex.Message}");
            }
            finally
            {
                _isLoadingRoster = false;
            }
        }

        private async Task LoadLeagues()
        {
            _isLoadingLeagues = true;
            try
            {
                var response = await TeamService.GetTeamLeagues(Id);
                _leagues = response.IsSuccess ? response.Data ?? new List<TeamLeagueDTO>() : new List<TeamLeagueDTO>();

                if (_leagues.Any())
                {
                    _statsSelectedSeason = _leagues
                        .Select(l => l.SeasonName)
                        .Distinct()
                        .OrderByDescending(s => s)
                        .First();

                    _statsSelectedLeagueId = _leagues
                        .Where(l => l.SeasonName == _statsSelectedSeason)
                        .Select(l => l.LeagueId)
                        .FirstOrDefault();

                    if (_statsSelectedLeagueId.HasValue)
                        await LoadPlayerStats();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading leagues: {ex.Message}");
            }
            finally
            {
                _isLoadingLeagues = false;
            }
        }

        private async Task LoadPlayerStats()
        {
            if (_statsSelectedLeagueId == null) return;

            _isLoadingPlayerStats = true;
            try
            {
                var statsTask = TeamService.GetTeamLeaguePlayerStats(Id, _statsSelectedLeagueId.Value);
                var standingTask = TeamService.GetTeamLeagueStanding(Id, _statsSelectedLeagueId.Value);
                await Task.WhenAll(statsTask, standingTask);

                _playerStats = statsTask.Result.IsSuccess ? statsTask.Result.Data ?? new() : new();
                _teamStanding = standingTask.Result.IsSuccess ? standingTask.Result.Data : null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading player stats: {ex.Message}");
            }
            finally
            {
                _isLoadingPlayerStats = false;
            }
        }

        private async Task OnSeasonChanged(string? season)
        {
            _statsSelectedSeason = season;
            _statsSelectedLeagueId = _leagues
                .Where(l => l.SeasonName == season)
                .Select(l => l.LeagueId)
                .FirstOrDefault();
            _playerStats = new();
            _teamStanding = null;
            if (_statsSelectedLeagueId.HasValue)
                await LoadPlayerStats();
        }

        private async Task OnLeagueChanged(Guid? leagueId)
        {
            _statsSelectedLeagueId = leagueId;
            _playerStats = new();
            _teamStanding = null;
            if (_statsSelectedLeagueId.HasValue)
                await LoadPlayerStats();
        }

        private async Task LoadMatchHistory()
        {
            _isLoadingMatchHistory = true;
            try
            {
                var response = await TeamService.GetTeamMatchHistory(Id);
                _matchHistory = response.IsSuccess ? response.Data ?? new() : new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading match history: {ex.Message}");
            }
            finally
            {
                _isLoadingMatchHistory = false;
            }
        }

        private async Task LoadRepresentatives()
        {
            _isLoadingReps = true;
            try
            {
                var response = await TeamService.GetTeamRepresentatives(Id);
                _representatives = response.IsSuccess ? response.Data ?? new() : new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading representatives: {ex.Message}");
            }
            finally
            {
                _isLoadingReps = false;
            }
        }

        private async Task OpenAssignRepDialog()
        {
            var options = new DialogOptions { MaxWidth = MaxWidth.Small, FullWidth = true, CloseButton = true };
            var parameters = new DialogParameters { ["TeamId"] = Id };
            var dialog = await DialogService.ShowAsync<AssignTeamRepresentative>("Assign Representative", parameters, options);
            var result = await dialog.Result;
            if (result is { Canceled: false })
            {
                Snackbar.Add("Representative assigned successfully!", Severity.Success);
                await LoadRepresentatives();
            }
        }

        private async Task RevokeRep(TeamRepresentativeDTO rep)
        {
            _isProcessing = true;
            try
            {
                var response = await TeamService.RevokeTeamRepresentative(Id, rep.Id);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Representative revoked.", Severity.Success);
                    await LoadRepresentatives();
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to revoke.", Severity.Error);
                }
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task OpenAssignPlayerDialog()
        {
            var options = new DialogOptions { MaxWidth = MaxWidth.Large, FullWidth = true };
            var parameters = new DialogParameters { ["TeamId"] = Id };

            var dialog = await DialogService.ShowAsync<AssignPlayerToTeam>("Assign Player", parameters, options);
            var result = await dialog.Result;

            if (result != null && !result.Canceled)
            {
                Snackbar.Add("Player assigned successfully!", Severity.Success);
                await LoadRoster();
            }
        }

        private async Task EditPlayerAssignment(TeamRosterPlayerDTO player)
        {
            var parameters = new DialogParameters
            {
                ["Player"] = player,
                ["TeamId"] = Id
            };

            var dialog = await DialogService.ShowAsync<EditTeamPlayerAssignment>("Edit Assignment", parameters, _dialogOptions);
            var result = await dialog.Result;

            if (result is { Canceled: false })
                await LoadRoster();
        }

        private void RemovePlayer(TeamRosterPlayerDTO player)
        {
            _playerToRemove = player;
            _showRemovePlayerDialog = true;
        }

        private async Task ConfirmRemovePlayer()
        {
            if (_playerToRemove == null) return;

            _isProcessing = true;

            try
            {
                var response = await TeamService.RemovePlayerFromTeam(Id, _playerToRemove.PlayerId);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Player removed from team successfully.", Severity.Success);
                    _showRemovePlayerDialog = false;
                    _playerToRemove = null;
                    await LoadRoster();
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                            Snackbar.Add(error, Severity.Error);
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to remove player from team.", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while removing the player.", Severity.Error);
                Console.WriteLine($"Error removing player: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task ConfirmRename()
        {
            if (_team == null)
            {
                return;
            }

            if (string.IsNullOrWhiteSpace(_newTeamName) || _newTeamName == _team.Name)
            {
                _showRenameDialog = false;
                return;
            }

            _isRenaming = true;

            try
            {
                var updateTeamCommand = new UpdateTeamCommand
                {
                    Id = _team.Id,
                    Name = _newTeamName.Trim()
                };

                var result = await TeamService.UpdateTeam(_team.Id, updateTeamCommand);

                if (result.IsSuccess)
                {
                    Snackbar.Add("Team renamed successfully.", Severity.Success);
                    _showRenameDialog = false;
                    await LoadTeamDetails();
                }
                else
                {
                    if (result.Errors?.Any() == true)
                    {
                        foreach (var error in result.Errors)
                        {
                            Snackbar.Add(error, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(result.Message ?? "Failed to rename team.", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("Unexpected error while renaming team.", Severity.Error);
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _isRenaming = false;
            }
        }

        private void DeactivateTeam()
        {
            _showDeactivateDialog = true;
        }

        private async Task ConfirmDeactivate()
        {
            if (_team == null)
            {
                return;
            }

            _isProcessing = true;

            try
            {
                var response = await TeamService.DeactivateTeam(_team.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Team deactivated successfully!", Severity.Success);
                    _showDeactivateDialog = false;
                    await LoadTeamDetails();
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                        {
                            Snackbar.Add(error, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to deactivate team", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while deactivating the team.", Severity.Error);
                Console.WriteLine($"Error deactivating team: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void DeleteTeam()
        {
            _showDeleteDialog = true;
        }

        private async Task ConfirmDelete()
        {
            if (_team == null)
            {
                return;
            }

            _isProcessing = true;

            try
            {
                var response = await TeamService.DeleteTeam(_team.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Team deleted successfully!", Severity.Success);
                    NavigationManager.NavigateTo("/teams");
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                        {
                            Snackbar.Add(error, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to delete team", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while deleting the team.", Severity.Error);
                Console.WriteLine($"Error deleting team: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void ActivateTeam()
        {
            _showActivateDialog = true;
        }

        private async Task ConfirmActivate()
        {
            if (_team == null)
            {
                return;
            }

            _isProcessing = true;

            try
            {
                var response = await TeamService.ActivateTeam(_team.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Team activated successfully!", Severity.Success);
                    _showActivateDialog = false;
                    await LoadTeamDetails();
                }
                else
                {
                    if (response.Errors?.Any() == true)
                    {
                        foreach (var error in response.Errors)
                        {
                            Snackbar.Add(error, Severity.Error);
                        }
                    }
                    else
                    {
                        Snackbar.Add(response.Message ?? "Failed to activate team", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while activating the team.", Severity.Error);
                Console.WriteLine($"Error activating team: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task ToggleFavorite()
        {
            _isTogglingFavorite = true;
            try
            {
                var response = await TeamService.ToggleFavoriteTeam(Id);
                if (response.IsSuccess)
                {
                    _isFavorited = response.Data;
                    Snackbar.Add(_isFavorited ? "Team followed!" : "Team unfollowed.", Severity.Success);
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to update follow status.", Severity.Error);
                }
            }
            finally
            {
                _isTogglingFavorite = false;
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
