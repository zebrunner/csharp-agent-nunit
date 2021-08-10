using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZebrunnerAgent.Registrar;

namespace ZebrunnerAgent.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZebrunnerTest : Attribute, ITestAction
    {
        private static readonly ITestRunRegistrar TestRunRegistrar = TestRunRegistrarFactory.GetTestRunRegistrar();
        public ActionTargets Targets => ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            TestRunRegistrar.RegisterTestRunStart(AttributeTargets.Class);
            TestRunRegistrar.RegisterTestStart(test);
        }

        public void AfterTest(ITest test)
        {
            TestRunRegistrar.RegisterTestFinish();
        }
    }
}