namespace HRKošarka.Application.Images;

public interface IImageService
{
    Task<ImageData?> ProcessAsync(
        Stream fileStream,
        string fileName,
        string contentType,
        CancellationToken cancellationToken = default);
}
