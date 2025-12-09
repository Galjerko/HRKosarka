using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Club
{
    public partial class ClubDetails : PermissionBaseComponent
    {
        [Parameter] public Guid Id { get; set; }
        [Inject] private IClubService ClubService { get; set; } = default!;

        private ClubDetailsDTO? _club;
        private bool _isLoading = true;
        private bool _isProcessing = false;
        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private bool _showDeleteDialog = false;

        private string DeactivateMessage =>
            _club is null
                ? string.Empty
                : $"Are you sure you want to deactivate <strong>{_club.Name}</strong>?";

        private string DeleteMessage =>
            _club is null
                ? string.Empty
                : $"Are you sure you want to permanently delete <strong>{_club.Name}</strong>?";

        private string ActivateMessage =>
            _club is null
                ? string.Empty
                : $"Are you sure you want to activate <strong>{_club.Name}</strong>?";

        private readonly DialogOptions _dialogOptions = new()
        {
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
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

                    // Set club-specific permissions
                    await SetClubPermissions(Id);
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

        private string? GetLogoUrl()
        {
            if (_club?.ImageBytes != null && _club.ImageBytes.Length > 0 &&
                !string.IsNullOrEmpty(_club.ImageContentType))
            {
                var base64 = Convert.ToBase64String(_club.ImageBytes);
                return $"data:{_club.ImageContentType};base64,{base64}";
            }
            return null;
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

        private void ActivateClub()
        {
            _showActivateDialog = true;
        }

        private async Task ConfirmActivate()
        {
            if (_club == null) return;

            _isProcessing = true;

            try
            {
                var response = await ClubService.ActivateClub(_club.Id);

                if (response.IsSuccess)
                {
                    Snackbar.Add("Club activated successfully!", Severity.Success);
                    _showActivateDialog = false;
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
                        Snackbar.Add(response.Message ?? "Failed to activate club", Severity.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add("An unexpected error occurred while activating the club.", Severity.Error);
                Console.WriteLine($"Error activating club: {ex.Message}");
            }
            finally
            {
                _isProcessing = false;
            }
        }
    }
}
