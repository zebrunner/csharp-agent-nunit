using System;
using ZafiraIntegration.Client;

namespace ZafiraIntegration.Registrar
{
    public static class Screenshot
    {
        private static readonly ZebrunnerApiClient ApiClient = ZebrunnerApiClient.Instance;

        public static void Upload(byte[] screenshot)
        {
            Upload(screenshot, DateTimeOffset.Now);
        }

        public static void Upload(byte[] screenshot, DateTimeOffset capturedAt)
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var testId = RunContext.GetCurrentTest().Id;
            ApiClient.UploadScreenshot(testRunId, testId, screenshot, capturedAt);
        }
    }
}