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
        private static SemaphoreSlim SEMAPHORE = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);
        private static SemaphoreSlim HighPrioritySemaphore = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);
        private static SemaphoreSlim NormalPrioritySemaphore = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);
        private static SemaphoreSlim LowPrioritySemaphore = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM);

        public enum Priority { Low, Normal, High }
        private static readonly SortedDictionary<Priority, Queue<Func<Task>>> TaskQueues = new SortedDictionary<Priority, Queue<Func<Task>>>
        {
            { Priority.High, new Queue<Func<Task>>() },
            { Priority.Normal, new Queue<Func<Task>>() },
            { Priority.Low, new Queue<Func<Task>>() }
        };
        private static readonly object QueueLock = new object();
        #region Runs
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
        #endregion

        public static void PausePriorityTasks()
        {
            NormalPrioritySemaphore = new SemaphoreSlim(0);
            LowPrioritySemaphore = new SemaphoreSlim(0);
        }

        public static void ResumePriorityTasks()
        {
            NormalPrioritySemaphore = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM - TaskQueues[Priority.High].Count);
            LowPrioritySemaphore = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM - TaskQueues[Priority.High].Count - TaskQueues[Priority.Normal].Count);
            ProcessQueue();
        }
        private static void ProcessQueue()
        {
            Task.Run(async () =>
            {
                
              //  Debug.WriteLine($"Semaphore wait: {MAX_DEGREE_OF_PARALLELISM - SEMAPHORE.CurrentCount}");
               // await SEMAPHORE.WaitAsync();
                SemaphoreSlim semaphoreToUse = null;
                Func<Task> taskToRun = null;

                lock (QueueLock)
                {
                    /*  foreach (var priority in new[] { Priority.High, Priority.Normal, Priority.Low })
                      {
                          if (TaskQueues[priority].Count > 0)
                          {
                              taskToRun = TaskQueues[priority].Dequeue();
                              break;
                          }
                      }*/

                    NormalPrioritySemaphore = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM - TaskQueues[Priority.High].Count);
                    LowPrioritySemaphore = new SemaphoreSlim(MAX_DEGREE_OF_PARALLELISM - TaskQueues[Priority.High].Count - TaskQueues[Priority.Normal].Count);
                    Debug.WriteLine($"Hight wait: {MAX_DEGREE_OF_PARALLELISM - HighPrioritySemaphore.CurrentCount} Normal: {MAX_DEGREE_OF_PARALLELISM - NormalPrioritySemaphore.CurrentCount} Low: {MAX_DEGREE_OF_PARALLELISM - LowPrioritySemaphore.CurrentCount}");
                    if (TaskQueues[Priority.High].Count > 0)
                    {
                        taskToRun = TaskQueues[Priority.High].Dequeue();
                        semaphoreToUse = HighPrioritySemaphore;
                    }
                    else if (TaskQueues[Priority.Normal].Count > 0)
                    {
                        taskToRun = TaskQueues[Priority.Normal].Dequeue();
                        semaphoreToUse = HighPrioritySemaphore;
                    }
                    else if (TaskQueues[Priority.Low].Count > 0)
                    {
                        taskToRun = TaskQueues[Priority.Low].Dequeue();
                        semaphoreToUse = LowPrioritySemaphore;
                    }
                }
                if (taskToRun != null && semaphoreToUse != null)
                {
                    await semaphoreToUse.WaitAsync();
                    try
                    {
                        await taskToRun();
                    }
                    finally
                    {
                        semaphoreToUse.Release();
                        ProcessQueue();
                        Debug.WriteLine($"Hight Release: {MAX_DEGREE_OF_PARALLELISM - HighPrioritySemaphore.CurrentCount} Normal: {MAX_DEGREE_OF_PARALLELISM - NormalPrioritySemaphore.CurrentCount} Low: {MAX_DEGREE_OF_PARALLELISM - LowPrioritySemaphore.CurrentCount}");
                    }
                }
                /* if (taskToRun != null)
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
                 }*/


            });
        }
    }
}
