using System;
using NLog;
using NLog.Layouts;
using NLog.Targets;
using ZafiraIntegration.Client.Requests;
using ZafiraIntegration.Registrar;

namespace ZafiraIntegration.Logging
{
    [Target("Zebrunner")]
    public class ZebrunnerNLogTarget : TargetWithLayout
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly LogsBuffer _logsBuffer = LogsBuffer.Instance;

        public ZebrunnerNLogTarget()
        {
            Logger.Debug($"{DateTime.UtcNow} Creating instance of NLog Target.");
        }

        protected override void Write(LogEventInfo logEvent)
        {
            var currentTest = RunContext.GetCurrentTest();
            if (currentTest != null)
            {
                var log = new Log
                {
                    TestId = currentTest.Id,
                    Level = logEvent.Level.Name,
                    Message = RenderLogEvent(Layout, logEvent),
                    Timestamp = new DateTimeOffset(logEvent.TimeStamp).ToUnixTimeMilliseconds()
                };
                _logsBuffer.Add(log);
            }
        }
    }
}