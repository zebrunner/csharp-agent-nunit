namespace ZafiraIntegration.http
{
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
}
