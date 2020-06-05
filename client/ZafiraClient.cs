using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using ZafiraIntegration.models;
using NLog;
using static ZafiraIntegration.ApiRequest;
using Newtonsoft.Json;
using ZafiraIntegration.http;

namespace ZafiraIntegration
{
    public class ZafiraClient
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();
        private static string STATUS_PATH = "/api/status";
        private static string REFRESH_TOKEN_PATH = "/api/auth/refresh";
        private static string USERS_PATH = "/api/users/profile?username={0}";
        private static string JOBS_PATH = "/api/jobs";
        private static string TESTS_PATH = "/api/tests";
        private static string TEST_FINISH_PATH = "/api/tests/{0}/finish";
        private static string TEST_WORK_ITEMS_PATH = "/api/tests/{0}/workitems";
        private static string TEST_SUITES_PATH = "/api/tests/suites";
        private static string TEST_CASES_PATH = "/api/tests/cases";
        private static string TEST_RUNS_PATH = "/api/tests/runs";
        private static string TEST_RUNS_FINISH_PATH = "/api/tests/runs/{0}/finish";
        private static string TEST_RUN_EMAIL_PATH = "/api/tests/runs/{0}/email?filter={1}&showStacktrace={2}";
        public string ServiceURL { get; set; }
        public string AuthToken { get; set; }
        public string Project { get; set; }

        public ZafiraClient(string url, string authToken, string project)
        {
            ServiceURL = url;
            AuthToken = authToken;
            Project = project;
        }

