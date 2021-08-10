using System;
using System.Collections.Generic;

namespace ZafiraIntegration.Client.Requests
{
    internal class StartTestRequest
    {
        public string Name { get; set; }
        public string CorrelationData { get; set; }
        public string ClassName { get; set; }
        public string MethodName { get; set; }
        public DateTime StartedAt { get; set; }
        public string Maintainer { get; set; }
        public string TestCase { get; set; }
        public List<Label> Labels { get; set; }
    }
}