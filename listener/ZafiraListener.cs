using NLog;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Configuration;
using System.IO;
using System.Text;
using ZafiraIntegration.models;

namespace ZafiraIntegration
{
    public class ZafiraListener
    {


        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string ANONYMOUS = "anonymous";
        private string ZAFIRA_URL;
        private string ZAFIRA_ACCESS_TOKEN;
        private string ZAFIRA_PROJECT;
        private string ZAFIRA_REPORT_EMAILS;
        private CIConfig ci;
        private ZafiraClient zc;
        private UserType user;
        private JobType parentJob;
        private JobType job;
        private TestSuiteType suite;
        private TestRunType run;
        [ThreadStatic]
        private static TestType test;
        [ThreadStatic]
        private static TestCaseType testCase;

        private bool isEnabled = false;
        private bool isAvailable = false;
        private bool isInitialized = false;

        public void OnStart(AttributeTargets attributeTarget)
        {
            InitZafira(attributeTarget);
        }

        public void OnTestStart(AttributeTargets attributeTarget)
        {
            InitZafira(attributeTarget);

            if (isAvailable)
            {
                logger.Info("Zafira is Attempting to Register a New Tests...");
                var fullName = TestContext.CurrentContext.Test.FullName;
                var className = TestContext.CurrentContext.Test.ClassName;
                var methodName = TestContext.CurrentContext.Test.MethodName;
                var testName = className + "." + methodName;
                var descr = (String)TestContext.CurrentContext.Test.Properties.Get("Description");
                var testMethodFullName = (descr != null ? descr + " - " : "") + fullName;
                testCase = zc.createTestCase(new TestCaseType(fullName, testMethodFullName, "", suite.id, user.id));
                test = new TestType(testMethodFullName, Status.IN_PROGRESS.ToString(), "", run.id, testCase.id, DateTimeOffset.Now.ToUnixTimeMilliseconds(), null, 0, "");
                test.finishTime = null;
                test.artifacts.Add(new TestArtifactType("Demo", ci.getCiUrl() + "/" + ci.getCiBuild() + "/Screenshots/" + testName + "/report.html"));
                test.artifacts.Add(new TestArtifactType("Log", ci.getCiUrl() + "/" + ci.getCiBuild() + "/Logs/" + testName + "/test.log"));
                test = zc.startTest(test);
            } else {
                logger.Error("Zafira is not available!");
            }
        }

