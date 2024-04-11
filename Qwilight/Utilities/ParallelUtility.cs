using System.Collections.Concurrent;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static Thread GetParallelHandler(ThreadStart onHandle, bool isEssential = true) => new Thread(onHandle)
        {
            IsBackground = true,
            Priority = isEssential ? ThreadPriority.Highest : ThreadPriority.Lowest
        };

        public static Thread HandleParallelly(ThreadStart onHandle, bool isEssential = true)
        {
            var t = GetParallelHandler(onHandle, isEssential);
            t.Start();
            return t;
        }

        public static void HandleLowestlyParallelly<T>(IProducerConsumerCollection<T> parallelItems, int onHandleBin, Action<T> onHandle, CancellationToken? setCancelParallel = null)
        {
            var ts = new Thread[onHandleBin];
            for (var i = ts.Length - 1; i >= 0; --i)
            {
                var t = new Thread(() =>
                {
                    while (setCancelParallel?.IsCancellationRequested != true && parallelItems.TryTake(out var parallelItem))
                    {
                        onHandle(parallelItem);
                    }
                })
                {
                    IsBackground = true,
                    Priority = ThreadPriority.Lowest
                };
                t.Start();
                ts[i] = t;
            }
            foreach (var longParallel in ts)
            {
                longParallel.Join();
            }
            setCancelParallel?.ThrowIfCancellationRequested();
        }
    }
}
