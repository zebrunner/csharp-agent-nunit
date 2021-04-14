using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZafiraIntegration.Registrar;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZafiraAssemblyTest : Attribute, ITestAction
    {
        private static readonly ITestRunRegistrar TestRunRegistrar = TestRunRegistrarFactory.GetTestRunRegistrar();
        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            TestRunRegistrar.RegisterTestRunStart(AttributeTargets.Assembly);
            TestRunRegistrar.RegisterTestStart(test);
        }

        public void AfterTest(ITest test)
        {
            TestRunRegistrar.RegisterTestFinish();
        }
    }
}