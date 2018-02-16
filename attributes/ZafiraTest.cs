using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;

namespace ZafiraIntegration
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true)]
    public class ZafiraTest : ZafiraSuite,  ITestAction
    {
        
        public ZafiraTest()
        {            
        }

        public ActionTargets Targets { get; } = ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
           zafiraListener.onTestStart();
        }

        public void AfterTest(ITest test)
        {
            zafiraListener.onTestFinish();           
        }
    }
}
