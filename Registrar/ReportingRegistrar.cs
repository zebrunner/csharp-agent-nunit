using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;
using NLog;
using NLog.Targets;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZafiraIntegration.Client;
using ZafiraIntegration.Client.Requests;
using ZafiraIntegration.Config;
using ZafiraIntegration.Logging;

namespace ZafiraIntegration.Registrar
{
    internal class ReportingRegistrar : ITestRunRegistrar
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private const string DefaultJobUrl = "http://localhost:8080/job/unavailable";
        private const string DefaultParentJobUrl = "http://localhost:8080/job/unavailable/parent";
        internal static ReportingRegistrar Instance { get; } = new ReportingRegistrar();

        private static readonly Dictionary<TestStatus, string> TestStatusToReason = new Dictionary<TestStatus, string>
        {
            {TestStatus.Passed, "PASSED"},
            {TestStatus.Failed, "FAILED"},
            {TestStatus.Inconclusive, "ABORTED"},
            {TestStatus.Skipped, "SKIPPED"},
            {TestStatus.Warning, "FAILED"}
        };

        private readonly ZebrunnerApiClient _apiClient = ZebrunnerApiClient.Instance;

        private ReportingRegistrar()
        {
            Target.Register<ZebrunnerNLogTarget>("Zebrunner");
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RegisterTestRunStart(AttributeTargets attributeTarget)
        {
            if (RunContext.GetCurrentTestRun() == null)
            {
                Logger.Debug("Registering test run start...");
                var startTestRunRequest = new StartTestRunRequest
                {
                    Name = GetSuiteName(attributeTarget),
                    Framework = "nunit",
                    StartedAt = DateTime.UtcNow,
                    JenkinsContext = GetJenkinsContext(),
                    Config = new StartTestRunRequest.ConfigDto
                    {
                        Environment = Configuration.GetEnvironment(),
                        Build = Configuration.GetBuild()
                    }
                };
                var saveTestRunResponse = _apiClient.RegisterTestRunStart(startTestRunRequest);
                RunContext.SetCurrentTestRun(saveTestRunResponse);

                Logger.Debug("Test run start was registered successfuly.");
            }
        }

        private string GetSuiteName(AttributeTargets attributeTarget)
        {
            var ciUrl = (Environment.GetEnvironmentVariable("ci_url") ?? DefaultJobUrl).TrimEnd('/');
            return attributeTarget == AttributeTargets.Assembly
                ? ciUrl.Substring(ciUrl.LastIndexOf('/') + 1)
                : TestContext.CurrentContext.Test.FullName;
        }

        private static StartTestRunRequest.JenkinsContextDto GetJenkinsContext()
        {
            return new StartTestRunRequest.JenkinsContextDto
            {
                JobUrl = (Environment.GetEnvironmentVariable("ci_url") ?? DefaultJobUrl).TrimEnd('/'),
                JobNumber = Environment.GetEnvironmentVariable("ci_build"),
                ParentJobUrl = Environment.GetEnvironmentVariable("ci_parent_url") ?? DefaultParentJobUrl,
                ParentJobNumber = Environment.GetEnvironmentVariable("ci_parent_build")
            };
        }

        public void RegisterTestRunFinish()
        {
            Logger.Debug("Registering test run finish...");
            var testRun = RunContext.GetCurrentTestRun();
            if (testRun != null)
            {
                var finishTestRunRequest = new FinishTestRunRequest
                {
                    EndedAt = DateTime.UtcNow
                };
                var saveTestRunResponse = _apiClient.RegisterTestRunFinish(testRun.Id, finishTestRunRequest);
                RunContext.SetCurrentTestRun(saveTestRunResponse);
                
                Logger.Debug("Test run finish was registered successfuly.");
            }
        }

        public void RegisterTestStart(ITest test)
        {
            var testRunId = RunContext.GetCurrentTestRun()?.Id;
            var testName = TestContext.CurrentContext.Test.ClassName + "." + TestContext.CurrentContext.Test.MethodName;
            Logger.Debug($"Registering test start for test run ${testRunId}...");

            if (testRunId != null)
            {
                var startTestRequest = new StartTestRequest
                {
                    ClassName = TestContext.CurrentContext.Test.ClassName,
                    MethodName = TestContext.CurrentContext.Test.MethodName,
                    Name = testName,
                    StartedAt = DateTime.UtcNow,
                    Maintainer = MaintainerResolver.ResolveMaintainer(test),
                    Labels = LabelsResolver.ResolveLabels(test)
                };
                var saveTestResponse = _apiClient.RegisterTestStart((long) testRunId, startTestRequest);
                RunContext.SetCurrentTest(saveTestResponse);

                var jobUrl = (Environment.GetEnvironmentVariable("ci_url") ?? DefaultJobUrl).TrimEnd('/');
                var jobNumber = Environment.GetEnvironmentVariable("ci_build");
                Artifact.AttachReferenceToTest("Demo", $"{jobUrl}/${jobNumber}/Screenshots/${testName}/report.html");
                Artifact.AttachReferenceToTest("Log", $"{jobUrl}/${jobNumber}/Logs/${testName}/test.log");
                
                Logger.Debug("Test start was registered successfuly.");
            }
        }

        public void RegisterTestFinish()
        {
            var testRunId = RunContext.GetCurrentTestRun()?.Id;
            var testId = RunContext.GetCurrentTest()?.Id;
            Logger.Debug($"Registering test finish for test run with id ${testRunId} and test with id ${testId}...");

            if (testRunId != null && testId != null)
            {
                var finishTestRequest = new FinishTestRequest
                {
                    EndedAt = DateTime.UtcNow,
                    Result = TestStatusToReason[TestContext.CurrentContext.Result.Outcome.Status],
                    Reason = GetFullStackTrace()
                };
                _apiClient.RegisterTestFinish((long) testRunId, (long) testId, finishTestRequest);
                RunContext.RemoveCurrentTest();
                
                Logger.Debug("Test finish was registered successfuly.");
            }
        }

        private string GetFullStackTrace()
        {
            var status = TestContext.CurrentContext.Result.Outcome.Status;
            if (status == TestStatus.Failed || status == TestStatus.Warning || status == TestStatus.Inconclusive)
            {
                var builder = new StringBuilder();
                builder.AppendLine(TestContext.CurrentContext.Result.Message);
                builder.AppendLine("Stacktrace: " + TestContext.CurrentContext.Result.StackTrace);
                return builder.ToString();
            }

            return null;
        }
    }
}