using System.Collections.Generic;
using System.Linq;
using ZafiraIntegration.Config.Provider;

namespace ZafiraIntegration.Config
{
    public static class Configuration
    {
        private static readonly List<IConfigurationProvider> ConfigurationProviders = new List<IConfigurationProvider>
        {
            new EnvironmentVariablesConfigurationProvider()
        };

        public static bool IsReportingEnabled()
        {
            return ConfigurationProviders
                .Select(provider => provider.IsReportingEnabled())
                .First(isReportingEnabled => isReportingEnabled);
        }

        public static string GetServerHost()
        {
            return ConfigurationProviders
                .Select(provider => provider.GetServerHost())
                .First(serverHost => serverHost != null && serverHost.Trim().Length != 0);
        }

        public static string GetAccessToken()
        {
            return ConfigurationProviders
                .Select(provider => provider.GetAccessToken())
                .First(accessToken => accessToken != null && accessToken.Trim().Length != 0);
        }

        public static string GetProjectKey()
        {
            return ConfigurationProviders
                .Select(provider => provider.GetProjectKey())
                .First(projectKey => projectKey != null && projectKey.Trim().Length != 0);
        }
    }
}