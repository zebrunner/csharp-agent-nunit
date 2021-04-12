using ZafiraIntegration.Client;
using ZafiraIntegration.Client.Requests;

namespace ZafiraIntegration.Registrar
{
    public static class Artifact
    {
        private static readonly ZebrunnerApiClient ApiClient = ZebrunnerApiClient.Instance;

        public static void AttachReferenceToTestRun(string name, string reference)
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var artifactReference = new ArtifactReference
            {
                Name = name,
                Value = reference
            };
            ApiClient.AttachArtifactReferenceToTestRun(testRunId, artifactReference);
        }

        public static void AttachReferenceToTest(string name, string reference)
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var testId = RunContext.GetCurrentTest().Id;
            var artifactReference = new ArtifactReference
            {
                Name = name,
                Value = reference
            };
            ApiClient.AttachArtifactReferenceToTest(testRunId, testId, artifactReference);
        }
    }
}