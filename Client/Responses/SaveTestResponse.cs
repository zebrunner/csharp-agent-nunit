using System;

namespace ZafiraIntegration.Client.Responses
{
    internal class SaveTestResponse
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string CorrelationData { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public string Maintainer { get; set; }
        public string TestCase { get; set; }
        public string Result { get; set; }
        public string Reason { get; set; }
    }
}