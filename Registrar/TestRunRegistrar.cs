using System;

namespace ZafiraIntegration.Registrar
{
    public interface ITestRunRegistrar
    {
        void RegisterTestRunStart(AttributeTargets attributeTarget);

        void RegisterTestRunFinish();

        void RegisterTestStart();

        void RegisterTestFinish();
    }
}