using System;

namespace ZafiraIntegration
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class Maintainer : Attribute
    {
        internal string Username { get; }

        public Maintainer(string username)
        {
            Username = username;
        }
    }
}