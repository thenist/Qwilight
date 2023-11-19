using System.Windows.Threading;

namespace Qwilight
{
    public sealed class UIHandler
    {
        public static readonly UIHandler Instance = new();

        public Dispatcher Handler { get; set; }

        public void Init(Action<Exception> onUnhandledFault)
        {
            Handler = Dispatcher.CurrentDispatcher;
            Handler.UnhandledException += (sender, e) =>
            {
                e.Handled = true;
                onUnhandledFault(e.Exception);
            };
        }

        public void HandleParallel(Action onHandle)
        {
            if (Handler.CheckAccess())
            {
                onHandle();
            }
            else
            {
                Handler.InvokeAsync(onHandle);
            }
        }
    }
}
