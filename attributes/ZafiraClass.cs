using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZafiraClass : Attribute, ITestAction
    {
        private ZafiraListener zafiraListener;

        public ActionTargets Targets { get; } = ActionTargets.Suite;

        public void BeforeTest(ITest test)
        {
            zafiraListener = ZafiraTest.zafiraListener;
            zafiraListener.OnStart(AttributeTargets.Class);
        }

        public void AfterTest(ITest test)
        {
            zafiraListener.OnFinish();
        }
    }
}