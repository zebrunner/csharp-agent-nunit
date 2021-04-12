using System;

namespace ZafiraIntegration.Client.Responses
{
    public class SaveTestRunResponse
    {
        public long Id { get; set; }
        public string Uuid { get; set; }
        public string Name { get; set; }
        public string Status { get; set; }
        public DateTime StartedAt { get; set; }
        public string EndedAt { get; set; }
        public string Framework { get; set; }
        public ConfigDto Config { get; set; }

        public class ConfigDto
        {
            public string Environment { get; set; }
            public string Build { get; set; }
        }
    }
}