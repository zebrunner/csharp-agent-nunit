using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;
using NLog;
using NLog.Targets;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using ZafiraIntegration.Client;
using ZafiraIntegration.Client.Requests;
using ZafiraIntegration.Logging;

namespace ZafiraIntegration.Registrar
{
    public class ReportingRegistrar : ITestRunRegistrar
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

        private readonly ZebrunnerApiClient _apiClient;

        private ReportingRegistrar()
        {
            Target.Register<ZebrunnerNLogTarget>("Zebrunner");
            _apiClient = ZebrunnerApiClient.Instance;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void RegisterTestRunStart(AttributeTargets attributeTarget)
        {
            if (RunContext.SaveTestRunResponse == null)
            {
                var startTestRunRequest = new StartTestRunRequest
                {
                    Name = GetSuiteName(attributeTarget),
                    Framework = "nunit",
                    StartedAt = DateTime.UtcNow,
                    JenkinsContext = GetJenkinsContext()
                };
                var saveTestRunResponse = _apiClient.RegisterTestRunStart(startTestRunRequest);
                RunContext.SaveTestRunResponse = saveTestRunResponse;
            }
        }

        private string GetSuiteName(AttributeTargets attributeTarget)
        {
            var ciUrl = Environment.GetEnvironmentVariable("ci_url") ?? DefaultJobUrl;
            return attributeTarget == AttributeTargets.Assembly
                ? ciUrl.Substring(ciUrl.LastIndexOf('/') + 1)
                : TestContext.CurrentContext.Test.FullName;
        }

        private static StartTestRunRequest.JenkinsContextDto GetJenkinsContext()
        {
            return new StartTestRunRequest.JenkinsContextDto
            {
                JobUrl = Environment.GetEnvironmentVariable("ci_url") ?? DefaultJobUrl,
                JobNumber = Environment.GetEnvironmentVariable("ci_build"),
                ParentJobUrl = Environment.GetEnvironmentVariable("ci_parent_url") ?? DefaultParentJobUrl,
                ParentJobNumber = Environment.GetEnvironmentVariable("ci_parent_build")
            };
        }

        public void RegisterTestRunFinish()
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var finishTestRunRequest = new FinishTestRunRequest
            {
                EndedAt = DateTime.UtcNow
            };
            var saveTestRunResponse = _apiClient.RegisterTestRunFinish(testRunId, finishTestRunRequest);
            RunContext.SaveTestRunResponse = saveTestRunResponse;
        }

        public void RegisterTestStart()
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var testName = TestContext.CurrentContext.Test.ClassName + "." + TestContext.CurrentContext.Test.MethodName;

            var startTestRequest = new StartTestRequest
            {
                ClassName = TestContext.CurrentContext.Test.ClassName,
                MethodName = TestContext.CurrentContext.Test.MethodName,
                Name = testName,
                StartedAt = DateTime.UtcNow
            };
            var saveTestResponse = _apiClient.RegisterTestStart(testRunId, startTestRequest);
            RunContext.SetCurrentTest(saveTestResponse);

            var jobUrl = Environment.GetEnvironmentVariable("ci_url") ?? DefaultJobUrl;
            var jobNumber = Environment.GetEnvironmentVariable("ci_build");
            Artifact.AttachReferenceToTest("Demo", $"{jobUrl}/${jobNumber}/Screenshots/${testName}/report.html");
            Artifact.AttachReferenceToTest("Log", $"{jobUrl}/${jobNumber}/Logs/${testName}/test.log");
        }

        public void RegisterTestFinish()
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var testId = RunContext.GetCurrentTest().Id;
            var finishTestRequest = new FinishTestRequest
            {
                EndedAt = DateTime.UtcNow,
                Result = TestStatusToReason[TestContext.CurrentContext.Result.Outcome.Status],
                Reason = GetFullStackTrace()
            };
            _apiClient.RegisterTestFinish(testRunId, testId, finishTestRequest);
            RunContext.RemoveCurrentTest();
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