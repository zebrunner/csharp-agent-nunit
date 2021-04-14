using System.IO;
using NLog;
using ZafiraIntegration.Client;
using ZafiraIntegration.Client.Requests;

namespace ZafiraIntegration.Registrar
{
    public static class Artifact
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        private static readonly ZebrunnerApiClient ApiClient = ZebrunnerApiClient.Instance;

        public static void AttachReferenceToTestRun(string name, string reference)
        {
            var testRun = RunContext.GetCurrentTestRun();
            if (testRun != null)
            {
                var artifactReference = new ArtifactReference(name, reference);
                ApiClient.AttachArtifactReferenceToTestRun(testRun.Id, artifactReference);
            }
            else
            {
                Logger.Debug("There is no registered test run. Test run artifact refarence will not be attached.");
            }
        }

        public static void AttachToTestRun(string name, Stream stream)
        {
            var testRun = RunContext.GetCurrentTestRun();
            if (testRun != null)
            {
                ApiClient.UploadTestRunArtifact(testRun.Id, name, stream);
            }
            else
            {
                Logger.Debug("There is no registered test run. Test run artifact will not be uploaded.");
            }
        }

        public static void AttachToTestRun(string name, string filePath)
        {
            using (Stream stream = new BufferedStream(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
            {
                AttachToTestRun(name, stream);
            }
        }

        public static void AttachToTestRun(string name, byte[] fileContent)
        {
            using (Stream stream = new MemoryStream(fileContent, false))
            {
                AttachToTestRun(name, stream);
            }
        }

        public static void AttachReferenceToTest(string name, string reference)
        {
            var testRun = RunContext.GetCurrentTestRun();
            var test = RunContext.GetCurrentTest();
            if (testRun != null && test != null)
            {
                var artifactReference = new ArtifactReference(name, reference);
                ApiClient.AttachArtifactReferenceToTest(testRun.Id, test.Id, artifactReference);
            }
            else
            {
                Logger.Debug("There is no registered test run or test. Test artifact reference will not be attached.");
            }
        }

        public static void AttachToTest(string name, Stream stream)
        {
            var testRun = RunContext.GetCurrentTestRun();
            var test = RunContext.GetCurrentTest();
            if (testRun != null && test != null)
            {
                ApiClient.UploadTestArtifact(testRun.Id, test.Id, name, stream);
            }
            else
            {
                Logger.Debug("There is no registered test run or test. Test artifact will not be uploaded.");
            }
        }

        public static void AttachToTest(string name, string filePath)
        {
            using (Stream stream = new BufferedStream(new FileStream(filePath, FileMode.Open, FileAccess.Read)))
            {
                AttachToTest(name, stream);
            }
        }

        public static void AttachToTest(string name, byte[] fileContent)
        {
            using (Stream stream = new MemoryStream(fileContent, false))
            {
                AttachToTest(name, stream);
            }
        }
    }
}