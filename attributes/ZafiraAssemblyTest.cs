using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZafiraIntegration.Registrar;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZafiraAssemblyTest : Attribute, ITestAction
    {
        private static readonly ITestRunRegistrar TestRunRegistrar = ReportingRegistrar.Instance;
        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            TestRunRegistrar.RegisterTestRunStart(AttributeTargets.Assembly);
            TestRunRegistrar.RegisterTestStart();
        }

        public void AfterTest(ITest test)
        {
            TestRunRegistrar.RegisterTestFinish();
        }
    }
}