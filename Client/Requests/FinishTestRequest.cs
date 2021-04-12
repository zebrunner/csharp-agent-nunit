using System;

namespace ZafiraIntegration.Client.Requests
{
    public class FinishTestRequest
    {
        public DateTime EndedAt { get; set; }
        public string Result { get; set; }
        public string Reason { get; set; }
    }
}