using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Team
{
    public partial class TeamDetails
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private ITeamService TeamService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        private TeamDetailsDTO? _team;
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private bool _showDeleteDialog = false;
        private bool _showRenameDialog = false;
        private string _newTeamName = string.Empty;
        private bool _isRenaming = false;

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
                    _newTeamName = _team?.Name ?? string.Empty;
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

        private async Task ConfirmRename()
        {
            if (_team == null) return;
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
                    await LoadTeamDetails();
                    _showRenameDialog = false;
                }
                else
                {
                    if (result.Errors?.Any() == true)
                        foreach (var error in result.Errors)
                            Snackbar.Add(error, Severity.Error);
                    else
                        Snackbar.Add(result.Message ?? "Failed to rename team.", Severity.Error);
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
            if (_team == null) return;
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
                            Snackbar.Add(error, Severity.Error);
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
            if (_team == null) return;
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
                            Snackbar.Add(error, Severity.Error);
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
            if (_team == null) return;

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
    }
}
