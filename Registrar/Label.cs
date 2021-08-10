using System.Linq;
using NLog;
using ZebrunnerAgent.Client;

namespace ZebrunnerAgent.Registrar
{
    public static class Label
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ZebrunnerApiClient ApiClient = ZebrunnerApiClient.Instance;

        public static void AttachToTestRun(string key, params string[] values)
        {
            var testRun = RunContext.GetCurrentTestRun();
            if (testRun != null)
            {
                var labels = values.Select(value => new Client.Requests.Label(key, value)).ToList();
                ApiClient.AttachLabelsToTestRun(testRun.Id, labels);
            }
            else
            {
                Logger.Debug("There is no registered test run. Test run label will not be attached.");
            }
        }

        public static void AttachToTest(string key, params string[] values)
        {
            var testRun = RunContext.GetCurrentTestRun();
            var test = RunContext.GetCurrentTest();
            if (testRun != null && test != null)
            {
                var labels = values.Select(value => new Client.Requests.Label(key, value)).ToList();
                ApiClient.AttachLabelsToTest(testRun.Id, test.Id, labels);
            }
            else
            {
                Logger.Debug("There is no registered test run or test. Test  label will not be attached.");
            }
        }
    }
}