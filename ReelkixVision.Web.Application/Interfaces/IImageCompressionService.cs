using Microsoft.AspNetCore.Http;

namespace ReelkixVision.Web.Application.Interfaces
{
    public interface IImageCompressionService
    {
        Task<IFormFile> CompressImageIfNecessary(IFormFile file);
    }
}
