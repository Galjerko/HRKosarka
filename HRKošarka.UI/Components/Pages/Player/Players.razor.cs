using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Player
{
    public partial class Players/* : PermissionBaseComponent, IDisposable*/
    {
        [Inject] public IPlayerService PlayerService { get; set; } = default!;

        private MudTable<PlayerDTO> _table = default!;
        private string _searchTerm = string.Empty;
        private bool _loading = false;
        private CancellationTokenSource _cancellationTokenSource = new();

        private bool _showDeleteDialog = false;
        private bool _showDeactivateDialog = false;
        private bool _showActivateDialog = false;
        private Guid _selectedPlayerId = Guid.Empty;
        private string? _selectedPlayerName;

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

            var isAdmin = CurrentUser?.IsInRole("Administrator") ?? false;
            CanCreate = isAdmin;
            CanEdit = isAdmin;
            CanDeactivate = isAdmin;
            CanDelete = isAdmin;
        }

        private async Task<TableData<PlayerDTO>> LoadServerData(TableState state, CancellationToken token)
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
                    SortBy = !string.IsNullOrEmpty(state.SortLabel) ? state.SortLabel : "LastName",
                    SortDirection = state.SortDirection == SortDirection.Descending ? "desc" : "asc",
                    SearchTerm = _searchTerm?.Trim()
                };

                var response = await PlayerService.GetPlayers(request);

                if (response.IsSuccess && response.Data != null)
                {
                    return new TableData<PlayerDTO>
                    {
                        Items = response.Data,
                        TotalItems = response.Pagination?.TotalCount ?? 0
                    };
                }
                else
                {
                    Snackbar.Add(response.Message ?? "Failed to load players", Severity.Error);
                    return new TableData<PlayerDTO> { Items = new List<PlayerDTO>(), TotalItems = 0 };
                }
            }
            catch (OperationCanceledException)
            {
                return new TableData<PlayerDTO> { Items = new List<PlayerDTO>(), TotalItems = 0 };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading players: {ex.Message}", Severity.Error);
                return new TableData<PlayerDTO> { Items = new List<PlayerDTO>(), TotalItems = 0 };
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
                await _table.ReloadServerData();
        }

        private void CreatePlayer() => NavigationManager.NavigateTo("/players/create");
        private void EditPlayer(Guid id) => NavigationManager.NavigateTo($"/players/edit/{id}");
        private void ViewPlayer(Guid id) => NavigationManager.NavigateTo($"/players/{id}");

        private void DeactivatePlayer(Guid id, string name)
        {
            _selectedPlayerId = id;
            _selectedPlayerName = name;
            _showDeactivateDialog = true;
        }

        private async Task ConfirmDeactivate()
        {
            try
            {
                var result = await PlayerService.DeactivatePlayer(_selectedPlayerId);
                if (result.IsSuccess)
                {
                    Snackbar.Add("Player deactivated successfully.", Severity.Success);
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to deactivate player", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deactivating player: {ex.Message}", Severity.Error);
            }
            finally
            {
                _showDeactivateDialog = false;
                _selectedPlayerId = Guid.Empty;
            }
        }

        private void ActivatePlayer(Guid id, string name)
        {
            _selectedPlayerId = id;
            _selectedPlayerName = name;
            _showActivateDialog = true;
        }

        private async Task ConfirmActivate()
        {
            try
            {
                var result = await PlayerService.ActivatePlayer(_selectedPlayerId);
                if (result.IsSuccess)
                {
                    Snackbar.Add("Player activated successfully.", Severity.Success);
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to activate player", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error activating player: {ex.Message}", Severity.Error);
            }
            finally
            {
                _showActivateDialog = false;
                _selectedPlayerId = Guid.Empty;
            }
        }

        private void DeletePlayer(Guid id, string name)
        {
            _selectedPlayerId = id;
            _selectedPlayerName = name;
            _showDeleteDialog = true;
        }

        private async Task ConfirmDelete()
        {
            try
            {
                var result = await PlayerService.DeletePlayer(_selectedPlayerId);
                if (result.IsSuccess)
                {
                    Snackbar.Add("Player deleted successfully.", Severity.Success);
                    await _table.ReloadServerData();
                }
                else
                {
                    Snackbar.Add(result.Message ?? "Failed to delete player", Severity.Error);
                }
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error deleting player: {ex.Message}", Severity.Error);
            }
            finally
            {
                _showDeleteDialog = false;
                _selectedPlayerId = Guid.Empty;
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

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