        public void OnTestFinish()
        {
            if (isAvailable)
            {
                if (TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Failed))
                {
                    test.status = Status.FAILED.ToString();
                }
                else if (TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Passed))
                {
                    test.status = Status.PASSED.ToString();
                }
                else if (TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Skipped))
                {
                    test.status = Status.SKIPPED.ToString();
                }
                else
                {
                    test.status = Status.UNKNOWN.ToString();
                }
                var finishedTest = zc.finishTest(PopulateTestResult(test));
            }
        }

        public void OnFinish()
        {
            if (isAvailable)
            {
                var finishedTestRun = zc.finishTestRun(run.id);
                var sentReport = zc.sendTestRunReport(run.id, ZAFIRA_REPORT_EMAILS, false, true);
                var className = TestContext.CurrentContext.Test.FullName;
                var folder = AppDomain.CurrentDomain.BaseDirectory;
                var pathToFolderWithReport = folder + "email_reports";
                var pathToReport = pathToFolderWithReport + "\\" + className + ".html";
                var exists = Directory.Exists(pathToFolderWithReport);
                if (!exists)
                {
                    Directory.CreateDirectory(pathToFolderWithReport);
                }
                File.WriteAllText(pathToReport, sentReport);
            }
        }

        private void InitZafira(AttributeTargets attributeTarget)
        {
            isEnabled = GetBoolean("zafira_enabled", false);
            if (!isEnabled)
            {
                logger.Info("Zafira is not enabled.");
                return;
            }

            if (isInitialized)
            {
                logger.Info("Zafira already initialized.");
                return;
            }

            try
            {
                ci = new CIConfig();
                ci.setCiRunId(GetString("ci_run_id", Guid.NewGuid().ToString()));
                ci.setCiUrl(GetString("ci_url", "http://localhost:8080/job/unavailable"));
                ci.setCiBuild(GetString("ci_build", null));
                ci.setCiBuildCause(GetString("ci_build_cause", "MANUALTRIGGER"));
                ci.setCiParentUrl(GetString("ci_parent_url", "http://localhost:8080/job/unavailable/parent"));
                ci.setCiParentBuild(GetString("ci_parent_build", null));
                ci.setCiUserId(GetString("ci_user_id", ANONYMOUS));
                ci.setCiUserFirstName(GetString("ci_user_first_name", null));
                ci.setCiUserLastName(GetString("ci_user_last_name", null));
                ci.setCiUserEmail(GetString("ci_user_email", null));
                ci.setGitBranch(GetString("git_branch", null));
                ci.setGitCommit(GetString("git_commit", null));
                ci.setGitUrl(GetString("git_url", null));

                ZAFIRA_URL = GetString("zafira_service_url", "");
                ZAFIRA_ACCESS_TOKEN = GetString("zafira_access_token", "");
                ZAFIRA_PROJECT = GetString("zafira_project", "UNKNOWN");
                ZAFIRA_REPORT_EMAILS = GetString("zafira_report_emails", "").Trim().Replace(" ", ",").Replace(";", ",");

                logger.Info("Attempting to init Zafira Client...");
                zc = new ZafiraClient(ZAFIRA_URL, ZAFIRA_PROJECT);

                isAvailable = zc.isAvailable();
                if (isAvailable)
                {
                    logger.Info("Zafira is available. Refreshing authToken...");
                    var auth = zc.refreshToken(ZAFIRA_ACCESS_TOKEN);
                    if (auth != null)
                    {
                        zc.AuthToken = auth.authTokenType + " " + auth.authToken;
                        logger.Info("Zafira authToken is: " + zc.AuthToken);
                    }
                    else
                    {
                        isAvailable = false;
                        logger.Info("Unable to get Zafira authToken!");
                        return;
                    }
                }

                logger.Info("Zafira is " + (isAvailable ? "available" : "unavailable"));
            }
            catch (Exception e)
            {
                logger.Error("Unable to init ZafiraClient!", e);
            }

            user = zc.getUser(ci.getCiUserId());
            suite = zc.createTestSuite(new TestSuiteType(GetSuiteName(attributeTarget), "--", user.id));
            parentJob = zc.registerJob(ci.getCiParentUrl(), user.id);
            job = zc.registerJob(ci.getCiUrl(), user.id);
            String configXml = "<config>"
                + "<arg unique=\"false\"><key>platform</key><value>" + Environment.GetEnvironmentVariable("platform") + "</value></arg>"
                + "<arg unique=\"false\"><key>env</key><value>" + Environment.GetEnvironmentVariable("env") + "</value></arg>"
                + "</config>";
            run = zc.startTestRun(new TestRunType("", suite.id, user.id, "", "", "", configXml, job.id, parentJob.id, ci.getCiBuild(), Initiator.HUMAN.ToString(), ""));

            isInitialized = true;

            return;
        }

        private static String GetString(String key, String defaultValue)
        {
            String result = Environment.GetEnvironmentVariable(key);
            return (result != null) ? result : defaultValue;
        }

        private static Boolean GetBoolean(String key, Boolean defaultValue)
        {
            String result = Environment.GetEnvironmentVariable(key);
            logger.Info("Key (" + key + ") Value: " + result);
            return (result != null) ? true : defaultValue;
        }


        private TestType PopulateTestResult(TestType test)
        {
            test.message = GetFullStackTrace();
            test.finishTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return test;
        }

        private string GetSuiteName(AttributeTargets attributeTarget)
        {
            switch (attributeTarget)
            {
                case AttributeTargets.Class:
                    return TestContext.CurrentContext.Test.FullName;
                case AttributeTargets.Assembly:
                    string ciUrl = ci.getCiUrl();
                    return ciUrl != null ? ciUrl.Substring(ciUrl.LastIndexOf('/') + 1) : "Unknown";
            }
            throw new Exception("Unable to resolve zafira test suite name. Please check ZafiraSuite attribute usage.");
        }

        private string GetFullStackTrace()
        {
            StringBuilder myStringBuilder = new StringBuilder();
            myStringBuilder.AppendLine(TestContext.CurrentContext.Result.Message);
            myStringBuilder.AppendLine("Stacktrace: " + TestContext.CurrentContext.Result.StackTrace);
            return myStringBuilder.ToString();
        }
    }
}