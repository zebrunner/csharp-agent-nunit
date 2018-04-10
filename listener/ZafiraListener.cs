using NLog;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using System;
using System.Configuration;
using System.IO;
using ZafiraIntegration.models;

namespace ZafiraIntegration
{
    public class ZafiraListener
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();
        private static string ANONYMOUS = "anonymous";
        private string JIRA_SUITE_ID;
        private bool ZAFIRA_ENABLED;
        private string ZAFIRA_URL;
        private string ZAFIRA_ACCESS_TOKEN;
        private string ZAFIRA_PROJECT;
        private string ZAFIRA_REPORT_EMAILS;
        private string ZAFIRA_REPORT_FOLDER;
        private bool ZAFIRA_RERUN_FAILURES;
        private bool ZAFIRA_REPORT_SHOW_STACKTRACE;
        private bool ZAFIRA_REPORT_SHOW_FAILURES_ONLY;
        private string ZAFIRA_CONFIGURATOR;
        private CIConfig ci;
        private ZafiraClient zc;
        private UserType user;
        private JobType parentJob;
        private JobType job;
        private TestSuiteType suite;
        private TestRunType run;
        private TestType test;
        private TestCaseType testCase;

        [OneTimeSetUp]
        public void onStart()
        {
            if (initializeZafira())
            {
                user = zc.createUser(new UserType(ci.getCiUserId(), ci.getCiUserEmail(), ci.getCiUserFirstName(),
                        ci.getCiUserLastName()));
                suite = zc.createTestSuite(new TestSuiteType(TestContext.CurrentContext.Test.FullName, "--", user.id));

                parentJob = zc.registerJob(ci.getCiParentUrl(), user.id);
                job = zc.registerJob(ci.getCiUrl(), user.id);
                String configXml = "<config><arg unique=\"false\"><key>platform</key><value>" + Environment.GetEnvironmentVariable("platform") + "</value></arg></config>";
                run = zc.startTestRun(new TestRunType("", suite.id, user.id, "", "", "", configXml, job.id, parentJob.id, ci.getCiBuild(), Initiator.HUMAN.ToString(), ""));
            }
        }

        [SetUp]
        public void onTestStart()
        {
            if (ZAFIRA_ENABLED)
            {
                var className = TestContext.CurrentContext.Test.FullName;
                var methodName = TestContext.CurrentContext.Test.MethodName;
                var descr = (String)TestContext.CurrentContext.Test.Properties.Get("Description");
                var testMethodFullName = (descr != null ? descr + " - " : "") + className;
                testCase = zc.createTestCase(new TestCaseType(className, testMethodFullName, "", suite.id, user.id));
                test = zc.startTest(populateTestResult(new TestType(testMethodFullName, Status.IN_PROGRESS.ToString(), "", run.id, testCase.id, 0, null, 0, "")));
            }
        }

