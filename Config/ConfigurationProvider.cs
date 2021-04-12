namespace ZafiraIntegration.Config
{
    public interface IConfigurationProvider
    {
        bool IsReportingEnabled();

        string GetServerHost();

        string GetAccessToken();

        string GetProjectKey();
    }
}