﻿using System.Collections.Concurrent;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static Thread GetLongParallel(ThreadStart onHandle, bool isEssential = true) => new Thread(onHandle)
        {
            IsBackground = true,
            Priority = isEssential ? ThreadPriority.Highest : ThreadPriority.Normal
        };

        public static Thread HandleLongParallel(ThreadStart onHandle, bool isEssential = true)
        {
            var t = GetLongParallel(onHandle, isEssential);
            t.Start();
            return t;
        }

        public static void HandleHMP<T>(IProducerConsumerCollection<T> parallelItems, int onHandleBin, Action<T> onHandle, CancellationToken? setCancelParallel = null)
        {
            var longParallels = new Thread[onHandleBin];
            for (var i = longParallels.Length - 1; i >= 0; --i)
            {
                var longParallel = new Thread(() =>
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
                longParallel.Start();
                longParallels[i] = longParallel;
            }
            foreach (var longParallel in longParallels)
            {
                longParallel.Join();
            }
            setCancelParallel?.ThrowIfCancellationRequested();
        }
    }
}
