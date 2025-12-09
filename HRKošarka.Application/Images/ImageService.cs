using Microsoft.Extensions.Options;

namespace HRKošarka.Application.Images
{
    public sealed class ImageService : IImageService
    {
        private readonly ImageSettings _settings;

        public ImageService(IOptions<ImageSettings> options)
        {
            _settings = options.Value;
        }

        public async Task<ImageData?> ProcessAsync(
            Stream fileStream,
            string fileName,
            string contentType,
            CancellationToken cancellationToken = default)
        {
            if (!ImageContentTypes.Allowed.Contains(contentType, StringComparer.OrdinalIgnoreCase))
            {
                return null;
            }


            using var ms = new MemoryStream();
            await fileStream.CopyToAsync(ms, cancellationToken);

            if (ms.Length == 0 || ms.Length > _settings.MaxBytes)
            {
                return null;
            }

            return new ImageData
            {
                ImageName = fileName,
                ImageContentType = contentType,
                ImageBytes = ms.ToArray()
            };
        }
    }
}