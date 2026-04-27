using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Components.Pages.Dialogs;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Player
{
    public partial class PlayerDetails : PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private IPlayerService PlayerService { get; set; } = default!;
        [Inject] private ITeamService TeamService { get; set; } = default!;
        [Inject] private IDialogService DialogService { get; set; } = default!;

        private PlayerDetailsDTO? _player;
        private List<PlayerAssignmentDTO> _assignments = new();
        private bool _isLoading = true;
        private bool _isLoadingAssignments = false;
        private bool _isProcessing = false;
        private bool _isAdmin = false;
        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private bool _showDeleteDialog = false;
        private bool _showRemoveAssignmentDialog = false;
        private PlayerAssignmentDTO? _assignmentToRemove;

        private string DeactivateMessage =>
            _player is null ? string.Empty
                : $"Are you sure you want to deactivate <strong>{_player.FirstName} {_player.LastName}</strong>?";

        private string ActivateMessage =>
            _player is null ? string.Empty
                : $"Are you sure you want to activate <strong>{_player.FirstName} {_player.LastName}</strong>?";

        private string DeleteMessage =>
            _player is null ? string.Empty
                : $"Are you sure you want to permanently delete <strong>{_player.FirstName} {_player.LastName}</strong>?";

        private readonly DialogOptions _dialogOptions = new()
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            _isAdmin = CurrentUser?.IsInRole("Administrator") ?? false;
            await LoadPlayerDetails();
        }

        private async Task LoadAssignments()
        {
            _isLoadingAssignments = true;

            try
            {
                var response = await PlayerService.GetPlayerAssignments(Id);

                if (response.IsSuccess && response.Data != null)
                    _assignments = response.Data;
                else
                    _assignments = new();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading player assignments: {ex.Message}");
            }
            finally
            {
                _isLoadingAssignments = false;
            }
        }

        private async Task OpenAssignTeamDialog()
        {
            var options = new DialogOptions { MaxWidth = MaxWidth.Medium, FullWidth = true };
            var parameters = new DialogParameters { ["PlayerId"] = Id };

            var dialog = await DialogService.ShowAsync<AssignTeamToPlayer>("Assign to Team", parameters, options);
            var result = await dialog.Result;

            if (result != null && !result.Canceled)
            {
                Snackbar.Add("Player assigned to team successfully!", Severity.Success);
                await LoadAssignments();
            }
        }

        private void RemoveAssignment(PlayerAssignmentDTO assignment)
        {
            _assignmentToRemove = assignment;
            _showRemoveAssignmentDialog = true;
        }

        private async Task ConfirmRemoveAssignment()
        {
            if (_assignmentToRemove == null) return;

            _isProcessing = true;

            try
            {
                var response = await TeamService.RemovePlayerFromTeam(_assignmentToRemove.TeamId, Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Player removed from team successfully.", Severity.Success);
                    _showRemoveAssignmentDialog = false;
                    _assignmentToRemove = null;
                    await LoadAssignments();
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
                Snackbar.Add("An unexpected error occurred.", Severity.Error);
                Console.WriteLine($"Error removing assignment: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task LoadPlayerDetails()
        {
            _isLoading = true;

            try
            {
                var response = await PlayerService.GetPlayerDetails(Id);

                if (response.IsSuccess && response.Data != null)
                {
                    _player = response.Data;
                    await LoadAssignments();
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
                        Snackbar.Add(response.Message ?? "Failed to load player details", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while loading player details.", Severity.Error);
                Console.WriteLine($"Error loading player details: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private string? GetPhotoUrl()
        {
            if (_player?.ImageBytes != null && _player.ImageBytes.Length > 0 &&
                !string.IsNullOrEmpty(_player.ImageContentType))
            {
                var base64 = Convert.ToBase64String(_player.ImageBytes);
                return $"data:{_player.ImageContentType};base64,{base64}";
            }
            return null;
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

        private void DeactivatePlayer() => _showDeactivateDialog = true;
        private void ActivatePlayer() => _showActivateDialog = true;
        private void DeletePlayer() => _showDeleteDialog = true;

        private async Task ConfirmDeactivate()
        {
            if (_player == null) return;
            _isProcessing = true;

            try
            {
                var response = await PlayerService.DeactivatePlayer(_player.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Player deactivated successfully!", Severity.Success);
                    _showDeactivateDialog = false;
                    await LoadPlayerDetails();
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
                        Snackbar.Add(response.Message ?? "Failed to deactivate player", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while deactivating the player.", Severity.Error);
                Console.WriteLine($"Error deactivating player: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task ConfirmActivate()
        {
            if (_player == null) return;
            _isProcessing = true;

            try
            {
                var response = await PlayerService.ActivatePlayer(_player.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Player activated successfully!", Severity.Success);
                    _showActivateDialog = false;
                    await LoadPlayerDetails();
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
                        Snackbar.Add(response.Message ?? "Failed to activate player", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while activating the player.", Severity.Error);
                Console.WriteLine($"Error activating player: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private async Task ConfirmDelete()
        {
            if (_player == null) return;
            _isProcessing = true;

            try
            {
                var response = await PlayerService.DeletePlayer(_player.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Player deleted successfully!", Severity.Success);
                    NavigationManager.NavigateTo("/players");
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
                        Snackbar.Add(response.Message ?? "Failed to delete player", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while deleting the player.", Severity.Error);
                Console.WriteLine($"Error deleting player: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
