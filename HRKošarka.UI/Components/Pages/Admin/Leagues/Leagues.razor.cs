using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Leagues
{
    public partial class Leagues : PermissionBaseComponent, IDisposable
    {
        [Inject] private ILeagueService LeagueService { get; set; } = default!;

        private MudTable<LeagueDTO> _table = default!;
        private string _searchTerm = string.Empty;
        private bool _loading = false;
        private bool _isProcessing = false;
        private CancellationTokenSource _cancellationTokenSource = new();

        private Gender? _filterGender;
        private CompetitionType? _filterCompetitionType;
        private bool? _filterActive;

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

        public CompetitionType? FilterCompetitionType
        {
            get => _filterCompetitionType;
            set
            {
                if (_filterCompetitionType != value)
                {
                    _filterCompetitionType = value;
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

        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private bool _showDeleteDialog = false;
        private Guid _selectedLeagueId = Guid.Empty;
        private string? _selectedLeagueName;

        private readonly int[] _pageSizeOptions = { 10, 25, 50, 100 };
        private readonly DialogOptions _dialogOptions = new()
        {
            CloseButton = false,
            MaxWidth = MaxWidth.Small,
            FullWidth = true
        };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private async Task<TableData<LeagueDTO>> LoadServerData(TableState state, CancellationToken token)
        {
            _loading = true;
            StateHasChanged();
            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                using var combined = CancellationTokenSource.CreateLinkedTokenSource(
                    token, _cancellationTokenSource.Token);

                var request = new PaginationRequest
                {
                    Page = state.Page + 1,
                    PageSize = state.PageSize <= 0 ? 10 : state.PageSize,
                    SortBy = !string.IsNullOrEmpty(state.SortLabel) ? state.SortLabel : "Name",
                    SortDirection = state.SortDirection == SortDirection.Descending ? "desc" : "asc",
                    SearchTerm = string.IsNullOrWhiteSpace(_searchTerm) ? null : _searchTerm.Trim()
                };

                var response = await LeagueService.GetLeagues(
                    request,
                    gender: _filterGender,
                    competitionType: _filterCompetitionType,
                    isActive: _filterActive);

                if (response.IsSuccess && response.Data != null)
                {
                    return new TableData<LeagueDTO>
                    {
                        Items = response.Data,
                        TotalItems = response.Pagination?.TotalCount ?? 0
                    };
                }

                Snackbar.Add(response.Message ?? "Failed to load leagues.", Severity.Error);
                return new TableData<LeagueDTO> { Items = new List<LeagueDTO>(), TotalItems = 0 };
            }
            catch (OperationCanceledException)
            {
                return new TableData<LeagueDTO> { Items = new List<LeagueDTO>(), TotalItems = 0 };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading leagues: {ex.Message}", Severity.Error);
                return new TableData<LeagueDTO> { Items = new List<LeagueDTO>(), TotalItems = 0 };
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

        private async Task OnFilterChanged()
        {
            if (_table != null)
            {
                await _table.ReloadServerData();
            }
        }

        private async Task ResetFilters()
        {
            _filterGender = null;
            _filterCompetitionType = null;
            _filterActive = null;
            _searchTerm = string.Empty;
            await OnFilterChanged();
        }


        private void OpenDeactivateDialog(Guid id, string name)
        {
            _selectedLeagueId = id;
            _selectedLeagueName = name;
            _showDeactivateDialog = true;
        }

        private void OpenActivateDialog(Guid id, string name)
        {
            _selectedLeagueId = id;
            _selectedLeagueName = name;
            _showActivateDialog = true;
        }

        private void OpenDeleteDialog(Guid id, string name)
        {
            _selectedLeagueId = id;
            _selectedLeagueName = name;
            _showDeleteDialog = true;
        }

        private async Task ConfirmDeactivate()
        {
            _isProcessing = true;
            try
            {
                var result = await LeagueService.DeactivateLeague(_selectedLeagueId);

                if (result.IsSuccess)
                {
                    Snackbar.Add("League deactivated successfully.", Severity.Success);
                    _showDeactivateDialog = false;
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to deactivate league.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deactivating league: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isProcessing = false;
                _selectedLeagueId = Guid.Empty;
            }
        }

        private async Task ConfirmActivate()
        {
            _isProcessing = true;
            try
            {
                var result = await LeagueService.ActivateLeague(_selectedLeagueId);

                if (result.IsSuccess)
                {
                    Snackbar.Add("League activated successfully.", Severity.Success);
                    _showActivateDialog = false;
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to activate league.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error activating league: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isProcessing = false;
                _selectedLeagueId = Guid.Empty;
            }
        }

        private async Task ConfirmDelete()
        {
            _isProcessing = true;
            try
            {
                var result = await LeagueService.DeleteLeague(_selectedLeagueId);

                if (result.IsSuccess)
                {
                    Snackbar.Add("League deleted successfully.", Severity.Success);
                    _showDeleteDialog = false;
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to delete league.", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deleting league: {ex.Message}", Severity.Error);
            }
            finally
            {
                _isProcessing = false;
                _selectedLeagueId = Guid.Empty;
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
