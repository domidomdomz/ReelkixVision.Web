using ReelkixVision.Web.Application.Interfaces;
using ReelkixVision.Web.Domain.Entities;
using ReelkixVision.Web.Infrastructure.Persistence;

namespace ReelkixVision.Web.Infrastructure.Services
{
    public class LoggingService : ILoggingService
    {
        private readonly AppDbContext _dbContext;
        private readonly IFeatureFlagService _featureFlagService;

        public LoggingService(AppDbContext dbContext, IFeatureFlagService featureFlagService)
        {
            _dbContext = dbContext;
            _featureFlagService = featureFlagService;
        }

        public async Task LogRequestAsync(RequestLog log)
        {
            // Only log if the feature flag is enabled.
            if (!_featureFlagService.IsDatabaseLoggingEnabled())
                return;

            _dbContext.RequestLogs.Add(log);
            await _dbContext.SaveChangesAsync();
        }
    }
}