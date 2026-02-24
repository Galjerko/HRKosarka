using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Seasons
{
    public partial class Seasons : PermissionBaseComponent, IDisposable
    {
        [Inject] private ISeasonService SeasonService { get; set; } = default!;

        private MudTable<SeasonDTO> _table = default!;
        private string _searchTerm = string.Empty;
        private bool _loading = false;
        private CancellationTokenSource _cancellationTokenSource = new();
        private readonly int[] _pageSizeOptions = { 10, 25, 50 };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
            await SetListPermissions();
        }

        private async Task<TableData<SeasonDTO>> LoadServerData(TableState state, CancellationToken token)
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
                    PageSize = state.PageSize <= 0 ? 10 : state.PageSize,
                    SearchTerm = string.IsNullOrWhiteSpace(_searchTerm) ? null : _searchTerm.Trim()
                };

                var response = await SeasonService.GetSeasons(request);

                if (response.IsSuccess && response.Data != null)
                    return new TableData<SeasonDTO>
                    {
                        Items = response.Data,
                        TotalItems = response.Pagination?.TotalCount ?? 0
                    };

                Snackbar.Add(response.Message ?? "Failed to load seasons.", Severity.Error);
                return new TableData<SeasonDTO> { Items = new List<SeasonDTO>(), TotalItems = 0 };
            }
            catch (OperationCanceledException)
            {
                return new TableData<SeasonDTO> { Items = new List<SeasonDTO>(), TotalItems = 0 };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading seasons: {ex.Message}", Severity.Error);
                return new TableData<SeasonDTO> { Items = new List<SeasonDTO>(), TotalItems = 0 };
            }
            finally { _loading = false; StateHasChanged(); }
        }

        private async Task OnSearchChanged()
        {
            if (_table != null) await _table.ReloadServerData();
        }

        private async Task DeleteSeasonAsync(SeasonDTO season)
        {
            bool? confirm = await DialogService.ShowMessageBox(
                "Delete season",
                $"Delete season \"{season.Name}\"? This cannot be undone.",
                yesText: "Delete", cancelText: "Cancel");
            if (confirm != true) return;

            var result = await SeasonService.DeleteSeason(season.Id);
            if (!result.IsSuccess)
            {
                Snackbar.Add(result.Message ?? "Failed to delete season.", Severity.Error);
                return;
            }

            Snackbar.Add("Season deleted successfully.", Severity.Success);
            await _table.ReloadServerData();
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
