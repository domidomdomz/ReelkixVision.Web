using Microsoft.Extensions.Configuration;
using ReelkixVision.Web.Application.Interfaces;

namespace ReelkixVision.Web.Infrastructure.FeatureFlags
{
    public class FeatureFlagService : IFeatureFlagService
    {
        private readonly IConfiguration _configuration;

        public FeatureFlagService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public bool IsDatabaseLoggingEnabled()
        {
            return bool.Parse(_configuration["FeatureFlags:DatabaseLogging"] ?? "false");
        }
    }
}