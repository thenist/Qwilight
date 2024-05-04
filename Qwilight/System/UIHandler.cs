using System.Windows.Threading;

namespace Qwilight
{
    public sealed class UIHandler
    {
        public static readonly UIHandler Instance = new();

        public Dispatcher Handler { get; set; }

        public void Init(Dispatcher handler)
        {
            Handler = handler;
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
