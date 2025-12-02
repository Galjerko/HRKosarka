using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Models.UserManagement;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Team
{
    public partial class Teams : PermissionBaseComponent, IDisposable
    {
        [Inject] public ITeamService TeamService { get; set; } = default!;
        [Inject] public IAgeCategoryService AgeCategoryService { get; set; } = default!;

        private MudTable<TeamDTO> _table = default!;
        private string _searchTerm = string.Empty;
        private bool _loading = false;
        private CancellationTokenSource _cancellationTokenSource = new();

        private Guid? _filterAgeCategory = null;
        private Gender? _filterGender = null;
        private bool? _filterActive = null;

        private bool _showDeleteDialog = false;
        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private bool _showRenameDialog = false;
        private Guid _selectedTeamId = Guid.Empty;
        private string? _selectedTeamName;
        private string _newTeamName = string.Empty;
        private bool _isRenaming = false;

        private readonly int[] _pageSizeOptions = { 10, 25, 50, 100 };
        private readonly DialogOptions _dialogOptions = new()
        {
            CloseButton = false,
            MaxWidth = MaxWidth.Small,
            FullWidth = true,
        };

        private List<AgeCategoryDTO> _ageCategories = new();

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            // REMOVED SetListPermissions() - using per-club permissions instead
            await LoadAgeCategories();
        }

        private async Task LoadAgeCategories()
        {
            var result = await AgeCategoryService.GetAllAgeCategories();
            if (result.IsSuccess && result.Data != null)
                _ageCategories = result.Data;
        }

        private async Task<UserPermissions> GetTeamPermissions(Guid clubId)
        {
            try
            {
                if (CurrentUser == null)
                    await LoadUserContext();

                return await PermissionService.GetPermissionsAsync(CurrentUser!, clubId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error getting team permissions: {ex.Message}");
                return new UserPermissions();
            }
        }

        public Guid? FilterAgeCategory
        {
            get => _filterAgeCategory;
            set
            {
                if (_filterAgeCategory != value)
                {
                    _filterAgeCategory = value;
                    _ = OnFilterChanged();
                }
            }
        }

        public Gender? FilterGender
        {
            get => _filterGender;
            set
            {
                if (_filterGender != value)
                {
                    _filterGender = value;
                    _ = OnFilterChanged();
                }
            }
        }

        public bool? FilterActive
        {
            get => _filterActive;
            set
            {
                if (_filterActive != value)
                {
                    _filterActive = value;
                    _ = OnFilterChanged();
                }
            }
        }

        private async Task OnSearchChanged()
        {
            if (_table != null)
                await _table.ReloadServerData();
        }

        private async Task OnFilterChanged()
        {
            if (_table != null)
                await _table.ReloadServerData();
        }

        private void ResetFilters()
        {
            FilterAgeCategory = null;
            FilterGender = null;
            FilterActive = null;
            _searchTerm = string.Empty;
            OnFilterChanged();
        }

        private async Task<TableData<TeamDTO>> LoadServerData(TableState state, CancellationToken token)
        {
            _loading = true;
            StateHasChanged();

            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                using var combinedTokenSource = CancellationTokenSource.CreateLinkedTokenSource(
                    token, _cancellationTokenSource.Token);

                var request = new TeamPaginationRequest
                {
                    Page = state.Page + 1,
                    PageSize = state.PageSize,
                    SortBy = !string.IsNullOrEmpty(state.SortLabel) ? state.SortLabel : "Name",
                    SortDirection = state.SortDirection == SortDirection.Descending ? "desc" : "asc",
                    SearchTerm = _searchTerm?.Trim(),
                    AgeCategoryId = _filterAgeCategory,
                    Gender = _filterGender,
                    IsActive = _filterActive,
                };

                var response = await TeamService.GetTeams(request);

                if (response.IsSuccess && response.Data != null)
                {
                    return new TableData<TeamDTO>
                    {
                        Items = response.Data,
                        TotalItems = response.Pagination?.TotalCount ?? 0
                    };
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to load teams", Severity.Error);
                    return new TableData<TeamDTO>
                    {
                        Items = new List<TeamDTO>(),
                        TotalItems = 0
                    };
                }
            }
            catch (OperationCanceledException)
            {
                return new TableData<TeamDTO>
                {
                    Items = new List<TeamDTO>(),
                    TotalItems = 0
                };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading teams: {ex.Message}", Severity.Error);
                return new TableData<TeamDTO>
                {
                    Items = new List<TeamDTO>(),
                    TotalItems = 0
                };
            }
            finally
            {
                _loading = false;
                StateHasChanged();
            }
        }

        private void ViewTeam(Guid id)
        {
            NavigationManager.NavigateTo($"/teams/{id}");
        }

        private void CreateTeam()
        {
            NavigationManager.NavigateTo("/teams/create");
        }

        private void EditTeam(Guid id, string name)
        {
            _selectedTeamId = id;
            _selectedTeamName = name;
            _newTeamName = name;
            _showRenameDialog = true;
        }

        private async Task ConfirmRename()
        {
            if (_selectedTeamId == Guid.Empty) return;

            var trimmed = _newTeamName?.Trim();
            if (string.IsNullOrWhiteSpace(trimmed) || trimmed == _selectedTeamName)
            {
                _showRenameDialog = false;
                return;
            }

            _isRenaming = true;

            try
            {
                var updateTeamCommand = new UpdateTeamCommand
                {
                    Id = _selectedTeamId,
                    Name = trimmed!
                };

                var result = await TeamService.UpdateTeam(_selectedTeamId, updateTeamCommand);

                if (result.IsSuccess)
                {
                    Snackbar.Add("Team renamed successfully.", Severity.Success);
                    _showRenameDialog = false;
                    await _table.ReloadServerData();
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

        private void DeactivateTeam(Guid id, string name)
        {
            _selectedTeamId = id;
            _selectedTeamName = name;
            _showDeactivateDialog = true;
        }

        private async Task ConfirmDeactivate()
        {
            try
            {
                var result = await TeamService.DeactivateTeam(_selectedTeamId);

                if (result.IsSuccess)
                {
                    Snackbar.Add("Team deactivated successfully.", Severity.Success);
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to deactivate team", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deactivating team: {ex.Message}", Severity.Error);
            }
            finally
            {
                _showDeactivateDialog = false;
                _selectedTeamId = Guid.Empty;
            }
        }

        private void ActivateTeam(Guid id, string name)
        {
            _selectedTeamId = id;
            _selectedTeamName = name;
            _showActivateDialog = true;
        }

        private async Task ConfirmActivate()
        {
            try
            {
                var result = await TeamService.ActivateTeam(_selectedTeamId);

                if (result.IsSuccess)
                {
                    Snackbar.Add("Team activated successfully.", Severity.Success);
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to activate team", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error activating team: {ex.Message}", Severity.Error);
            }
            finally
            {
                _showActivateDialog = false;
                _selectedTeamId = Guid.Empty;
            }
        }

        private void DeleteTeam(Guid id, string name)
        {
            _selectedTeamId = id;
            _selectedTeamName = name;
            _showDeleteDialog = true;
        }

        private async Task ConfirmDelete()
        {
            try
            {
                var result = await TeamService.DeleteTeam(_selectedTeamId);

                if (result.IsSuccess)
                {
                    Snackbar.Add("Team deleted successfully.", Severity.Success);
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to delete team", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deleting team: {ex.Message}", Severity.Error);
            }
            finally
            {
                _showDeleteDialog = false;
                _selectedTeamId = Guid.Empty;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
