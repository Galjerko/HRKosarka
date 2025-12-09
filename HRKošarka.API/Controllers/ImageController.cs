using HRKošarka.Application.Images;
using Microsoft.AspNetCore.Mvc;

namespace HRKošarka.API.Controllers
{
    [Route("api/images")]
    [ApiController]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;

        public ImageController(IImageService imageService)
        {
            _imageService = imageService;
        }

        [HttpPost("process")]
        public async Task<ActionResult<ImageData>> Process(
            IFormFile file,
            CancellationToken cancellationToken)
        {
            if (file is not { Length: > 0 })
            {
                return BadRequest("No file.");
            }

            await using var stream = file.OpenReadStream();
            var imageData = await _imageService.ProcessAsync(
                stream,
                file.FileName,
                file.ContentType,
                cancellationToken);

            if (imageData is null)
            {
                return BadRequest("Invalid image type or size.");
            }

            return Ok(imageData);
        }
    }
}
