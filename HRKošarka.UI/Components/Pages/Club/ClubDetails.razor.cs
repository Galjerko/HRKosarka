using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Contracts;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Club
{
    public partial class ClubDetails
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private IClubService ClubService { get; set; } = default!;
        [Inject] private NavigationManager NavigationManager { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        private ClubDetailsDTO? _club;
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
            await LoadClubDetails();
        }

        private async Task LoadClubDetails()
        {
            _isLoading = true;

            try
            {
                var response = await ClubService.GetClubDetails(Id);

                if (response.IsSuccess && response.Data != null)
                {
                    _club = response.Data;
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
                        Snackbar.Add(response.Message ?? "Failed to load club details", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while loading club details.", Severity.Error);
                Console.WriteLine($"Error loading club details: {ex.Message}");
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void DeactivateClub()
        {
            _showDeactivateDialog = true;
        }

        private async Task ConfirmDeactivate()
        {
            if (_club == null) return;

            _isProcessing = true;

            try
            {
                var response = await ClubService.DeactivateClub(_club.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Club deactivated successfully!", Severity.Success);
                    _showDeactivateDialog = false;
                    await LoadClubDetails();
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
                        Snackbar.Add(response.Message ?? "Failed to deactivate club", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while deactivating the club.", Severity.Error);
                Console.WriteLine($"Error deactivating club: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }

        private void DeleteClub()
        {
            _showDeleteDialog = true;
        }

        private async Task ConfirmDelete()
        {
            if (_club == null) return;

            _isProcessing = true;

            try
            {
                var response = await ClubService.DeleteClub(_club.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Club deleted successfully!", Severity.Success);
                    NavigationManager.NavigateTo("/clubs");
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
                        Snackbar.Add(response.Message ?? "Failed to delete club", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while deleting the club.", Severity.Error);
                Console.WriteLine($"Error deleting club: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
