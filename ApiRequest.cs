using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Http.Headers;


namespace ZafiraIntegration
{
    public class User
    {
        public string username { get; set; }
        public string password { get; set; }
    }

    public class Httpresponse
    {
        public int Statuscode { get; set; }
        public string Location { get; set; }
    }
    /// <summary>
    /// Sends credentials for Basic HTTP Authentication
    /// </summary>
    public class Credentials
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }

    public class ResponseStatus
    {
        private string _statusCode;
        private string _statusMessage;
        private string _responseBody;
        private string _header;
        public ResponseStatus()
        {
            _statusCode = "";
            _statusMessage = "";
            _responseBody = "";
            _header = "";
        }
        public string Header
        {
            get
            {
                return _header;
            }
            set
            {
                _header = value;
            }
        }
        public string StatusCode
        {
            get
            {
                return _statusCode;
            }
            set
            {
                _statusCode = value;
            }
        }
        public string StatusMessage
        {
            get
            {
                return _statusMessage;
            }
            set
            {
                _statusMessage = value;
            }
        }
        public string ResponseBody
        {
            get
            {
                return _responseBody;
            }
            set
            {
                _responseBody = value;
            }
        }
    }
    /// <summary>
    /// Create Http Request, using json, and read Http Response.
    /// </summary>
    public class ApiRequest
    {
         /// <summary>
        /// Url of http server wich request will be created to.
        /// </summary>
        public String URL { get; set; }

        /// <summary>
        /// Optional Request Access Token
        /// </summary>
        public String AccessToken { get; set; }


        /// <summary>
        /// Optional Request Access Token
        /// </summary>
        public String Project { get; set; }



        /// <summary>
        /// HTTP Verb (Method Eg. GET, POST, PUT, DELETE)
        /// </summary>
        public String Verb { get; set; }

        /// <summary>
        /// Request content
        /// </summary>
        public String Content
        {
            get { return "application/json"; }
        }

        public Credentials Credentials { get; set; }
        public HttpWebRequest HttpRequest { get; internal set; }
        public HttpWebResponse HttpResponse { get; internal set; }
        public CookieContainer CookieContainer = new CookieContainer();

        /// <summary>
        /// Constructor Overload that allows passing URL and the VERB to be used.
        /// </summary>
        /// <param name="url">URL which request will be created</param>
        /// <param name="verb">Http Verb that will be userd in this request</param>
        public ApiRequest(string url, string verb, string accessToken = "")
        {
            URL = url;
            Verb = verb;
            AccessToken = accessToken;
        }

        /// <summary>
        /// Default constructor overload without any paramter
        /// </summary>
        public ApiRequest()
        {
            Verb = "GET";
        }

        /// <summary>
        /// Uploads File via API endpoint
        /// </summary>
        public class FileUpload
        {
         
            public async void UploadFile(string url, string filePath)
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


        public object Execute<TT>(string url, object obj, string verb)
        {
            

            if (url != null)
                URL = url;
            

            if (verb != null)
                Verb = verb;
           
            HttpRequest = CreateRequest();

            WriteStream(obj);

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();

            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                return ReadResponseFromError(error);
            }
            return JsonConvert.DeserializeObject<TT>(ReadResponse().ResponseBody);
        }

        public object Execute<TT>(string url)
        {
            if (url != null)
                URL = url;
            

            HttpRequest = CreateRequest();

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                return ReadResponseFromError(error);
            }
            return JsonConvert.DeserializeObject<TT>(ReadResponse().ResponseBody);
        }

        public object Execute<TT>()
        {
            if (URL == null)
                throw new ArgumentException("URL cannot be null.");
            

            HttpRequest = CreateRequest();

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                return ReadResponseFromError(error);
            }
            return JsonConvert.DeserializeObject<TT>(ReadResponse().ResponseBody);
        }

        /// <summary>
        /// Executes http request passing url and object 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        public object Execute(string url, object obj, string verb)
        {
            

            if (url != null)
                URL = URL + url;
            

            if (verb != null)
                Verb = verb;
            

            HttpRequest = CreateRequest();

            WriteStream(obj);

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                return ReadResponseFromError(error);
            }
            return ReadResponse();
        }

        /// <summary>
        /// Executes http request passing url and object 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        public object Execute(string url, object obj, string verb, string acceptHeader)
        {


            if (url != null)
                URL = URL + url;


            if (verb != null)
                Verb = verb;


            HttpRequest = CreateRequest(acceptHeader);

            WriteStream(obj);

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                return ReadResponseFromError(error);
            }
            return ReadResponse();
        }
        /// <summary>
        /// Executes http request passing url with dataKey and object 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="urlDataKey"></param>
        /// <param name="obj"></param>
        /// <param name="verb"></param>
        /// <returns></returns>
        public object Execute(string url, string urlDataKey, object obj, string verb)
        {
            

            if (url != null)
                URL = URL + urlDataKey;
            

            if (verb != null)
                Verb = verb;
            

            HttpRequest = CreateRequest();
            WriteStream(obj);
            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                return ReadResponseFromError(error);
            }
            return ReadResponse();
        }
        /// <summary>
        /// Executes http request passing url
        /// </summary>
        /// <param name="url"></param>
        /// <param name="sAccessToken"></param>
        /// <returns></returns>
        public object Execute(string url)
        {
            if (url != null)
                URL = URL + url;
            

            HttpRequest = CreateRequest(); ;
            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                return ReadResponseFromError(error);
            }
            return ReadResponse();
        }
        /// <summary>
        /// Executes http request
        /// </summary>
        /// <returns></returns>
        public object Execute()
        {
            if (URL == null)
                throw new ArgumentException("URL cannot be null.");
            

            HttpRequest = CreateRequest();

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                return ReadResponseFromError(error);
            }
            return ReadResponse();
        }
        /// <summary>
        /// Creates HttpWebRequest
        /// </summary>
        /// <param name="sAccessToken"></param>
        /// <returns></returns>
        internal HttpWebRequest CreateRequest()
        {
            var basicRequest = (HttpWebRequest)WebRequest.Create(URL);
            basicRequest.ContentType = Content;
            basicRequest.Method = Verb;
            //basicRequest.CookieContainer = CookieContainer;
            //basicRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
            basicRequest.Accept = "application/json";
            
            if (AccessToken != null)
            {
                basicRequest.Headers.Add("Authorization", AccessToken);
            }

            if (Project != null)
            {
                basicRequest.Headers.Add("Project", Project);
            }
           

            if (Credentials != null)
                basicRequest.Headers.Add("Authorization", "Basic" + " " + EncodeCredentials(Credentials));

            return basicRequest;
        }

        /// <summary>
        /// Creates HttpWebRequest
        /// </summary>
        /// <param name="sAccessToken"></param>
        /// <returns></returns>
        internal HttpWebRequest CreateRequest(string acceptHeader)
        {
            var basicRequest = (HttpWebRequest)WebRequest.Create(URL);
            basicRequest.ContentType = Content;
            basicRequest.Method = Verb;
            //basicRequest.CookieContainer = CookieContainer;
            //basicRequest.Headers.Add("Authorization", "Bearer " + AccessToken);
            basicRequest.Accept = acceptHeader;

            if (AccessToken != null)
            {
                basicRequest.Headers.Add("Authorization", AccessToken);
            }

            if (Project != null)
            {
                basicRequest.Headers.Add("Project", Project);
            }


            if (Credentials != null)
                basicRequest.Headers.Add("Authorization", "Basic" + " " + EncodeCredentials(Credentials));

            return basicRequest;
        }

        /// <summary>
        /// Writes Stream
        /// </summary>
        /// <param name="obj"></param>
        internal void WriteStream(object obj)
        {
            if (obj != null)
            {
                using (var streamWriter = new StreamWriter(HttpRequest.GetRequestStream()))
                {
                    if (obj is string)
                        streamWriter.Write(obj);
                    else
                        streamWriter.Write(JsonConvert.SerializeObject(obj));
                }
            }
        }
        /// <summary>
        /// Gets reposnse data
        /// </summary>
        /// <returns></returns>
        internal ResponseStatus ReadResponse()
        {
           
            ResponseStatus status = new ResponseStatus();
            if (HttpResponse != null)
                using (var streamReader = new StreamReader(HttpResponse.GetResponseStream()))
                {
                    WebHeaderCollection header = HttpResponse.Headers;
                    if (!header.Equals(null))
                    {
                        try
                        {
                            
                            status.Header = header.ToString();
                        }
                        catch (Exception e)
                        {
                            
                        }
                    }
                    status.StatusCode = ((int)HttpResponse.StatusCode).ToString();
                    status.StatusMessage = HttpResponse.StatusDescription.ToString();
                    status.ResponseBody = streamReader.ReadToEnd();
                    
                }
            return status;
        }
        /// <summary>
        /// Gets Response Errors
        /// </summary>
        /// <param name="error"></param>
        /// <returns></returns>
        internal ResponseStatus ReadResponseFromError(WebException error)
        {
           
            WebHeaderCollection header = HttpResponse.Headers;
            ResponseStatus status = new ResponseStatus();
            if (!header.Equals(null))
            {
                try
                {
                   
                    status.Header = header.ToString();
                }
                catch (Exception e)
                {
                }
            }

            status.StatusCode = ((int)HttpResponse.StatusCode).ToString();
            status.StatusMessage = HttpResponse.StatusDescription.ToString();
           

            using (var streamReader = new StreamReader(error.Response.GetResponseStream()))
                status.ResponseBody = streamReader.ReadToEnd();

            return status;
        }
        /// <summary>
        /// Adds Optional Credentials for Basic Authentication
        /// </summary>
        /// <param name="credentials"></param>
        /// <returns></returns>
        internal static string EncodeCredentials(Credentials credentials)
        {
            var strCredentials = string.Format("{0}:{1}", credentials.UserName, credentials.Password);
            var encodedCredentials = Convert.ToBase64String(Encoding.UTF8.GetBytes(strCredentials));

            return encodedCredentials;
        }

        /// <summary>
        /// Supported Methods
        /// </summary>
        public enum Method
        {
            GET,
            DELETE,
            POST,
            PUT
        }


    }
}

