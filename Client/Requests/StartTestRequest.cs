using System;

namespace ZafiraIntegration.Client.Requests
{
    public class StartTestRequest
    {
        public string Name { get; set; }
        public string CorrelationData { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime StartedAt { get; set; }
        public string Maintainer { get; set; }
        public string TestCase { get; set; }
    }
}