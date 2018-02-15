using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;

namespace ZafiraIntegration
{
    [AttributeUsage( AttributeTargets.Class, AllowMultiple = true)]
    public class ReportTest : ReportSuite,  ITestAction
    {
        
        public ReportTest()
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
