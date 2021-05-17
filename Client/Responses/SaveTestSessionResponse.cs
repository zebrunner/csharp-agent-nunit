using System;
using System.Collections.Generic;

namespace ZafiraIntegration.Client.Responses
{
    public class SaveTestSessionResponse
    {
        public long Id { get; set; }
        public string SessionId { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime EndedAt { get; set; }
        public object DesiredCapabilities { get; set; }
        public object Capabilities { get; set; }
        public ISet<long> TestIds { get; } = new HashSet<long>();
    }
}