using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;

namespace ZafiraIntegration
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true)]
    public class ZafiraSuite : Attribute, ITestAction
    {
        public static ZafiraListener zafiraListener = new ZafiraListener();

        public ZafiraSuite()
        {            
        }

        public ActionTargets Targets { get; } = ActionTargets.Suite;

        public void BeforeTest(ITest test)
        {
            zafiraListener.onStart();
        }

        public void AfterTest(ITest test)
        {           
            zafiraListener.onFinish();
        }

       
    }
}
