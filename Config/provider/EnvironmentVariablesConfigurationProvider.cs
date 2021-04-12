using System;

namespace ZafiraIntegration.Config.Provider
{
    public class EnvironmentVariablesConfigurationProvider : IConfigurationProvider
    {
        private const string ReportingEnabledEnvironmentVariable = "zafira_enabled";
        private const string ServerHostEnvironmentVariable = "zafira_service_url";
        private const string AccessTokenEnvironmentVariable = "zafira_access_token";
        private const string ProjectKeyEnvironmentVariable = "zafira_project";

        private const string DefaultProjectKey = "UNKNOWN";

        public bool IsReportingEnabled()
        {
            var reportingEnabled = Environment.GetEnvironmentVariable(ReportingEnabledEnvironmentVariable);
            return string.Compare("true", reportingEnabled, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public string GetServerHost()
        {
            return Environment.GetEnvironmentVariable(ServerHostEnvironmentVariable);
        }

        public string GetAccessToken()
        {
            return Environment.GetEnvironmentVariable(AccessTokenEnvironmentVariable);
        }

        public string GetProjectKey()
        {
            var projectKey = Environment.GetEnvironmentVariable(ProjectKeyEnvironmentVariable);
            return projectKey ?? DefaultProjectKey;
        }
    }
}