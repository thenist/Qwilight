using System;
using System.Globalization;

namespace Igniter.Utilities
{
    public static partial class Utility
    {
        public static int ToInt32(object o) => Convert.ToInt32(o, CultureInfo.InvariantCulture);
    }
}
