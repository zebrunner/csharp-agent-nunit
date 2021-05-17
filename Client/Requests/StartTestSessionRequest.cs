using System;
using System.Collections.Generic;

namespace ZafiraIntegration.Client.Requests
{
    internal class StartTestSessionRequest
    {
        public string SessionId { get; set; }
        public DateTime StartedAt { get; set; }
        public object DesiredCapabilities { get; set; }
        public object Capabilities { get; set; }
        public ISet<long> TestIds { get; set; }
    }
}