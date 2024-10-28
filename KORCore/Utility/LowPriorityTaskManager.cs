using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KORCore.Utility
{
    public class LowPriorityTaskManager : IDisposable
    {
        private static int _instanceCount = 0;
        private static readonly SemaphoreSlim _semaphore = new SemaphoreSlim(1, 1);
        private static readonly SemaphoreSlim _instanceSemaphore = new SemaphoreSlim(0, int.MaxValue);
        private bool _disposed = false;

        public LowPriorityTaskManager()
        {
            Interlocked.Increment(ref _instanceCount);
            Debug.WriteLine($"LowPriorityTask created. Instance count: {_instanceCount}");
        }

        public void Dispose()
        {
            if (!_disposed)
            {
                _disposed = true;
                if (Interlocked.Decrement(ref _instanceCount) == 0)
                {
                    Debug.WriteLine("Last LowPriorityTask disposed. Releasing _instanceSemaphore.");
                    _instanceSemaphore.Release();
                }
                Debug.WriteLine($"LowPriorityTask disposed. Instance count: {_instanceCount}");
            }
        }

        public static async Task WaitAsyncRunLowPriority(CancellationToken token = default)
        {
            await _semaphore.WaitAsync(token);
            try
            {
#if DEBUG
                var stopwatch = Stopwatch.StartNew();
#endif
                while (Volatile.Read(ref _instanceCount) > 0)
                {
                    Debug.WriteLine("Waiting for all LowPriorityTask instances to be disposed...");
                    await _instanceSemaphore.WaitAsync(token);
                }
#if DEBUG
                stopwatch.Stop();
                Debug.WriteLine($"WaitAsyncRunLowPriority completed. Waited for {stopwatch.ElapsedMilliseconds} ms.");
#endif
            }
            finally
            {
                _semaphore.Release();
            }
        }
    }
}
