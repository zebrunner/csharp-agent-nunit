using System;
using System.IO;
using System.Net;
using System.Text;
using Newtonsoft.Json;
using ZafiraIntegration.http;
using NLog;

namespace ZafiraIntegration
{


    /// <summary>
    /// Create Http Request, using json, and read Http Response.
    /// </summary>
    public class ApiRequest
    {
        private static Logger LOGGER = LogManager.GetCurrentClassLogger();
        /// <summary>
        /// Url of http server wich request will be created to.
        /// </summary>
        public String URL { get; set; }

        /// <summary>
        /// Optional Auth Token
        /// </summary>
        public String AuthToken { get; set; }

        /// <summary>
        /// Optional Request Project
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

        public HttpWebRequest HttpRequest { get; internal set; }
        public HttpWebResponse HttpResponse { get; internal set; }
        public CookieContainer CookieContainer = new CookieContainer();

        /// <summary>
        /// Constructor Overload that allows passing URL and the VERB to be used.
        /// </summary>
        /// <param name="url">URL which request will be created</param>
        /// <param name="verb">Http Verb that will be userd in this request</param>
        public ApiRequest(string url, string verb = "GET", string authToken = null)
        {
            URL = url;
            Verb = verb;
            AuthToken = authToken;
        }

        /// <summary>
        /// Default constructor overload without any paramter
        /// </summary>
        public ApiRequest()
        {
            Verb = "GET";
        }            


        public object Execute<TT>(string url, object obj, string verb)
        {
            if (url != null)
                URL = url;
            
            if (verb != null)
                Verb = verb;
           
            HttpRequest = CreateRequest();
            LOGGER.Info("Request (1): " + HttpRequest.ToString());

            WriteStream(obj);

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();

            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                LOGGER.Error("Error (1): " + error.Message, error);
                return ReadResponseFromError(error);
            }
            catch (Exception e)
            {
                LOGGER.Error("Error (1.2): " + e.Message, e);
            }
            return JsonConvert.DeserializeObject<TT>(ReadResponse().ResponseBody);
        }

        public object Execute<TT>(string url)
        {
            if (url != null)
                URL = URL + url;
            
