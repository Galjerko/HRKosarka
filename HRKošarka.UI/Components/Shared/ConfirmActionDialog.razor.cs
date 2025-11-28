using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace HRKošarka.UI.Components.Shared
{
    public partial class ConfirmActionDialog
    {
        [Parameter] public bool Visible { get; set; }
        [Parameter] public EventCallback<bool> VisibleChanged { get; set; }

        [Parameter] public string? Title { get; set; }
        [Parameter] public string? TitleIcon { get; set; }
        [Parameter] public Color TitleIconColor { get; set; } = Color.Error;

        [Parameter] public string? Message { get; set; }
        [Parameter] public string? SecondaryMessage { get; set; }
        [Parameter] public Color SecondaryMessageColor { get; set; } = Color.Default;

        [Parameter] public string CancelText { get; set; } = "Cancel";
        [Parameter] public Color CancelColor { get; set; } = Color.Default;
        [Parameter] public Variant CancelVariant { get; set; } = Variant.Text;

        [Parameter] public string ConfirmText { get; set; } = "OK";
        [Parameter] public string? ConfirmIcon { get; set; }
        [Parameter] public Color ConfirmColor { get; set; } = Color.Primary;
        [Parameter] public Variant ConfirmVariant { get; set; } = Variant.Filled;

        [Parameter] public bool IsProcessing { get; set; }
        [Parameter] public EventCallback OnConfirm { get; set; }

        [Parameter] public DialogOptions? Options { get; set; }

        private async Task OnCancelClicked()
        {
            await VisibleChanged.InvokeAsync(false);
        }

        private async Task OnConfirmClicked()
        {
            if (OnConfirm.HasDelegate)
                await OnConfirm.InvokeAsync();
        }
    }
}
