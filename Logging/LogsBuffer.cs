using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using NLog;
using ZebrunnerAgent.Client;
using ZebrunnerAgent.Client.Requests;
using ZebrunnerAgent.Registrar;

namespace ZebrunnerAgent.Logging
{
    internal class LogsBuffer
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        internal static LogsBuffer Instance { get; } = new LogsBuffer();

        private volatile ConcurrentQueue<Log> _buffer = new ConcurrentQueue<Log>();
        private readonly ZebrunnerApiClient _apiClient = ZebrunnerApiClient.Instance;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private LogsBuffer()
        {
            Task.Run(() =>
            {
                Logger.Info("Starting a recurring logs flushing task");
                while (!_cancellationTokenSource.IsCancellationRequested)
                {
                    Flush();
                    Task.Delay(TimeSpan.FromSeconds(1), _cancellationTokenSource.Token);
                }

                Logger.Info("Logs flushing task finished");
            });
        }

        internal void Add(Log log)
        {
            _buffer.Enqueue(log);
        }

        private void Flush()
        {
            // theoretically, not a fully thread-safe rotation
            var previousBuffer = _buffer;
            _buffer = new ConcurrentQueue<Log>();

            var testRun = RunContext.GetCurrentTestRun();
            if (testRun != null)
            {
                var logs = new List<Log>();
                logs.AddRange(previousBuffer);

                _apiClient.SendLogs(testRun.Id, logs);
            }
        }

        ~LogsBuffer()
        {
            Logger.Info("Finalizing logs buffer...");
            _cancellationTokenSource.Cancel();
            Flush();
        }
    }
}