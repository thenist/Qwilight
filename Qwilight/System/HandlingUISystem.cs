using System.Windows.Threading;

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
                e.Handled = true;
                onUnhandledFault(e.Exception);
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
                return UIHandler.Invoke(onHandle);
            }
        }
    }
}
