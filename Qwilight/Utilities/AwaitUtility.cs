using Windows.Foundation;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static T Await<T>(IAsyncOperation<T> t)
        {
            return Await(t.AsTask());
        }

        public static T Await<T>(Task<T> t)
        {
            t.Wait();
            return t.Result;
        }
    }
}
