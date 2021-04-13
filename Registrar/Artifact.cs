using System.IO;
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
            var artifactReference = new ArtifactReference(name, reference);
            ApiClient.AttachArtifactReferenceToTestRun(testRunId, artifactReference);
        }

        public static void AttachToTestRun(string name, Stream stream)
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            ApiClient.UploadTestRunArtifact(testRunId, name, stream);
        }

        public static void AttachToTestRun(string name, string filePath)
        {
            Stream stream = new BufferedStream(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            AttachToTestRun(name, stream);
        }

        public static void AttachToTestRun(string name, byte[] fileContent)
        {
            AttachToTestRun(name, new MemoryStream(fileContent, false));
        }

        public static void AttachReferenceToTest(string name, string reference)
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var testId = RunContext.GetCurrentTest().Id;
            var artifactReference = new ArtifactReference(name, reference);
            ApiClient.AttachArtifactReferenceToTest(testRunId, testId, artifactReference);
        }

        public static void AttachToTest(string name, Stream stream)
        {
            var testRunId = RunContext.SaveTestRunResponse.Id;
            var testId = RunContext.GetCurrentTest().Id;
            ApiClient.UploadTestArtifact(testRunId, testId, name, stream);
        }

        public static void AttachToTest(string name, string filePath)
        {
            Stream stream = new BufferedStream(new FileStream(filePath, FileMode.Open, FileAccess.Read));
            AttachToTest(name, stream);
        }

        public static void AttachToTest(string name, byte[] fileContent)
        {
            AttachToTest(name, new MemoryStream(fileContent, false));
        }
    }
}