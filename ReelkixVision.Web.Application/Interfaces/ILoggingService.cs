using ReelkixVision.Web.Domain.Entities;

namespace ReelkixVision.Web.Application.Interfaces
{
    public interface ILoggingService
    {
        Task LogRequestAsync(RequestLog log);
    }
}