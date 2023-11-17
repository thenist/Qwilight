using System.Windows.Threading;

namespace Qwilight
{
    public sealed class UIHandler
    {
        public static readonly UIHandler Instance = new();

        public Dispatcher MainHandler { get; set; }

        public void Init(Action<Exception> onUnhandledFault)
        {
            MainHandler = Dispatcher.CurrentDispatcher;
            MainHandler.UnhandledException += (sender, e) =>
            {
                e.Handled = true;
                onUnhandledFault(e.Exception);
            };
        }

        public void HandleParallel(Action onHandle)
        {
            if (MainHandler.CheckAccess())
            {
                onHandle();
            }
            else
            {
                MainHandler.InvokeAsync(onHandle);
            }
        }
    }
}
