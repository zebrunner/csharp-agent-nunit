using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using NLog;
using ZebrunnerAgent.Client;
using ZebrunnerAgent.Client.Requests;
using ZebrunnerAgent.Client.Responses;

namespace ZebrunnerAgent.Registrar
{
    internal class SessionRegistrar : ITestSessionRegistrar
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        internal static SessionRegistrar Instance { get; } = new SessionRegistrar();

        private readonly ZebrunnerApiClient _apiClient = ZebrunnerApiClient.Instance;

        private readonly IDictionary<string, SaveTestSessionResponse> _sessionIdToSession =
            new ConcurrentDictionary<string, SaveTestSessionResponse>();

        private readonly ThreadLocal<ISet<string>> _threadSessionIds = new ThreadLocal<ISet<string>>(
            () => new HashSet<string>()
        );

        private SessionRegistrar()
        {
        }

        public void RegisterTestSessionStart(string sessionId, TestSessionStart testSessionStart)
        {
            var testRun = RunContext.GetCurrentTestRun();
            if (testRun != null)
            {
                Log($"({testRun.Id}) Registering test session start...");
                var startTestSessionRequest = new StartTestSessionRequest
                {
                    SessionId = sessionId,
                    InitiatedAt = testSessionStart.StartedAt.ToUniversalTime(),
                    StartedAt = testSessionStart.StartedAt.ToUniversalTime(),
                    Status = "RUNNING",
                    DesiredCapabilities = testSessionStart.DesiredCapabilities,
                    Capabilities = testSessionStart.Capabilities
                };

                var test = RunContext.GetCurrentTest();
                if (test != null)
                {
                    startTestSessionRequest.TestIds = new HashSet<long>
                    {
                        test.Id
                    };
                }

                var response = _apiClient.RegisterTestSessionStart(testRun.Id, startTestSessionRequest);

                _sessionIdToSession[sessionId] = response;
                _threadSessionIds.Value.Add(sessionId);

                Log($"({testRun.Id}, {response.Id}) Test session start was registered successfully.");
            }
        }

        public void RegisterTestSessionFinish(string sessionId, DateTime endedAt)
        {
            var testRun = RunContext.GetCurrentTestRun();
            var testSession = _sessionIdToSession[sessionId];
            if (testRun != null && testSession != null)
            {
                Log($"({testRun.Id}, {testSession.Id}) Registering test session update...");
                var request = new UpdateTestSessionRequest
                {
                    EndedAt = endedAt.ToUniversalTime()
                };
                _apiClient.RegisterTestSessionUpdate(testRun.Id, testSession.Id, request);

                _sessionIdToSession.Remove(sessionId);
                _threadSessionIds.Value.Remove(sessionId);

                Log($"({testRun.Id}, {testSession.Id}) Test session update was registered successfully.");
            }
        }

        public void LinkAllCurrentToTest(long zebrunnerTestId)
        {
            foreach (var sessionId in _threadSessionIds.Value)
            {
                Link(sessionId, zebrunnerTestId);
            }
        }

        public void LinkToCurrentTest(string sessionId)
        {
            var currentTest = RunContext.GetCurrentTest();
            if (currentTest != null)
            {
                Link(sessionId, currentTest.Id);
            }
        }

        private void Link(string sessionId, long zebrunnerTestId)
        {
            var testRun = RunContext.GetCurrentTestRun();
            var testSession = _sessionIdToSession[sessionId];
            if (testRun != null && testSession != null)
            {
                if (testSession.TestIds.Add(zebrunnerTestId))
                {
                    Log($"({testRun.Id}) Linking test with id {zebrunnerTestId} to session {sessionId}.");

                    var request = new UpdateTestSessionRequest
                    {
                        TestIds = new HashSet<long>
                        {
                            zebrunnerTestId
                        }
                    };
                    _apiClient.RegisterTestSessionUpdate(testRun.Id, testSession.Id, request);

                    Log($"({testRun.Id}) Session {sessionId} was linked to test {zebrunnerTestId} successfully.");
                }
            }
        }

        private static void Log(string message)
        {
            Logger.Debug($"{DateTime.UtcNow} [{Thread.CurrentThread.Name}] {message}");
        }
    }
}