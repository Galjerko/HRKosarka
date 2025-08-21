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
        private bool _showDeleteDialog = false;

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
    }
}
