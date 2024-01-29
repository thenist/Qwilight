using Windows.Foundation;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static void Await(this IAsyncAction awaitable)
        {
            using var t = awaitable.AsTask();
            t.Wait();
        }
        public static T Await<T>(this IAsyncOperation<T> awaitable)
        {
            using var t = awaitable.AsTask();
            t.Wait();
            return t.Result;
        }

        public static T Await<T, U>(this IAsyncOperationWithProgress<T, U> awaitable)
        {
            using var t = awaitable.AsTask();
            t.Wait();
            return t.Result;
        }
    }
}
