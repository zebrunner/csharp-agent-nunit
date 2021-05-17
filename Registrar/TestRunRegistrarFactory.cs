﻿using ZafiraIntegration.Config;

namespace ZafiraIntegration.Registrar
{
    public static class TestRunRegistrarFactory
    {
        public static ITestRunRegistrar GetTestRunRegistrar()
        {
            return Configuration.IsReportingEnabled()
                ? ReportingRegistrar.Instance
                : NoOpTestRunRegistrar.Instance as ITestRunRegistrar;
        }
    }
}
