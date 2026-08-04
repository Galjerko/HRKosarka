using HRKošarka.UI.Components.Base;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Pages.Admin.EmailNotifications
{
    public partial class EmailNotifications : PermissionBaseComponent
    {
        [Inject] public IEmailNotificationService EmailNotificationService { get; set; } = default!;

        private MudTable<EmailNotificationDTO> _table = default!;
        private bool _loading = false;
        private NotificationType? _notificationTypeFilter;
        private bool? _successFilter;
        private readonly int[] _pageSizeOptions = { 10, 25, 50, 100 };

        protected override async Task OnInitializedAsync()
        {
            await base.OnInitializedAsync();
        }

        private async Task<TableData<EmailNotificationDTO>> LoadServerData(TableState state, CancellationToken token)
        {
            _loading = true;
            StateHasChanged();

            try
            {
                var response = await EmailNotificationService.GetEmailNotifications(
                    state.Page + 1,
                    state.PageSize <= 0 ? 10 : state.PageSize,
                    _notificationTypeFilter,
                    _successFilter);

                if (response.IsSuccess && response.Data != null)
                {
                    return new TableData<EmailNotificationDTO>
                    {
                        Items = response.Data,
                        TotalItems = response.Pagination?.TotalCount ?? 0
                    };
                }

                Snackbar.Add(response.Message ?? "Failed to load email notifications", Severity.Error);

                return new TableData<EmailNotificationDTO>
                {
                    Items = new List<EmailNotificationDTO>(),
                    TotalItems = 0
                };
            }
            catch (Exception ex)
            {
                Snackbar.Add($"Error loading email notifications: {ex.Message}", Severity.Error);
                return new TableData<EmailNotificationDTO>
                {
                    Items = new List<EmailNotificationDTO>(),
                    TotalItems = 0
                };
            }
            finally
            {
                _loading = false;
                StateHasChanged();
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
            _notificationTypeFilter = null;
            _successFilter = null;
            await OnFilterChanged();
        }

        private async Task ViewContent(EmailNotificationDTO notification)
        {
            await DialogService.ShowMessageBox(
                notification.Subject,
                (MarkupString)notification.Body,
                yesText: "Close");
        }
    }
}
