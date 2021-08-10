using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZebrunnerAgent.Registrar;

namespace ZebrunnerAgent.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZebrunnerClass : Attribute, ITestAction
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