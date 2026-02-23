using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Components.Pages.Dialogs;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using HRKošarka.UI.Services.Base.Common.Requests;
using HRKošarka.UI.Services.Base.Common.Responses;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.Users
{
    public partial class Users : PermissionBaseComponent, IDisposable
    {
        [Inject] public IUserService UserService { get; set; } = default!;

        private MudTable<NonAdminUserDTO> _table = default!;
        private string _searchTerm = string.Empty;
        private bool _loading = false;
        private CancellationTokenSource _cancellationTokenSource = new();
        private readonly int[] _pageSizeOptions = { 10, 25, 50, 100 };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private async Task<TableData<NonAdminUserDTO>> LoadServerData(TableState state, CancellationToken token)
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

                PaginatedResponse<NonAdminUserDTO> response =
                    await UserService.GetUsers(request);

                if (response.IsSuccess && response.Data != null)
                {
                    return new TableData<NonAdminUserDTO>
                    {
                        Items = response.Data,
                        TotalItems = response.Pagination?.TotalCount ?? 0
                    };
                }

                Snackbar.Add(response.Message ?? "Failed to load users", Severity.Error);

                return new TableData<NonAdminUserDTO>
                {
                    Items = new List<NonAdminUserDTO>(),
                    TotalItems = 0
                };
            }
            catch (OperationCanceledException)
            {
                return new TableData<NonAdminUserDTO>
                {
                    Items = new List<NonAdminUserDTO>(),
                    TotalItems = 0
                };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading users: {ex.Message}", Severity.Error);
                return new TableData<NonAdminUserDTO>
                {
                    Items = new List<NonAdminUserDTO>(),
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

        private async Task OpenAssignClubDialog(NonAdminUserDTO user)
        {
            var parameters = new DialogParameters
            {
                { nameof(AssignClubManager.User), user }
            };

            var options = new DialogOptions
            {
                CloseButton = true,
                MaxWidth = MaxWidth.Small,
                FullWidth = true
            };

            var dialog = DialogService.Show<AssignClubManager>(
                "Assign club manager",
                parameters,
                options);

            var result = await dialog.Result;

            if (!result.Canceled)
            {
                Snackbar.Add("Club manager assigned successfully.", Severity.Success);
                await _table.ReloadServerData();
            }
        }


        private async Task RemoveClubManagerAsync(NonAdminUserDTO user)
        {
            bool? confirm = await DialogService.ShowMessageBox(
                "Remove club manager",
                $"Remove {user.FullName} as club manager for {user.ManagedClubName} and lock the account?",
                yesText: "Remove",
                cancelText: "Cancel");

            if (confirm != true)
            {
                return;
            }


            try
            {
                var result = await UserService.RemoveClubManager(user.Id);

                if (!result.IsSuccess)
                {
                    Snackbar.Add(result.Message ?? "Failed to remove club manager.", Severity.Error);
                    return;
                }

                Snackbar.Add("Club manager removed and user locked out.", Severity.Success);
                await _table.ReloadServerData();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error removing club manager: {ex.Message}", Severity.Error);
            }
        }

        private async Task LockOutUserAsync(NonAdminUserDTO user)
        {
            bool? confirm = await DialogService.ShowMessageBox(
                "Lock out user",
                $"Lock out {user.FullName}? They will not be able to sign in.",
                yesText: "Lock out",
                cancelText: "Cancel");

            if (confirm != true)
                return;

            try
            {
                var result = await UserService.LockUser(user.Id);

                if (!result.IsSuccess)
                {
                    Snackbar.Add(result.Message ?? "Failed to lock out user.", Severity.Error);
                    return;
                }

                Snackbar.Add("User locked out.", Severity.Success);
                await _table.ReloadServerData();
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error locking out user: {ex.Message}", Severity.Error);
            }
        }


        public void Dispose()
        {
            _cancellationTokenSource?.Cancel();
            _cancellationTokenSource?.Dispose();
        }
    }
}
