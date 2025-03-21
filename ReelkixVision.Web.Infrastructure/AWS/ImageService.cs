using Amazon.Runtime;
using Amazon.S3;
using Amazon.S3.Model;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using ReelkixVision.Web.Application.Interfaces;

namespace ReelkixVision.Web.Infrastructure.AWS
{
    public class ImageService : IImageService
    {
        private readonly IAmazonS3 _amazonS3;
        private readonly IConfiguration _configuration;

        public ImageService(IConfiguration configuration)
        {
            _configuration = configuration;

            // Retrieve environment variables or throw an exception if not set
            var awsAccessKeyId = Environment.GetEnvironmentVariable("AWS_ACCESS_KEY_ID");
            var awsSecretAccessKey = Environment.GetEnvironmentVariable("AWS_SECRET_ACCESS_KEY");
            if (string.IsNullOrEmpty(awsAccessKeyId) || string.IsNullOrEmpty(awsSecretAccessKey))
            {
                throw new Exception("AWS credentials are not set in the environment variables.");
            }

            // Create the S3 client with explicit credentials and region
            var credentials = new BasicAWSCredentials(awsAccessKeyId, awsSecretAccessKey);
            var region = _configuration["AWS:Region"] ?? "us-east-1";
            var regionEndpoint = Amazon.RegionEndpoint.GetBySystemName(region);
            _amazonS3 = new AmazonS3Client(credentials, regionEndpoint);
        }

        public async Task<string> UploadImageAsync(IFormFile file)
        {
            // Read bucket name and create a unique file name
            string bucketName = _configuration["AWS:BucketName"] ?? throw new Exception("AWS:BucketName is not set in configuration.");
            string uploadFolder = _configuration["AWS:UploadFolder"] ?? "";
            string fileName = $"{Guid.NewGuid()}-{file.FileName}";
            string key = string.IsNullOrEmpty(uploadFolder) ? fileName : $"{uploadFolder}/{fileName}";

            using var stream = file.OpenReadStream();
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = key,
                InputStream = stream,
                ContentType = file.ContentType
            };

            await _amazonS3.PutObjectAsync(request);
            return $"https://{bucketName}.s3.{_configuration["AWS:Region"]}.amazonaws.com/{key}";
        }
    }
}