using System;
using NLog;
using ZafiraIntegration.Client;

namespace ZafiraIntegration.Registrar
{
    public static class Screenshot
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ZebrunnerApiClient ApiClient = ZebrunnerApiClient.Instance;

        public static void Upload(byte[] screenshot)
        {
            Upload(screenshot, DateTimeOffset.Now);
        }

        public static void Upload(byte[] screenshot, DateTimeOffset capturedAt)
        {
            var testRun = RunContext.GetCurrentTestRun();
            var test = RunContext.GetCurrentTest();
            if (testRun != null && test != null)
            {
                ApiClient.UploadScreenshot(testRun.Id, test.Id, screenshot, capturedAt);
            }
            else
            {
                Logger.Debug("There is no registered test run or test. Test screenshot will not be attached.");
            }
        }
    }
}