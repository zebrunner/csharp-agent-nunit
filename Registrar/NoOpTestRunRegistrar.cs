using System;
using NUnit.Framework.Interfaces;

namespace ZebrunnerAgent.Registrar
{
    internal class NoOpTestRunRegistrar : ITestRunRegistrar
    {
        internal static NoOpTestRunRegistrar Instance { get; } = new NoOpTestRunRegistrar();

        private NoOpTestRunRegistrar()
        {
        }

        public void RegisterTestRunStart(AttributeTargets attributeTarget)
        {
        }

        public void RegisterTestRunFinish()
        {
        }

        public void RegisterTestStart(ITest test)
        {
        }

        public void RegisterTestFinish()
        {
        }
    }
}