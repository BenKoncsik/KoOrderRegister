using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Utility
{
    public static class ThreadManager
    {
        private static int MAX_DEGREE_OF_PARALLELISM = Environment.ProcessorCount;
        private static SemaphoreSlim SEMAPHORE => new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

        public static Task Run(Func<Task> action)
        {
            return Task.Run(async () =>
            {
                await SEMAPHORE.WaitAsync();
                try
                {
                    await action();
                }
                finally
                {
                    SEMAPHORE.Release();
                }
            });
        }

        public static Task<TResult> Run<TResult>(Func<Task<TResult>> action)
        {
            return Task.Run(async () =>
            {
                await SEMAPHORE.WaitAsync();
                try
                {
                    return await action();
                }
                finally
                {
                    SEMAPHORE.Release();
                }
            });
        }
    }
}