        public ApiRequest Client()
        {
            return new ApiRequest
            {
                URL = ServiceURL,
                AccessToken = AuthToken,
                Project = Project
            };
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public AuthTokenType refreshToken(String token)
        {
            var response = new AuthTokenType();
            try
            {
                LOGGER.Info("Refresh Token Before Refresh: " + token);
                var postRefreshToken = Client().Execute(REFRESH_TOKEN_PATH, new RefreshTokenType(token), Method.POST.ToString());
                var status = ((ResponseStatus)postRefreshToken).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<AuthTokenType>(((ResponseStatus)postRefreshToken).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to create user " + e);
            }
            return response;
        }

        public bool isAvailable()
        {
            var isAvailable = false;
            try
            {
                LOGGER.Info("Is Available Path: " + STATUS_PATH);
                var response = Client().Execute(STATUS_PATH);
                LOGGER.Info("Attempting to get Response Status..");
                var status = ((ResponseStatus)response).StatusCode;
                LOGGER.Info("Response Status of is Available: " + status);

                if (status.Equals("200") || status.Equals("OK"))
                {
                    isAvailable = true;
                }
            }
            catch (Exception e)
            {
                LOGGER.Error(e, "Unable to send ping");
                LOGGER.Error("Error Message: " + e.StackTrace.ToString());
            }
            return isAvailable;
        }

        public JobType registerJob(String jobUrl, long userId)
        {
            var jobName = jobUrl.Split('/').Last();
            var jenkinsHost = "";
            if (jobUrl.Contains("/view/"))
            {
                jenkinsHost = jobUrl.Split(new[] { "/view/" }, StringSplitOptions.None)[0];
            }
            else if (jobUrl.Contains("/job/"))
            {
                jenkinsHost = jobUrl.Split(new[] { "/job/" }, StringSplitOptions.None)[0];
            }

            var jobDetails = "jobName: {0}, jenkinsHost: {1}, userId: {2}";
            LOGGER.Info("Job details for registration:" + String.Format(jobDetails, jobName, jenkinsHost, userId));

            var job = new JobType(jobName, jobUrl, jenkinsHost, userId);

            job = createJob(job);

            if (job == null)
            {
                throw new NLogRuntimeException("Unable to register job for zafira service: " + ServiceURL);
            }
            else
            {
                LOGGER.Info("Registered job details:" + String.Format(jobDetails, job.name, job.jenkinsHost, job.userId));
            }

            return job;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public JobType createJob(JobType job)
        {
            var response = new JobType();
            try
            {
                var postJob = Client().Execute(JOBS_PATH, job, Method.POST.ToString());
                var status = ((ResponseStatus)postJob).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<JobType>(((ResponseStatus)postJob).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to create job " + e);
            }
            return response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TestCaseType createTestCase(TestCaseType testCase)

        {
            TestCaseType response = new TestCaseType();
            try
            {
                var postTestCase = Client().Execute(TEST_CASES_PATH, testCase, Method.POST.ToString());
                var status = ((ResponseStatus)postTestCase).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<TestCaseType>(((ResponseStatus)postTestCase).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to create test case " + e);
            }
            return response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TestSuiteType createTestSuite(TestSuiteType testSuite)

        {
            TestSuiteType response = new TestSuiteType();
            try
            {
                var postTestSuite = Client().Execute(TEST_SUITES_PATH, testSuite, Method.POST.ToString());
                var status = ((ResponseStatus)postTestSuite).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<TestSuiteType>(((ResponseStatus)postTestSuite).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to create test suite " + e);
            }
            return response;
        }


        public UserType getUser(String userId)
        {
            var response = new UserType();
            try
            {
                var url = String.Format(USERS_PATH, userId);
                var getUser = Client().Execute(url);
                var status = ((ResponseStatus)getUser).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<UserType>(((ResponseStatus)getUser).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to get user " + e);
            }
            return response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TestRunType finishTestRun(long id)
        {
            var response = new TestRunType();
            try
            {
                var postRunFinished = Client().Execute(String.Format(TEST_RUNS_FINISH_PATH, id), "", Method.POST.ToString());
                var status = ((ResponseStatus)postRunFinished).StatusCode;
                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<TestRunType>(((ResponseStatus)postRunFinished).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to finish test run " + e);
            }
            return response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TestRunType startTestRun(TestRunType testRun)
        {
            var response = new TestRunType();
            try
            {
                var postTestRun = Client().Execute(TEST_RUNS_PATH, testRun, Method.POST.ToString());
                var status = ((ResponseStatus)postTestRun).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<TestRunType>(((ResponseStatus)postTestRun).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to start test run " + e);
            }
            return response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public string sendTestRunReport(long id, String recipients, Boolean showOnlyFailures, Boolean showStacktrace)
        {
            string response = null;
            try
            {
                var obj = new EmailType(recipients);
                var url = String.Format(TEST_RUN_EMAIL_PATH, id, showOnlyFailures ? "failures" : "all", showStacktrace ? "true" : "false");

                var postTestRun = Client().Execute(url, new EmailType(recipients), Method.POST.ToString(), "text/html");
                var status = ((ResponseStatus)postTestRun).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = ((ResponseStatus)postTestRun).ResponseBody;
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to send test run report " + e);
            }
            return response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TestType finishTest(TestType test)
        {
            var response = new TestType();
            try
            {
                var postFinishTest = Client().Execute(String.Format(TEST_FINISH_PATH, test.id), test, Method.POST.ToString());
                var status = ((ResponseStatus)postFinishTest).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<TestType>(((ResponseStatus)postFinishTest).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to finish test " + e);
            }
            return response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TestType startTest(TestType test)
        {
            var response = new TestType();
            try
            {
                var postTest = Client().Execute(TESTS_PATH, test, Method.POST.ToString());
                var status = ((ResponseStatus)postTest).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<TestType>(((ResponseStatus)postTest).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to start test " + e);
            }
            return response;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public TestRunType startTest(TestRunType testRun)
        {
            var response = new TestRunType();
            try
            {
                var postTestRunType = Client().Execute(TEST_RUNS_PATH, testRun, Method.POST.ToString());
                var status = ((ResponseStatus)postTestRunType).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<TestRunType>(((ResponseStatus)postTestRunType).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to start test run " + e);
            }
            return response;
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public TestType createTestWorkItems(long testId, List<String> workItems)
        {
            var response = new TestType();
            try
            {
                var postTestType = Client().Execute(String.Format(TEST_WORK_ITEMS_PATH, testId), workItems, Method.POST.ToString());
                var status = ((ResponseStatus)postTestType).StatusCode;

                if (status.Equals("200") || status.Equals("OK"))
                {
                    response = JsonConvert.DeserializeObject<TestType>(((ResponseStatus)postTestType).ResponseBody);
                }
            }
            catch (Exception e)
            {
                LOGGER.Error("Unable to create test work items " + e);
            }
            return response;
        }

        public TestType registerWorkItems(long testId, List<String> workItems)
        {
            TestType test = null;
            if (workItems != null && workItems.Count() > 0)
            {
                var response = createTestWorkItems(testId, workItems);
                test = response;
            }
            return test;
        }

        private string convertWebResponceToString(WebResponse response)
        {
            using (var stream = response.GetResponseStream())
            {
                var reader = new StreamReader(stream, Encoding.UTF8);
                var responseString = reader.ReadToEnd();
                return responseString;
            }
        }
    }
}