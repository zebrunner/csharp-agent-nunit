using System;
using ZafiraIntegration.Client.Requests;

namespace ZafiraIntegration.Registrar
{
    public interface ITestSessionRegistrar
    {
        void RegisterTestSessionStart(string sessionId, TestSessionStart testSessionStart);

        void RegisterTestSessionFinish(string sessionId, DateTime endedAt);

        void LinkAllCurrentToTest(long zebrunnerTestId);

        void LinkToCurrentTest(string sessionId);
    }
}