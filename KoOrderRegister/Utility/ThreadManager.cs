using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace KoOrderRegister.Utility
{
    public static class ThreadManager
    {
        private static int MAX_DEGREE_OF_PARALLELISM = Environment.ProcessorCount;
        private static SemaphoreSlim SEMAPHORE => new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

        public enum Priority { Low, Normal, High }
        private static readonly SortedDictionary<Priority, Queue<Func<Task>>> TaskQueues = new SortedDictionary<Priority, Queue<Func<Task>>>
        {
            { Priority.High, new Queue<Func<Task>>() },
            { Priority.Normal, new Queue<Func<Task>>() },
            { Priority.Low, new Queue<Func<Task>>() }
        };
        private static readonly object QueueLock = new object();

        public static Task Run(Func<Task> action, Priority priority = Priority.Normal)
        {
            var tcs = new TaskCompletionSource<bool>();
            lock (QueueLock)
            {
                TaskQueues[priority].Enqueue(async () =>
                {
                    try
                    {
                        await action();
                        tcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });
            }
            ProcessQueue();
            return tcs.Task;
        }

        public static Task Run(Action action, Priority priority = Priority.Normal)
        {
            var tcs = new TaskCompletionSource<bool>();
            lock (QueueLock)
            {
                TaskQueues[priority].Enqueue(async () =>
                {
                    try
                    {
                        action();
                        tcs.SetResult(true);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });
            }
            ProcessQueue();
            return tcs.Task;
        }

        public static Task<TResult> Run<TResult>(Func<Task<TResult>> action, Priority priority = Priority.Normal)
        {
            var tcs = new TaskCompletionSource<TResult>();
            lock (QueueLock)
            {
                TaskQueues[priority].Enqueue(async () =>
                {
                    try
                    {
                        TResult result = await action();
                        tcs.SetResult(result);
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                    }
                });
            }
            ProcessQueue();
            return tcs.Task;
        }

        public static Task<TResult> Run<TResult>(Func<TResult> action, Priority priority = Priority.Normal)
        {
            var tcs = new TaskCompletionSource<TResult>();
            lock (QueueLock)
            {
                TaskQueues[priority].Enqueue(() =>
                {
                    try
                    {
                        TResult result = action();
                        tcs.SetResult(result);
                        return Task.CompletedTask;
                    }
                    catch (Exception ex)
                    {
                        tcs.SetException(ex);
                        return Task.CompletedTask;
                    }
                });
            }
            ProcessQueue();
            return tcs.Task;
        }
    private static void ProcessQueue()
        {
            Task.Run(async () =>
            {
                Debug.WriteLine($"Semaphore wait: {MAX_DEGREE_OF_PARALLELISM - SEMAPHORE.CurrentCount}");
                await SEMAPHORE.WaitAsync();
                Func<Task> taskToRun = null;

                lock (QueueLock)
                {
                    foreach (var priority in new[] { Priority.High, Priority.Normal, Priority.Low })
                    {
                        if (TaskQueues[priority].Count > 0)
                        {
                            taskToRun = TaskQueues[priority].Dequeue();
                            break;
                        }
                    }
                }

                if (taskToRun != null)
                {
                    try
                    {
                        await taskToRun();
                    }
                    finally
                    {
                        SEMAPHORE.Release();
                        ProcessQueue();
                    }
                }
                else
                {
                    SEMAPHORE.Release();
                    Debug.WriteLine($"Semaphore release: {MAX_DEGREE_OF_PARALLELISM - SEMAPHORE.CurrentCount}");
                }
            });
        }
    }
}
