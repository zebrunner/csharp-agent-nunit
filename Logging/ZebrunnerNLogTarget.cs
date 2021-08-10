using System;
using NLog;
using NLog.Targets;
using ZebrunnerAgent.Client.Requests;
using ZebrunnerAgent.Registrar;

namespace ZebrunnerAgent.Logging
{
    [Target("Zebrunner")]
    public class ZebrunnerNLogTarget : TargetWithLayout
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        private readonly LogsBuffer _logsBuffer = LogsBuffer.Instance;

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