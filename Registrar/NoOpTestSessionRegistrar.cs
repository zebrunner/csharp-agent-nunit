using System;
using ZafiraIntegration.Client.Requests;

namespace ZafiraIntegration.Registrar
{
    internal class NoOpTestSessionRegistrar : ITestSessionRegistrar
    {
        internal static NoOpTestSessionRegistrar Instance { get; } = new NoOpTestSessionRegistrar();

        private NoOpTestSessionRegistrar()
        {
        }

        public void RegisterTestSessionStart(string sessionId, TestSessionStart testSessionStart)
        {
        }

        public void RegisterTestSessionUpdate(string sessionId, DateTime endedAt)
        {
        }

        public void LinkAllCurrentToTest(long zebrunnerTestId)
        {
        }

        public void LinkToCurrentTest(string sessionId)
        {
        }
    }
}