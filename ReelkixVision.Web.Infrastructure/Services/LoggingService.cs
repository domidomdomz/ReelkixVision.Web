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

        public async Task LogRequestAsync(RequestLog log, ShoeAnalysisResult? analysisResult = null)
        {
            // Only log if the feature flag is enabled.
            if (!_featureFlagService.IsDatabaseLoggingEnabled())
                return;

            // If there's an analysis result, link it via the navigation property.
            if (analysisResult != null)
            {
                log.ShoeAnalysisResult = analysisResult; // This establishes the relationship.
            }

            // Add the RequestLog (and the related ShoeAnalysisResult).
            _dbContext.RequestLogs.Add(log);

            // Save both in one transaction.
            await _dbContext.SaveChangesAsync();
        }
    }
}