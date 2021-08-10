using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZebrunnerAgent.Registrar;

namespace ZebrunnerAgent.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZebrunnerAssemblyTest : Attribute, ITestAction
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