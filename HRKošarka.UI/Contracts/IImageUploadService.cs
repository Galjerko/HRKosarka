using Microsoft.AspNetCore.Components.Forms;

namespace HRKošarka.UI.Contracts
{
    public interface IImageUploadService
    {
        Task<(bool IsSuccess, string? ErrorMessage, string Name, string ContentType, byte[] Bytes)>
            ProcessAsync(IBrowserFile file);
    }
}
