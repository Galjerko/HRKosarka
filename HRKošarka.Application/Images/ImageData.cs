namespace HRKošarka.Application.Images
{
    public sealed class ImageData
    {
        public required string ImageName { get; init; }
        public required string ImageContentType { get; init; }
        public required byte[] ImageBytes { get; init; }
    }
}
