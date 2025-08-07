using Microsoft.AspNetCore.Http;
using ReelkixVision.Web.Application.Interfaces;
using SixLabors.ImageSharp.Processing;

namespace ReelkixVision.Web.Infrastructure.Services
{
    public class ImageCompressionService : IImageCompressionService
    {
        public async Task<IFormFile> CompressImageIfNecessary(IFormFile file)
        {
            const long maxFileSize = 5 * 1024 * 1024; // 5 MB
            if (file.Length <= maxFileSize)
            {
                return file; // If the file is already small enough, return it as-is.
            }

            // Compress the image using ImageSharp
            var compressedStream = new MemoryStream();
            using (var inputStream = file.OpenReadStream())
            {
                var image = await SixLabors.ImageSharp.Image.LoadAsync(inputStream);
                image.Mutate(x => x.Resize(new SixLabors.ImageSharp.Processing.ResizeOptions
                {
                    Mode = SixLabors.ImageSharp.Processing.ResizeMode.Max,
                    Size = new SixLabors.ImageSharp.Size(1024, 1024) // Optional dimensions
                }));
                await image.SaveAsync(compressedStream, new SixLabors.ImageSharp.Formats.Jpeg.JpegEncoder
                {
                    Quality = 75 // Adjust compression level
                });
            }
            compressedStream.Seek(0, SeekOrigin.Begin);

            // Create a new IFormFile implementation using a custom wrapper
            var formFile = new FormFileWrapper(compressedStream, file.FileName, file.ContentType);

            return formFile;
        }
    }

    // Custom implementation of IFormFile
    public class FormFileWrapper : IFormFile
    {
        private readonly Stream _stream;

        public FormFileWrapper(Stream stream, string fileName, string contentType)
        {
            _stream = stream;
            FileName = fileName;
            ContentType = contentType;
            Headers = new HeaderDictionary();
        }

        public Stream OpenReadStream() => _stream;

        public string FileName { get; }
        public string ContentType { get; }
        public long Length => _stream.Length;
        public string Name => FileName; // Can adjust as per your requirement
        public IHeaderDictionary Headers { get; }

        public string ContentDisposition => throw new NotImplementedException();

        public void CopyTo(Stream target)
        {
            _stream.CopyTo(target);
        }

        public async Task CopyToAsync(Stream target, CancellationToken cancellationToken = default)
        {
            await _stream.CopyToAsync(target, cancellationToken);
        }
    }
}