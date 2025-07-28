using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Shared
{
    public partial class AuditHistory
    {
        [Parameter] public DateTimeOffset DateCreated { get; set; }
        [Parameter] public DateTimeOffset? DateModified { get; set; }
        [Parameter] public DateTimeOffset? DateDeleted { get; set; }
        [Parameter] public string? CreatedBy { get; set; }
        [Parameter] public string? ModifiedBy { get; set; }
        [Parameter] public string? DeletedBy { get; set; }

        private bool _showDialog = false;
        private readonly DialogOptions _dialogOptions = new()
        {
            MaxWidth = MaxWidth.Medium,
            FullWidth = true
        };

        private void ShowHistory()
        {
            _showDialog = true;
        }
        private void CloseDialog()
        {
            _showDialog = false;
        }
    }
}
