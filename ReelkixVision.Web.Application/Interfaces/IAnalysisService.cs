using ReelkixVision.Web.Application.DTOs;

namespace ReelkixVision.Web.Application.Interfaces
{
    public interface IAnalysisService
    {
        Task<ShoeAnalysisResultDto> AnalyzeImageAsync(string imageUrl);
    }
}