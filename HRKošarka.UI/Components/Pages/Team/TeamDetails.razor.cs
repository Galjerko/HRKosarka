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
        private bool _isLoading = true;
        private bool _isLoadingRoster = false;
        private bool _isLoadingLeagues = false;
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
                    await Task.WhenAll(LoadRoster(), LoadLeagues());
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
                ["Player"] = player
            };

            var dialog = await DialogService.ShowAsync<EditTeamPlayerAssignment>("Edit Assignment", parameters, _dialogOptions);
            var result = await dialog.Result;

            if (result?.Canceled != false || result.Data is not UpdatePlayerAssignmentInTeamCommand command)
            {
                return;
            }

            _isProcessing = true;

            try
            {
                var response = await TeamService.UpdatePlayerAssignmentInTeam(Id, player.PlayerId, command);
                if (response.IsSuccess)
                {
                    Snackbar.Add("Player assignment updated successfully.", Severity.Success);
                    await LoadRoster();
                }
                else
                {
                    foreach (var error in response.Errors?.Any() == true
                        ? response.Errors
                        : new List<string> { response.Message ?? "Failed to update player assignment." })
                    {
                        Snackbar.Add(error, Severity.Error);
                    }
                }
            }
            finally
            {
                _isProcessing = false;
            }
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
