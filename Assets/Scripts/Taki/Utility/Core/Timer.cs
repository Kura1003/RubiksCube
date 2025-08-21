using System;
using System.Diagnostics;

namespace Taki.Utility.Core
{
    internal static class Timer
    {
        internal static IDisposable Measure(string message)
        {
            return new PerformanceTimer(message);
        }

        private sealed class PerformanceTimer : IDisposable
        {
            private readonly Stopwatch _stopwatch;
            private readonly string _message;

            public PerformanceTimer(string message)
            {
                _message = message;
                _stopwatch = Stopwatch.StartNew();
            }

            public void Dispose()
            {
                _stopwatch.Stop();
                Console.WriteLine($"{_message} ÇÃé¿çséûä‘: {_stopwatch.ElapsedMilliseconds} ms");
            }
        }
    }
}