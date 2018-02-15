using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class | AttributeTargets.Interface | AttributeTargets.Assembly,
    AllowMultiple = true)]
    public class ReportSuite : Attribute, ITestAction
    {
        public static ZafiraListener zafiraListener = new ZafiraListener();

        public ReportSuite()
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
