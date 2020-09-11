using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZafiraAssemblyTest : Attribute, ITestAction
    {
        public ActionTargets Targets { get; } = ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            ZafiraAssembly.zafiraListener.OnTestStart(AttributeTargets.Assembly);
        }

        public void AfterTest(ITest test)
        {
            ZafiraAssembly.zafiraListener.OnTestFinish();
        }
    }
}