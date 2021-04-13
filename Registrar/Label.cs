using System.Linq;
using ZafiraIntegration.Client;

namespace ZafiraIntegration.Registrar
{
    public static class Label
    {
        private static readonly ZebrunnerApiClient ApiClient = ZebrunnerApiClient.Instance;

        public static void AttachToTestRun(string key, params string[] values)
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var labels = values.Select(value => new Client.Requests.Label(key, value)).ToList();

            ApiClient.AttachLabelsToTestRun(testRunId, labels);
        }

        public static void AttachToTest(string key, params string[] values)
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var testId = RunContext.GetCurrentTest().Id;
            var labels = values.Select(value => new Client.Requests.Label(key, value)).ToList();

            ApiClient.AttachLabelsToTest(testRunId, testId, labels);
        }
    }
}