            HttpRequest = CreateRequest();
            LOGGER.Info("Request (2): " + HttpRequest.ToString());

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                LOGGER.Error("Error (2): " + error.Message, error);
                return ReadResponseFromError(error);
            }
            catch (Exception e)
            {
                LOGGER.Error("Error (2.2): " + e.Message, e);
            }
            return JsonConvert.DeserializeObject<TT>(ReadResponse().ResponseBody);
        }

        public object Execute<TT>()
        {
            if (URL == null)
                throw new ArgumentException("URL cannot be null.");

            HttpRequest = CreateRequest();
            LOGGER.Info("Request (3): " + HttpRequest.ToString());

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                LOGGER.Error("Error (3): " + error.Message, error);
                return ReadResponseFromError(error);
            }
            catch (Exception e)
            {
                LOGGER.Error("Error (3.2): " + e.Message, e);
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
            LOGGER.Info("Request (4): " + HttpRequest.ToString());
            LOGGER.Info("Request URL: " + URL);
            LOGGER.Info("Request Headers: " + HttpRequest.Headers.ToString());
            LOGGER.Info("Request Method: " + HttpRequest.Method);

            WriteStream(obj);

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();

            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                LOGGER.Error("Error (4): " + error.Message, error);
                return ReadResponseFromError(error);
            }
            catch (Exception e)
            {
                LOGGER.Error("Error (4.2): " + e.Message, e);
            }
            return ReadResponse();
        }

        /// <summary>
        /// Executes http request passing url and object 
        /// </summary>
        /// <param name="url"></param>
        /// <param name="obj"></param>
        /// <param name="verb"></param>
        /// <param name="acceptHeader"></param>
        /// <returns></returns>
        public object Execute(string url, object obj, string verb, string acceptHeader)
        {
            if (url != null)
                URL = URL + url;

            if (verb != null)
                Verb = verb;

            HttpRequest = CreateRequest(acceptHeader);
            LOGGER.Info("Request (5): " + HttpRequest.ToString());

            WriteStream(obj);

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                LOGGER.Error("Error (5): " + error.Message, error);
                return ReadResponseFromError(error);
            }
            catch (Exception e)
            {
                LOGGER.Error("Error (5.2): " + e.Message, e);
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
            LOGGER.Info("Request (6): " + HttpRequest.ToString());
            WriteStream(obj);
            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                LOGGER.Error("Error (6): " + error.Message, error);
                return ReadResponseFromError(error);
            }
            catch (Exception e)
            {
                LOGGER.Error("Error (6.2): " + e.Message, e);
            }
            return ReadResponse();
        }
        /// <summary>
        /// Executes http request passing url
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        public object Execute(string url)
        {
            if (url != null)
                URL = URL + url;
            
            HttpRequest = CreateRequest(); ;
            LOGGER.Info("Request (7): " + HttpRequest.ToString());
            LOGGER.Info("Request (7) Uri is: " + HttpRequest.RequestUri.OriginalString);
            LOGGER.Info("Request (7) Headers are: " + HttpRequest.Headers);
            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
                LOGGER.Info("Response Code is: " + HttpResponse.StatusCode);
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                LOGGER.Error("Error (7): " + error.Message, error);
                return ReadResponseFromError(error);
            }
            catch (Exception e)
            {
                LOGGER.Error("Error (7.2): " + e.Message, e);
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
            LOGGER.Info("Request (8): " + HttpRequest.ToString());

            try
            {
                HttpResponse = (HttpWebResponse)HttpRequest.GetResponse();
            }
            catch (WebException error)
            {
                HttpResponse = (HttpWebResponse)error.Response;
                LOGGER.Error("Error (8): " + error.Message, error);
                return ReadResponseFromError(error);
            }
            catch (Exception e)
            {
                LOGGER.Error("Error (8.2): " + e.Message, e);
            }
            return ReadResponse();
        }
        /// <summary>
        /// Creates HttpWebRequest
        /// </summary>
        /// <returns></returns>
        internal HttpWebRequest CreateRequest()
        {
            var basicRequest = (HttpWebRequest)WebRequest.Create(URL);
            basicRequest.ContentType = Content;
            basicRequest.Method = Verb;
            //basicRequest.CookieContainer = CookieContainer;
            //basicRequest.Headers.Add("Authorization", "Bearer " + AuthToken);
            basicRequest.Accept = "application/json";
            
            if (AuthToken != null)
            {
                basicRequest.Headers.Add("Authorization", AuthToken);
            }

            if (Project != null)
            {
                basicRequest.Headers.Add("Project", Project);
            }
           
            return basicRequest;
        }

        /// <summary>
        /// Creates HttpWebRequest
        /// </summary>
        /// <param name="sAcceptHeader"></param>
        /// <returns></returns>
        internal HttpWebRequest CreateRequest(string acceptHeader)
        {
            var basicRequest = (HttpWebRequest)WebRequest.Create(URL);
            basicRequest.ContentType = Content;
            basicRequest.Method = Verb;
            //basicRequest.CookieContainer = CookieContainer;
            //basicRequest.Headers.Add("Authorization", "Bearer " + AuthToken);
            basicRequest.Accept = acceptHeader;

            if (AuthToken != null)
            {
                basicRequest.Headers.Add("Authorization", AuthToken);
            }

            if (Project != null)
            {
                basicRequest.Headers.Add("Project", Project);
            }

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
                    {
                        streamWriter.Write(obj);
                        LOGGER.Info("WriteStream (String): " + obj);
                    }
                    else
                    {
                        streamWriter.Write(JsonConvert.SerializeObject(obj));
                        LOGGER.Info("WriteStream (JSON): " + JsonConvert.SerializeObject(obj));
                    }
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
                        status.Header = header.ToString();
                    }
                    status.StatusCode = ((int)HttpResponse.StatusCode).ToString();
                    status.StatusMessage = HttpResponse.StatusDescription.ToString();
                    status.ResponseBody = streamReader.ReadToEnd();
                    LOGGER.Info("Status Code: " + status.StatusCode);
                    LOGGER.Info("Status ResponseBody: " + status.ResponseBody);
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
                status.Header = header.ToString();
            }

            status.StatusCode = ((int)HttpResponse.StatusCode).ToString();
            status.StatusMessage = HttpResponse.StatusDescription.ToString();
            LOGGER.Info("Error Status Code: " + status.StatusCode);
            LOGGER.Info("Error Status Message: " + status.StatusMessage);
           
            using (var streamReader = new StreamReader(error.Response.GetResponseStream()))
                status.ResponseBody = streamReader.ReadToEnd();

            LOGGER.Info("Error Status Body: " + status.ResponseBody);

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

