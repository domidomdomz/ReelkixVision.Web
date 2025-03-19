using Microsoft.AspNetCore.Http;

namespace ReelkixVision.Web.Application.Interfaces
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile file);
    }
}