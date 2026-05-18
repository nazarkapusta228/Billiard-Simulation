using System;
using System.Collections.Concurrent;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BilliardSimulation.Data
{
    public class DiagnosticLogger : IDiagnosticLogger
    {
        private readonly BlockingCollection<string> _queue =
    new BlockingCollection<string>(boundedCapacity: 1000);

        private readonly CancellationTokenSource _cts =
            new CancellationTokenSource();

        private readonly Task _loggingTask;

        private readonly string _filePath = "diagnostics.txt";

        public DiagnosticLogger()
        {
            _loggingTask = Task.Run(ProcessQueueAsync);
        }

        public void LogBallState(Ball ball)
        {
            ball.GetState(
                out double x,
                out double y,
                out double vx,
                out double vy,
                out double radius,
                out double mass);

            string log =
                $"{DateTime.Now:HH:mm:ss.fff}; " +
                $"X={x:F2}; Y={y:F2}; " +
                $"VX={vx:F2}; VY={vy:F2}; " +
                $"M={mass:F2}";

            _queue.Add(log);
        }

        private async Task ProcessQueueAsync()
        {
            using var writer = new StreamWriter(
                _filePath,
                append: true,
                Encoding.ASCII);

            try
            {
                while (!_cts.Token.IsCancellationRequested)
                {
                    string log = _queue.Take(_cts.Token);

                    await writer.WriteLineAsync(log);

                    await writer.FlushAsync();

                    // симуляція повільного диска
                    await Task.Delay(5);
                }
            }
            catch (OperationCanceledException)
            {
            }
        }

        public async Task StopAsync()
        {
            _cts.Cancel();

            await _loggingTask;
        }
    }
}