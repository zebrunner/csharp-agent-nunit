using System;

namespace ZafiraIntegration.Config.Provider
{
    internal class EnvironmentVariablesConfigurationProvider : IConfigurationProvider
    {
        private const string ReportingEnabledLegacyEnvironmentVariable = "zafira_enabled";
        private const string ReportingEnabledEnvironmentVariable = "REPORTING_ENABLED";
        private const string ServerHostLegacyEnvironmentVariable = "zafira_service_url";
        private const string ServerHostEnvironmentVariable = "REPORTING_SERVER_HOSTNAME";
        private const string AccessTokenLegacyEnvironmentVariable = "zafira_access_token";
        private const string AccessTokenEnvironmentVariable = "REPORTING_SERVER_ACCESS_TOKEN";
        private const string ProjectKeyLegacyEnvironmentVariable = "zafira_project";
        private const string ProjectKeyEnvironmentVariable = "REPORTING_PROJECT_KEY";
        private const string EnvironmentEnvironmentVariable = "REPORTING_RUN_ENVIRONMENT";
        private const string BuildEnvironmentVariable = "REPORTING_RUN_BUILD";

        private const string DefaultProjectKey = "UNKNOWN";

        public bool IsReportingEnabled()
        {
            var reportingEnabled = Environment.GetEnvironmentVariable(ReportingEnabledLegacyEnvironmentVariable)
                                   ?? Environment.GetEnvironmentVariable(ReportingEnabledEnvironmentVariable);
            return string.Compare("true", reportingEnabled, StringComparison.OrdinalIgnoreCase) == 0;
        }

        public string GetServerHost()
        {
            return Environment.GetEnvironmentVariable(ServerHostLegacyEnvironmentVariable)
                   ?? Environment.GetEnvironmentVariable(ServerHostEnvironmentVariable);
        }

        public string GetAccessToken()
        {
            return Environment.GetEnvironmentVariable(AccessTokenLegacyEnvironmentVariable)
                   ?? Environment.GetEnvironmentVariable(AccessTokenEnvironmentVariable);
        }

        public string GetProjectKey()
        {
            return Environment.GetEnvironmentVariable(ProjectKeyLegacyEnvironmentVariable)
                   ?? Environment.GetEnvironmentVariable(ProjectKeyEnvironmentVariable)
                   ?? DefaultProjectKey;
        }

        public string GetEnvironment()
        {
            return Environment.GetEnvironmentVariable(EnvironmentEnvironmentVariable);
        }

        public string GetBuild()
        {
            return Environment.GetEnvironmentVariable(BuildEnvironmentVariable);
        }
    }
}