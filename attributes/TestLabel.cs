using System;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true)]
    public class TestLabel : Attribute
    {
        internal string Key { get; }
        internal string[] Values { get; }

        public TestLabel(string key, params string[] values)
        {
            Key = key;
            Values = values;
        }
    }
}