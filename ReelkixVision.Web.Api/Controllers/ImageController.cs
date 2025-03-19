using Microsoft.AspNetCore.Mvc;
using ReelkixVision.Web.Application.Interfaces;
using ReelkixVision.Web.Domain.Entities;

namespace ReelkixVision.Web.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ImageController : ControllerBase
    {
        private readonly IImageService _imageService;
        private readonly ILoggingService _loggingService;

        public ImageController(IImageService imageService, ILoggingService loggingService)
        {
            _imageService = imageService;
            _loggingService = loggingService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            // Validate file existence and type (JPG and PNG only)
            if (file == null || (file.ContentType != "image/jpeg" && file.ContentType != "image/png"))
            {
                return BadRequest("Only JPG and PNG files are allowed.");
            }

            // Upload the file to AWS S3.
            var imageUrl = await _imageService.UploadImageAsync(file);

            // Log the request/response only if logging is enabled.
            await _loggingService.LogRequestAsync(new RequestLog
            {
                RequestTime = DateTime.UtcNow,
                FileName = file.FileName,
                Url = imageUrl,
                ResponseDetails = imageUrl
            });

            return Ok(new { Url = imageUrl });
        }
    }
}