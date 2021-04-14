using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZafiraIntegration.Registrar;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZafiraClass : Attribute, ITestAction
    {
        private static readonly ITestRunRegistrar TestRunRegistrar = TestRunRegistrarFactory.GetTestRunRegistrar();
        public ActionTargets Targets => ActionTargets.Suite;

        public void BeforeTest(ITest test)
        {
            TestRunRegistrar.RegisterTestRunStart(AttributeTargets.Class);
        }

        public void AfterTest(ITest test)
        {
            TestRunRegistrar.RegisterTestRunFinish();
        }
    }
}