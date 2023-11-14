using System.Globalization;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static double ToFloat64(string text) => Convert.ToDouble(text, CultureInfo.InvariantCulture);

        public static bool ToFloat64(string text, out double value) => double.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);

        public static float ToFloat32(string text) => Convert.ToSingle(text, CultureInfo.InvariantCulture);

        public static int ToInt32(object o) => Convert.ToInt32(o, CultureInfo.InvariantCulture);

        public static long ToInt64(object o) => Convert.ToInt64(o, CultureInfo.InvariantCulture);

        public static bool ToInt32(string text, out int value) => int.TryParse(text, NumberStyles.Number, CultureInfo.InvariantCulture, out value);

        public static bool ToBool(string text) => Convert.ToBoolean(text, CultureInfo.InvariantCulture);
    }
}
