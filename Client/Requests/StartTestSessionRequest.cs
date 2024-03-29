﻿using System;
using System.Collections.Generic;

namespace ZebrunnerAgent.Client.Requests
{
    internal class StartTestSessionRequest
    {
        public string SessionId { get; set; }
        public DateTime InitiatedAt { get; set; }
        public DateTime StartedAt { get; set; }
        public string Status { get; set; }
        public object DesiredCapabilities { get; set; }
        public object Capabilities { get; set; }
        public ISet<long> TestIds { get; set; }
    }
}