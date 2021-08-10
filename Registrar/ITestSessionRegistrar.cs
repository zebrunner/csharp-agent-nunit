using System;

namespace ZebrunnerAgent.Registrar
{
    public interface ITestSessionRegistrar
    {
        void RegisterTestSessionStart(string sessionId, TestSessionStart testSessionStart);

        void RegisterTestSessionFinish(string sessionId, DateTime endedAt);

        void LinkAllCurrentToTest(long zebrunnerTestId);

        void LinkToCurrentTest(string sessionId);
    }
}