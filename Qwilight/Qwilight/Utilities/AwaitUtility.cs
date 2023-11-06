﻿using Windows.Foundation;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static T Await<T>(IAsyncOperation<T> awaitable)
        {
            using var t = awaitable.AsTask();
            t.Wait();
            return t.Result;
        }
    }
}
