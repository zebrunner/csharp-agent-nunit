using System;

namespace ZebrunnerAgent.Registrar
{
    public class TestSessionStart
    {
        public DateTime StartedAt { get; set; }
        public object DesiredCapabilities { get; }
        public object Capabilities { get; }

        public TestSessionStart(DateTime startedAt, object desiredCapabilities, object capabilities)
        {
            StartedAt = startedAt;
            DesiredCapabilities = desiredCapabilities;
            Capabilities = capabilities;
        }
    }
}
