using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZafiraIntegration.Registrar;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZafiraTest : Attribute, ITestAction
    {
        private static readonly ITestRunRegistrar TestRunRegistrar = ReportingRegistrar.Instance;
        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            TestRunRegistrar.RegisterTestRunStart(AttributeTargets.Class);
            TestRunRegistrar.RegisterTestStart();
        }

        public void AfterTest(ITest test)
        {
            TestRunRegistrar.RegisterTestFinish();
        }
    }
}