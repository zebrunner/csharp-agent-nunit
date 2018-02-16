using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

namespace ZafiraIntegration.http
{
    /// <summary>
    /// Uploads File via API endpoint
    /// </summary>
    public class FileUpload
    {

        public void UploadFile(string url, string filePath)
        {
            var httpClient = new HttpClient();
            var form = new MultipartFormDataContent();

            var fs = File.OpenRead(filePath);
            var streamContent = new StreamContent(fs);

            var fileContent = new ByteArrayContent(streamContent.ReadAsByteArrayAsync().Result);
            fileContent.Headers.ContentType = MediaTypeHeaderValue.Parse("multipart/form-data");

            form.Add(fileContent, "batch", Path.GetFileName(filePath));
            var response = httpClient.PostAsync(url, form).Result;
            if (response.StatusCode == HttpStatusCode.Created)
            {
                var responseContent = response.Content.ReadAsStringAsync().Result;
            }
        }
    }
}
