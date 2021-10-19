using System;

namespace ZebrunnerAgent.Attributes
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