        [TearDown]
        public void onTestFinish()
        {
            if (ZAFIRA_ENABLED)
            {
                if (TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Failed))
                {
                    test.status = Status.FAILED.ToString();
                }
                else if (TestContext.CurrentContext.Result.Outcome.Status.Equals(TestStatus.Passed))
                {
                    test.status = Status.PASSED.ToString();
                }
                else
                {
                    test.status = Status.UNKNOWN.ToString();
                }
                var finishedTest = zc.finishTest(test);
                var finishedTestRun = zc.finishTestRun(run.id);
            }
        }

        [OneTimeTearDown]
        public void onFinish()
        {
            if (ZAFIRA_ENABLED)
            {
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

        private Boolean initializeZafira()
        {
            Boolean success = false;
            try
            {
                ci = new CIConfig();
                ci.setCiRunId(getString("ci_run_id", Guid.NewGuid().ToString()));
                ci.setCiUrl(getString("ci_url", "http://localhost:8080/job/unavailable"));
                ci.setCiBuild(getString("ci_build", null));
                ci.setCiBuildCause(getString("ci_build_cause", "MANUALTRIGGER"));
                ci.setCiParentUrl(getString("ci_parent_url", "http://localhost:8080/job/unavailable/parent"));
                ci.setCiParentBuild(getString("ci_parent_build", null));
                ci.setCiUserId(getString("ci_user_id", ANONYMOUS));
                ci.setCiUserFirstName(getString("ci_user_first_name", null));
                ci.setCiUserLastName(getString("ci_user_last_name", null));
                ci.setCiUserEmail(getString("ci_user_email", null));
                ci.setGitBranch(getString("git_branch", null));
                ci.setGitCommit(getString("git_commit", null));
                ci.setGitUrl(getString("git_url", null));

                JIRA_SUITE_ID = getString("jira_suite_id", null);
                ZAFIRA_ENABLED = getBoolean("zafira_enabled", false);
                ZAFIRA_URL = getString("zafira_service_url", "http://demo.qaprosoft.com/zafira-ws");
                ZAFIRA_ACCESS_TOKEN = getString("zafira_access_token", "eyJhbGciOiJIUzUxMiJ9.eyJzdWIiOiIyIiwicGFzc3dvcmQiOiIySDl5ZVhNcWoxb1lLVm1WZlYxY28vZ3ZYdmRHejdxTiIsImV4cCI6MTMwMzY3NTM4MTk1fQ.yqp4BJd7OpgX7aOdQOjGKdYb2DvHK2ds6ilc0MoO6p_vkbZhkjIK-eCr8dhT7Riwj8x6ru0Lup6Zj-FithCfOw");
                ZAFIRA_PROJECT = getString("zafira_project", "DemoTest");
                ZAFIRA_REPORT_EMAILS = getString("zafira_report_emails", "demoqaprosoft@gmail.com").Trim().Replace(" ", ",").Replace(";", ",");
                ZAFIRA_REPORT_FOLDER = getString("zafira_report_folder", "FOLDER_PATH");
                ZAFIRA_RERUN_FAILURES = getBoolean("zafira_rerun_failures", false);
                ZAFIRA_REPORT_SHOW_STACKTRACE = getBoolean("zafira_report_show_stacktrace", true);
                ZAFIRA_REPORT_SHOW_FAILURES_ONLY = getBoolean("zafira_report_failures_only", false);
                ZAFIRA_CONFIGURATOR = getString("zafira_configurator", "com.qaprosoft.zafira.listener.DefaultConfigurator");

                if (ZAFIRA_ENABLED)
                {
                    zc = new ZafiraClient(ZAFIRA_URL, ZAFIRA_ACCESS_TOKEN, ZAFIRA_PROJECT);

                    ZAFIRA_ENABLED = zc.isAvailable();

                    if (ZAFIRA_ENABLED)
                    {
                        var auth = zc.refreshToken(ZAFIRA_ACCESS_TOKEN);
                        if (auth != null)
                        {
                            zc.AuthToken = auth.type + " " + auth.accessToken;
                        }
                        else
                        {
                            ZAFIRA_ENABLED = false;
                        }
                    }

                    logger.Info("Zafira is " + (ZAFIRA_ENABLED ? "available" : "unavailable"));
                }
                success = ZAFIRA_ENABLED;
            }
            catch (ConfigurationException e)
            {
                logger.Error("Unable to locate init ZafiraClient ", e);
            }
            return success;
        }

        private static String getString(String key, String defaultValue)
        {
            String result = Environment.GetEnvironmentVariable(key);
            return (result != null) ? result : defaultValue;
        }

        private static Boolean getBoolean(String key, Boolean defaultValue)
        {
            String result = Environment.GetEnvironmentVariable(key);
            return (result != null) ? true : defaultValue;
        }

        private TestType populateTestResult(TestType test)
        {
            String testName = TestContext.CurrentContext.Test.ClassName + "." + TestContext.CurrentContext.Test.MethodName;
            test.artifacts.Add(new TestArtifactType("Demo", ci.getCiUrl() + "/" + ci.getCiBuild() + "/Screenshots/" + testName + "/report.html"));
            test.artifacts.Add(new TestArtifactType("Log", ci.getCiUrl() + "/" + ci.getCiBuild() + "/Logs/" + testName + "/test.log"));
            return test;
        }

    }
}