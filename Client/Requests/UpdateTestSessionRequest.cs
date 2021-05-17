using System;
using System.Collections.Generic;

namespace ZafiraIntegration.Client.Requests
{
    internal class UpdateTestSessionRequest
    {
        public DateTime EndedAt { get; set; }
        public HashSet<long> TestIds { get; set; }
    }
}
