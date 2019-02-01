using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Assembly)]
    public class ZafiraAssembly : Attribute, ITestAction
    {
        public static ZafiraListener zafiraListener = new ZafiraListener();

        public ActionTargets Targets { get; } = ActionTargets.Suite;

        public void BeforeTest(ITest test)
        {
            zafiraListener.OnStart(AttributeTargets.Assembly);
        }

        public void AfterTest(ITest test)
        {
            zafiraListener.OnFinish();
        }
    }
}