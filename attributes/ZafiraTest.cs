using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class ZafiraTest : Attribute, ITestAction
    {
        [ThreadStatic]
        private static ZafiraListener _zafiraListener;

        public static ZafiraListener zafiraListener
        {
            get
            {
                if (_zafiraListener == null)
                {
                    _zafiraListener = new ZafiraListener();
                }
                return _zafiraListener;
            }
        }

        public ActionTargets Targets { get; } = ActionTargets.Test;

        public void BeforeTest(ITest test)
        {
            zafiraListener.OnTestStart(AttributeTargets.Class);
        }

        public void AfterTest(ITest test)
        {
            zafiraListener.OnTestFinish();
        }
    }
}