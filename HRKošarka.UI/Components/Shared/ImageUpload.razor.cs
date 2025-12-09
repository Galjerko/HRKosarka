using HRKošarka.UI.Contracts;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using MudBlazor;

namespace HRKošarka.UI.Components.Shared
{
    public partial class ImageUpload
    {
        [Parameter] public string Label { get; set; } = string.Empty;
        [Parameter] public EventCallback<(string? name, string? contentType, byte[]? bytes)> OnImageChanged { get; set; }

        [Parameter] public string? InitialImageName { get; set; }
        [Parameter] public string? InitialImageContentType { get; set; }
        [Parameter] public byte[]? InitialImageBytes { get; set; }

        [Inject] private IImageUploadService ImageUploadService { get; set; } = default!;
        [Inject] private ISnackbar Snackbar { get; set; } = default!;

        private string? ImagePreviewUrl { get; set; }
        private string? Error { get; set; }

        private bool _initialisedFromParameters;

        protected override void OnParametersSet()
        {
            if (_initialisedFromParameters)
                return;

            if (InitialImageBytes != null &&
                InitialImageBytes.Length > 0 &&
                !string.IsNullOrWhiteSpace(InitialImageContentType))
            {
                ImagePreviewUrl =
                    $"data:{InitialImageContentType};base64,{Convert.ToBase64String(InitialImageBytes)}";
            }
            else
            {
                ImagePreviewUrl = null;
            }

            _initialisedFromParameters = true;
        }

        private async Task OnFilesChanged(IBrowserFile file)
        {
            Error = null;
            if (file is null)
                return;

            var (isSuccess, errorMessage, name, contentType, bytes) =
                await ImageUploadService.ProcessAsync(file);

            if (!isSuccess || bytes == null || bytes.Length == 0 || string.IsNullOrWhiteSpace(contentType))
            {
                Error = errorMessage ?? "Failed to upload image";
                Snackbar.Add(Error, Severity.Error);
                ImagePreviewUrl = null;
                await OnImageChanged.InvokeAsync((null, null, null));
                return;
            }

            ImagePreviewUrl = $"data:{contentType};base64,{Convert.ToBase64String(bytes)}";
            await OnImageChanged.InvokeAsync((name, contentType, bytes));
            Snackbar.Add("Image uploaded successfully", Severity.Success);
        }

        private async Task OnDeleteImage()
        {
            ImagePreviewUrl = null;
            Error = null;
            await OnImageChanged.InvokeAsync((null, null, null));
            Snackbar.Add("Image removed", Severity.Info);
        }
    }

}
