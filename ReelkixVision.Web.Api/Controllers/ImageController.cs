using Microsoft.AspNetCore.Mvc;
using ReelkixVision.Web.Application.DTOs;
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
        private readonly IAnalysisService _analysisService;

        public ImageController(IImageService imageService, ILoggingService loggingService, IAnalysisService analysisService)
        {
            _imageService = imageService;
            _loggingService = loggingService;
            _analysisService = analysisService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (file == null || (file.ContentType != "image/jpeg" && file.ContentType != "image/png"))
            {
                return BadRequest("Only JPG and PNG files are allowed.");
            }

            // Step 1: Upload the image to AWS S3.
            var imageUrl = await _imageService.UploadImageAsync(file);

            // Step 2: Call the Node.js AI-powered API to analyze the image.
            ShoeAnalysisResultDto analysisResult;
            try
            {
                analysisResult = await _analysisService.AnalyzeImageAsync(imageUrl);
            }
            catch (Exception ex)
            {
                // Handle the error as needed.
                return StatusCode(500, "Failed to analyze image: " + ex.Message);
            }

            // Step 2: Optionally log the request if logging is enabled.
            await _loggingService.LogRequestAsync(new RequestLog
            {
                RequestTime = DateTime.UtcNow,
                FileName = file.FileName,
                Url = imageUrl,
                ResponseDetails = analysisResult.Text
            });

            // Return both the image URL and the analysis result.
            return Ok(new
            {
                ImageUrl = imageUrl,
                Analysis = analysisResult
            });
        }
    }
}