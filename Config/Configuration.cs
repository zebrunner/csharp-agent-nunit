using System;
using System.Collections.Generic;
using System.Linq;
using ZafiraIntegration.Config.Provider;

namespace ZafiraIntegration.Config
{
    internal static class Configuration
    {
        private static readonly List<IConfigurationProvider> ConfigurationProviders = new List<IConfigurationProvider>
        {
            new EnvironmentVariablesConfigurationProvider()
        };

        internal static bool IsReportingEnabled()
        {
            return ConfigurationProviders
                .Select(provider => provider.IsReportingEnabled())
                .FirstOrDefault(isReportingEnabled => isReportingEnabled);
        }

        internal static string GetServerHost()
        {
            return GetStringProperty(provider => provider.GetServerHost());
        }

        internal static string GetAccessToken()
        {
            return GetStringProperty(provider => provider.GetAccessToken());
        }

        internal static string GetProjectKey()
        {
            return GetStringProperty(provider => provider.GetProjectKey());
        }

        internal static string GetEnvironment()
        {
            return GetStringProperty(provider => provider.GetEnvironment());
        }

        internal static string GetBuild()
        {
            return GetStringProperty(provider => provider.GetBuild());
        }

        private static string GetStringProperty(Func<IConfigurationProvider, string> propertyResolver)
        {
            return ConfigurationProviders
                .Select(propertyResolver)
                .FirstOrDefault(property => property != null && property.Trim().Length != 0);
        }
    }
}