using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Users
{
    public partial class InactiveUsers : PermissionBaseComponent, IDisposable
    {
        [Inject] public IUserService UserService { get; set; } = default!;

        private MudTable<InactiveUserDTO> _table = default!;
        private string _searchTerm = string.Empty;
        private bool _loading = false;
        private CancellationTokenSource _cancellationTokenSource = new();
        private readonly int[] _pageSizeOptions = { 10, 25, 50, 100 };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private async Task<TableData<InactiveUserDTO>> LoadServerData(TableState state, CancellationToken token)
        {
            _loading = true;
            StateHasChanged();

            try
            {
                _cancellationTokenSource?.Cancel();
                _cancellationTokenSource = new CancellationTokenSource();

                using var linkedCts = CancellationTokenSource.CreateLinkedTokenSource(
                    token, _cancellationTokenSource.Token);

                var request = new PaginationRequest
                {
                    Page = state.Page + 1,
                    PageSize = state.PageSize <= 0 ? 10 : state.PageSize,
                    SearchTerm = string.IsNullOrWhiteSpace(_searchTerm) ? null : _searchTerm.Trim()
                };

                PaginatedResponse<InactiveUserDTO> response =
                    await UserService.GetInactiveUsers(request);

                if (response.IsSuccess && response.Data != null)
                {
                    return new TableData<InactiveUserDTO>
                    {
                        Items = response.Data,
                        TotalItems = response.Pagination?.TotalCount ?? 0
                    };
                }

                Snackbar.Add(response.Message ?? "Failed to load inactive users", Severity.Error);

                return new TableData<InactiveUserDTO>
                {
                    Items = new List<InactiveUserDTO>(),
                    TotalItems = 0
                };
            }
            catch (OperationCanceledException)
            {
                return new TableData<InactiveUserDTO>
                {
                    Items = new List<InactiveUserDTO>(),
                    TotalItems = 0
                };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading inactive users: {ex.Message}", Severity.Error);
                return new TableData<InactiveUserDTO>
                {
                    Items = new List<InactiveUserDTO>(),
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
                await _table.ReloadServerData();
        }

        private async Task UnlockUserAsync(InactiveUserDTO user)
        {
            bool? confirm = await DialogService.ShowMessageBox(
                "Unlock user",
                $"Unlock {user.FullName}? They will be able to sign in again.",
                yesText: "Unlock",
                cancelText: "Cancel");

            if (confirm != true)
                return;

            try
            {
                var result = await UserService.UnlockUser(user.Id);

                if (!result.IsSuccess)
                {
                    Snackbar.Add(result.Message ?? "Failed to unlock user.", Severity.Error);
                    return;
                }

                Snackbar.Add("User unlocked successfully.", Severity.Success);
                await _table.ReloadServerData();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error unlocking user: {ex.Message}", Severity.Error);
            }
        }

        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
