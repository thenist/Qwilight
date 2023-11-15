using System.Windows.Threading;

namespace Qwilight
{
    public sealed class HandlingUISystem
    {
        public static readonly HandlingUISystem Instance = new();

        public Dispatcher UIHandler { get; set; }

        public void Init(Func<Exception, ValueTask> onUnhandledFault)
        {
            UIHandler = Dispatcher.CurrentDispatcher;
            UIHandler.UnhandledException += async (sender, e) =>
            {
                e.Handled = true;
                await onUnhandledFault(e.Exception).ConfigureAwait(false);
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
