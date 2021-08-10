using System;
using NUnit.Framework.Interfaces;

namespace ZebrunnerAgent.Registrar
{
    public interface ITestRunRegistrar
    {
        void RegisterTestRunStart(AttributeTargets attributeTarget);

        void RegisterTestRunFinish();

        void RegisterTestStart(ITest test);

        void RegisterTestFinish();
    }
}
