using ZafiraIntegration.Config;

namespace ZafiraIntegration.Registrar
{
    public static class TestSessionRegistrarFactory
    {
        public static ITestSessionRegistrar GetTestSessionRegistrar()
        {
            return Configuration.IsReportingEnabled()
                ? SessionRegistrar.Instance
                : NoOpTestSessionRegistrar.Instance as ITestSessionRegistrar;
        }
    }
}
