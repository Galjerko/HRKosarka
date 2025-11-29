using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Models.UserManagement;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Club
{
    public partial class Clubs : PermissionBaseComponent, IDisposable
    {
        [Inject] public IClubService ClubService { get; set; } = default!;

        private MudTable<ClubDTO> _table = default!;
        private string _searchTerm = string.Empty;
        private bool _loading = false;
        private CancellationTokenSource _cancellationTokenSource = new();

        private bool _showDeleteDialog = false;
        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private Guid _selectedClubId = Guid.Empty;
        private string? _selectedClubName;

        private readonly int[] _pageSizeOptions = { 10, 25, 50, 100 };
        private readonly DialogOptions _dialogOptions = new()
        {
            CloseButton = false,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
        };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await SetListPermissions();
        }

        private async Task<UserPermissions> GetClubPermissions(Guid clubId)
        {
            try
            {
                return await PermissionService.GetPermissionsAsync(CurrentUser, clubId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting club permissions: {ex.Message}");
                return new UserPermissions();
            }
        }

        private async Task<TableData<ClubDTO>> LoadServerData(TableState state, CancellationToken token)
        {
            _loading = true;
            StateHasChanged();

            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                    token, _cancellationTokenSource.Token);

                var request = new PaginationRequest
                {
                    Page = state.Page + 1,
                    PageSize = state.PageSize,
                    SortBy = !string.IsNullOrEmpty(state.SortLabel) ? state.SortLabel : "Name",
                    SortDirection = state.SortDirection == SortDirection.Descending ? "desc" : "asc",
                    SearchTerm = _searchTerm?.Trim()
                };

                var response = await ClubService.GetClubs(request);

                if (response.IsSuccess && response.Data != null)
                {
                    return new TableData<ClubDTO>
                    {
                        Items = response.Data,
                        TotalItems = response.Pagination?.TotalCount ?? 0
                    };
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to load clubs", Severity.Error);
                    return new TableData<ClubDTO>
                    {
                        Items = new List<ClubDTO>(),
                        TotalItems = 0
                    };
                }
            }
            catch (OperationCanceledException)
            {
                return new TableData<ClubDTO>
                {
                    Items = new List<ClubDTO>(),
                    TotalItems = 0
                };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading clubs: {ex.Message}", Severity.Error);
                return new TableData<ClubDTO>
                {
                    Items = new List<ClubDTO>(),
                    TotalItems = 0
                };
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }
        }

        private async Task OnSearchChanged()
        {
            if (_table != null)
            {
                await _table.ReloadServerData();
            }
        }

        private void CreateClub()
        {
            try
            {
                NavigationManager.NavigateTo("/clubs/create");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Navigation error: {ex.Message}", Severity.Error);
            }
        }

        private void EditClub(Guid id)
        {
            try
            {
                NavigationManager.NavigateTo($"/clubs/edit/{id}");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Navigation error: {ex.Message}", Severity.Error);
            }
        }

        private void ViewClub(Guid id)
        {
            try
            {
                NavigationManager.NavigateTo($"/clubs/{id}");
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Navigation error: {ex.Message}", Severity.Error);
            }
        }

        private void DeactivateClub(Guid id, string name)
        {
            _selectedClubId = id;
            _selectedClubName = name;
            _showDeactivateDialog = true;
        }

        private async Task ConfirmDeactivate()
        {
            try
            {
                var result = await ClubService.DeactivateClub(_selectedClubId);

                if (result.IsSuccess)
                {
                    Snackbar.Add("Club deactivated successfully.", Severity.Success);
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to deactivate club", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deactivating club: {ex.Message}", Severity.Error);
            }
            finally
            {
                _showDeactivateDialog = false;
                _selectedClubId = Guid.Empty;
            }
        }

        private void DeleteClub(Guid id, string name)
        {
            _selectedClubId = id;
            _selectedClubName = name;
            _showDeleteDialog = true;
        }

        private async Task ConfirmDelete()
        {
            try
            {
                var result = await ClubService.DeleteClub(_selectedClubId);

                if (result.IsSuccess)
                {
                    Snackbar.Add("Club deleted successfully.", Severity.Success);
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to delete club", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deleting club: {ex.Message}", Severity.Error);
            }
            finally
            {
                _showDeleteDialog = false;
                _selectedClubId = Guid.Empty;
            }
        }

        private void ActivateClub(Guid id, string name)
        {
            _selectedClubId = id;
            _selectedClubName = name;
            _showActivateDialog = true;
        }

        private async Task ConfirmActivate()
        {
            try
            {
                var result = await ClubService.ActivateClub(_selectedClubId);

                if (result.IsSuccess)
                {
                    Snackbar.Add("Club activated successfully.", Severity.Success);
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to activate club", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error activating club: {ex.Message}", Severity.Error);
            }
            finally
            {
                _showActivateDialog = false;
                _selectedClubId = Guid.Empty;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
