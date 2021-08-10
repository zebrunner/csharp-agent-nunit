using System;

namespace ZebrunnerAgent.Client.Requests
{
    internal class StartTestRunRequest
    {
        public string Uuid { get; set; }
        public string Name { get; set; }
        public DateTime StartedAt { get; set; }
        public string Framework { get; set; }
        public ConfigDto Config { get; set; }
        public JenkinsContextDto JenkinsContext { get; set; }

        internal class ConfigDto
        {
            public string Environment { get; set; }
            public string Build { get; set; }
        }

        internal class JenkinsContextDto
        {
            public string JobUrl { get; set; }
            public string JobNumber { get; set; }
            public string ParentJobUrl { get; set; }
            public string ParentJobNumber { get; set; }
        }
    }
}