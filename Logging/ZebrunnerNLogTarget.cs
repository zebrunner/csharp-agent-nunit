using System;
using System.Collections.Generic;
using NLog;
using NLog.Targets;
using ZafiraIntegration.Client;
using ZafiraIntegration.Client.Requests;
using ZafiraIntegration.Registrar;

namespace ZafiraIntegration.Logging
{
    [Target("Zebrunner")]
    public class ZebrunnerNLogTarget : TargetWithLayout
    {
        private readonly ZebrunnerApiClient _apiClient = ZebrunnerApiClient.Instance;

        protected override void Write(LogEventInfo logEvent)
        {
            string logMessage = RenderLogEvent(Layout, logEvent);

            var log = new Log
            {
                TestId = RunContext.GetCurrentTest().Id,
                Level = logEvent.Level.Name,
                Message = logMessage,
                Timestamp = new DateTimeOffset(logEvent.TimeStamp).ToUnixTimeMilliseconds()
            };

            _apiClient.SendLogs(RunContext.SaveTestRunResponse.Id, new List<Log> {log});
        }
    }
}