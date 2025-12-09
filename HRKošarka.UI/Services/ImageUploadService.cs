using Blazored.LocalStorage;
using HRKošarka.UI.Contracts;
using HRKošarka.UI.Services.Base;
using Microsoft.AspNetCore.Components.Forms;

namespace HRKošarka.UI.Services
{
    public class ImageUploadService : BaseHttpService, IImageUploadService
    {
        public ImageUploadService(IClient client, ILocalStorageService localStorage)
            : base(client, localStorage) { }

        public async Task<(bool, string?, string, string, byte[])> ProcessAsync(IBrowserFile file)
        {
            try
            {
                await AddBearerToken();

                var stream = file.OpenReadStream(1_048_576); // 1 MB
                var fp = new FileParameter(stream, file.Name, file.ContentType);

                var response = await _client.ProcessAsync(fp);

                return (true, null, response.ImageName, response.ImageContentType, response.ImageBytes);
            }
            catch (ApiException ex)
            {
                return (false, ex.Message, string.Empty, string.Empty, Array.Empty<byte>());
            }
        }

    }
}
