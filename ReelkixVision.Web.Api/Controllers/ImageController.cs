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
        private readonly IImageCompressionService _imageCompressionService;

        public ImageController(
            IImageService imageService, 
            ILoggingService loggingService, 
            IAnalysisService analysisService, 
            IImageCompressionService imageCompressionService)
        {
            _imageService = imageService;
            _loggingService = loggingService;
            _analysisService = analysisService;
            _imageCompressionService = imageCompressionService;
        }

        [HttpPost("upload")]
        public async Task<IActionResult> UploadImage(IFormFile file)
        {
            if (!IsValidFile(file))
            {
                return Problem(
                    detail: "Only JPG and PNG files are allowed.",
                    statusCode: 400,
                    title: "Invalid File Type"
                );
            }

            // Step 1: Compress and prepare the file (if required)
            var compressedStream = await _imageCompressionService.CompressImageIfNecessary(file);

            // Step 2: Upload to AWS S3
            string imageUrl;
            try
            {
                imageUrl = await _imageService.UploadImageAsync(compressedStream);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: $"Failed to upload image: {ex.Message}",
                    statusCode: 500,
                    title: "Image Upload Error"
                );
            }

            // Step 3: Analyze the image
            ShoeAnalysisResultDto analysisResult;
            try
            {
                analysisResult = await _analysisService.AnalyzeImageAsync(imageUrl);
            }
            catch (Exception ex)
            {
                return Problem(
                    detail: $"Failed to analyze image: {ex.Message}",
                    statusCode: 500,
                    title: "Image Analysis Error"
                );
            }

            // Step 4: Log the request
            await LogRequest(file.FileName, imageUrl, analysisResult);

            return Ok(new
            {
                Message = "Image uploaded, compressed, and analyzed successfully.",
                ImageUrl = imageUrl,
                Analysis = analysisResult
            });
        }

        private bool IsValidFile(IFormFile file)
        {
            return file != null && (file.ContentType == "image/jpeg" || file.ContentType == "image/png");
        }

        private async Task LogRequest(string fileName, string imageUrl, ShoeAnalysisResultDto analysisResult)
        {
            var requestLog = new RequestLog
            {
                RequestTime = DateTime.UtcNow,
                FileName = fileName,
                Url = imageUrl
            };

            var analysis = new ShoeAnalysisResult
            {
                Brand = analysisResult.Brand,
                Model = analysisResult.Model,
                Colorway = analysisResult.Colorway,
                Sku = analysisResult.Sku,
                Text = analysisResult.Text
            };

            await _loggingService.LogRequestAsync(requestLog, analysis);
        }
    }
}