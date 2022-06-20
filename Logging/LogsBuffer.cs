using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using ZebrunnerAgent.Client;
using ZebrunnerAgent.Client.Requests;
using ZebrunnerAgent.Registrar;

namespace ZebrunnerAgent.Logging
{
    internal class LogsBuffer
    {
        internal static LogsBuffer Instance { get; } = new LogsBuffer();

        private volatile ConcurrentQueue<Log> _buffer = new ConcurrentQueue<Log>();
        private readonly ZebrunnerApiClient _apiClient = ZebrunnerApiClient.Instance;
        private readonly CancellationTokenSource _cancellationTokenSource = new CancellationTokenSource();

        private LogsBuffer()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    if (!_cancellationTokenSource.IsCancellationRequested)
                    {
                        Thread.Sleep(TimeSpan.FromSeconds(1));
                        Flush();
                    }
                    else
                    {
                        Flush();
                        return;
                    }
                }
            }, _cancellationTokenSource.Token);
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
            _cancellationTokenSource.Cancel();
        }
    }
}