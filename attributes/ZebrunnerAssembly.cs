using System;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZebrunnerAgent.Registrar;

namespace ZebrunnerAgent.Attributes
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ZebrunnerAssembly : Attribute, ITestAction
    {
        private static readonly ITestRunRegistrar TestRunRegistrar = TestRunRegistrarFactory.GetTestRunRegistrar();

        public ActionTargets Targets => ActionTargets.Suite;

        public void BeforeTest(ITest test)
        {
            TestRunRegistrar.RegisterTestRunStart(AttributeTargets.Assembly);
        }

        public void AfterTest(ITest test)
        {
            TestRunRegistrar.RegisterTestRunFinish();
        }
    }
}