using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using NLog;
using RestSharp;
using RestSharp.Authenticators;
using RestSharp.Serializers.NewtonsoftJson;
using ZafiraIntegration.Client.Requests;
using ZafiraIntegration.Client.Responses;
using ZafiraIntegration.Config;

namespace ZafiraIntegration.Client
{
    internal class ZebrunnerApiClient
    {
        internal static ZebrunnerApiClient Instance { get; } = new ZebrunnerApiClient();
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly RestClient _restClient;
        private readonly string _authenticationToken;

        private ZebrunnerApiClient()
        {
            _restClient = new RestClient(Configuration.GetServerHost());
            _restClient.UseNewtonsoftJson();

            var requestBody = new RefreshAccessTokenRequest {RefreshToken = Configuration.GetAccessToken()};
            var request = new RestRequest(Iam("/v1/auth/refresh"), DataFormat.Json);
            request.AddJsonBody(requestBody);

            var response = _restClient.Post<RefreshAccessTokenResponse>(request);
            _restClient.Authenticator = new JwtAuthenticator(response.Data.AuthToken);
            _authenticationToken = response.Data.AuthToken;
        }

        private static string Iam(string resourceUri)
        {
            return "/api/iam" + resourceUri;
        }

        private static string Reporting(string resourceUri)
        {
            return "/api/reporting" + resourceUri;
        }

        public SaveTestRunResponse RegisterTestRunStart(StartTestRunRequest requestBody)
        {
            var request = new RestRequest(Reporting("/v1/test-runs"), DataFormat.Json);
            request.AddJsonBody(requestBody);
            request.AddQueryParameter("projectKey", Configuration.GetProjectKey());

            var response = _restClient.Post<SaveTestRunResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception($"Could not register start of test run. Response body is {response.Content}");
        }

        public SaveTestRunResponse RegisterTestRunFinish(long testRunId, FinishTestRunRequest requestBody)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}"), DataFormat.Json);
            request.AddJsonBody(requestBody);

            var response = _restClient.Put<SaveTestRunResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception($"Could not register finish of test run. Response body is {response.Content}");
        }

        public SaveTestResponse RegisterTestStart(long testRunId, StartTestRequest requestBody)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/tests"), DataFormat.Json);
            request.AddJsonBody(requestBody);

            var response = _restClient.Post<SaveTestResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception($"Could not register start of test. Response body is {response.Content}");
        }

        public SaveTestResponse RegisterTestFinish(long testRunId, long testId, FinishTestRequest requestBody)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/tests/{testId}"), DataFormat.Json);
            request.AddJsonBody(requestBody);

            var response = _restClient.Put<SaveTestResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception($"Could not register finish of test. Response body is {response.Content}");
        }

        public void SendLogs(long testRunId, List<Log> requestBody)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/logs"), DataFormat.Json);
            request.AddJsonBody(requestBody);

            var response = _restClient.Post<SaveTestResponse>(request);
            if (response.StatusCode != HttpStatusCode.Accepted)
            {
                Logger.Error($"Could not send test logs. Response body: {response.Content}");
            }
        }

        public void UploadScreenshot(long testRunId, long testId, byte[] bytes, DateTimeOffset capturedAt)
        {
            var httpClient = new HttpClient {BaseAddress = new Uri(Configuration.GetServerHost())};
            httpClient.DefaultRequestHeaders.Authorization =
                new AuthenticationHeaderValue("Bearer", _authenticationToken);

            var httpContent = new ByteArrayContent(bytes);
            httpContent.Headers.ContentType = new MediaTypeHeaderValue("image/png");
            httpContent.Headers.Add("x-zbr-screenshot-captured-at", capturedAt.ToUnixTimeMilliseconds().ToString());

            var requestUri = Reporting($"/v1/test-runs/{testRunId}/tests/{testId}/screenshots");
            var responseMessage = httpClient.PostAsync(requestUri, httpContent).Result;

            if (responseMessage.StatusCode != HttpStatusCode.Created)
            {
                var responseBody = responseMessage.Content.ReadAsStringAsync().Result;
                Logger.Error($"Could not upload screenshot. Response body: {responseBody}");
            }
        }

        public void UploadTestRunArtifact(long testRunId, string name, Stream stream)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/artifacts"));
            request.AddFile("file", stream.CopyTo, name, stream.Length);
            request.AlwaysMultipartFormData = true;

            var response = _restClient.Post<string>(request);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                Logger.Error($"Could not upload test run artifact. Response body: {response.Content}");
            }
        }

        public void UploadTestArtifact(long testRunId, long testId, string name, Stream stream)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/tests/{testId}/artifacts"));
            request.AddFile("file", stream.CopyTo, name, stream.Length);
            request.AlwaysMultipartFormData = true;

            var response = _restClient.Post<string>(request);
            if (response.StatusCode != HttpStatusCode.Created)
            {
                Logger.Error($"Could not upload test artifact. Response body: {response.Content}");
            }
        }

        public void AttachArtifactReferenceToTestRun(long testRunId, ArtifactReference artifactReference)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/artifact-references"));
            request.AddJsonBody(new JsonObject {["items"] = new JsonArray {artifactReference}});

            var response = _restClient.Put<string>(request);
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Logger.Error($"Could not attach test run artifact reference. Response body: {response.Content}");
            }
        }

        public void AttachArtifactReferenceToTest(long testRunId, long testId, ArtifactReference artifactReference)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/tests/{testId}/artifact-references"));
            request.AddJsonBody(new JsonObject {["items"] = new JsonArray {artifactReference}});

            var response = _restClient.Put<string>(request);
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Logger.Error($"Could not attach test artifact reference. Response body: {response.Content}");
            }
        }

        public void AttachLabelsToTestRun(long testRunId, IList<Label> labels)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/labels"));
            request.AddJsonBody(new JsonObject {["items"] = labels});

            var response = _restClient.Put<string>(request);
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Logger.Error($"Could not attach test run labels. Response body: {response.Content}");
            }
        }

        public void AttachLabelsToTest(long testRunId, long testId, IList<Label> labels)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/tests/{testId}/labels"));
            request.AddJsonBody(new JsonObject {["items"] = labels});

            var response = _restClient.Put<string>(request);
            if (response.StatusCode != HttpStatusCode.NoContent)
            {
                Logger.Error($"Could not attach test labels. Response body: {response.Content}");
            }
        }

        public SaveTestSessionResponse RegisterTestSessionStart(long testRunId, StartTestSessionRequest requestBody)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/test-sessions"), DataFormat.Json);
            request.AddJsonBody(requestBody);

            var response = _restClient.Post<SaveTestSessionResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception($"Could not register start of test session. Response body is {response.Content}");
        }

        public SaveTestSessionResponse RegisterTestSessionUpdate(long testRunId, long testSessionId, UpdateTestSessionRequest requestBody)
        {
            var request = new RestRequest(Reporting($"/v1/test-runs/{testRunId}/test-sessions/{testSessionId}"), DataFormat.Json);
            request.AddJsonBody(requestBody);

            var response = _restClient.Put<SaveTestSessionResponse>(request);
            if (response.StatusCode == HttpStatusCode.OK)
            {
                return response.Data;
            }

            throw new Exception($"Could not register update of test session. Response body is {response.Content}");
        }
    }
}
