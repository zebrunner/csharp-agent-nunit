using ZafiraIntegration.Config;

namespace ZafiraIntegration.Registrar
{
    internal static class TestRunRegistrarFactory
    {
        internal static ITestRunRegistrar GetTestRunRegistrar()
        {
            return Configuration.IsReportingEnabled()
                ? ReportingRegistrar.Instance
                : NoOpTestRunRegistrar.Instance as ITestRunRegistrar;
        }
    }
}