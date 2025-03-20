using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Http;
using ReelkixVision.Web.Application.Interfaces;

namespace ReelkixVision.Web.Infrastructure.AWS
{
    public class ImageService : IImageService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IConfiguration _configuration;

        public ImageService(IAmazonS3 amazonS3, IConfiguration configuration)
        {
            _amazonS3 = amazonS3;
            _configuration = configuration;
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Read bucket name and create a unique file name.
            string bucketName = _configuration["AWS:BucketName"] ?? "";
            string region = _configuration["AWS:Region"] ?? "";
            string uploadFolder = _configuration["AWS:UploadFolder"] ?? "";
            string fileName = $"{Guid.NewGuid()}-{file.FileName}";

            using var stream = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = fileName,
                InputStream = stream,
                ContentType = file.ContentType
            };

            await _amazonS3.PutObjectAsync(request);
            return $"https://{bucketName}.s3.{region}.amazonaws.com/{uploadFolder}/{fileName}";
        }
    }
}