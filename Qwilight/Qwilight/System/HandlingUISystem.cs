﻿using System.Windows.Threading;

namespace Qwilight
{
    public sealed class HandlingUISystem
    {
        public static readonly HandlingUISystem Instance = new();

        public Dispatcher UIHandler { get; set; }

        public void Init(Action<Exception> onUnhandledFault)
        {
            UIHandler = Dispatcher.CurrentDispatcher;
            UIHandler.UnhandledException += (sender, e) =>
            {
                onUnhandledFault(e.Exception);
                e.Handled = true;
            };
        }

        public void HandleParallel(Action onHandle)
        {
            if (UIHandler.CheckAccess())
            {
                onHandle();
            }
            else
            {
                UIHandler.InvokeAsync(onHandle);
            }
        }

        public T Handle<T>(Func<T> onHandle)
        {
            if (UIHandler.CheckAccess())
            {
                return onHandle();
            }
            else
            {
                var handleCSX = new object();
                var isHandling = true;
                T value = default;
                UIHandler.InvokeAsync(() =>
                {
                    try
                    {
                        value = onHandle();
                    }
                    finally
                    {
                        lock (handleCSX)
                        {
                            isHandling = false;
                            Monitor.Pulse(handleCSX);
                        }
                    }
                });
                lock (handleCSX)
                {
                    if (isHandling)
                    {
                        Monitor.Wait(handleCSX);
                    }
                }
                return value;
            }
        }
    }
}
