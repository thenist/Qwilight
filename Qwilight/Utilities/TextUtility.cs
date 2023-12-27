namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static bool EqualsCaseless(this string text, string value) => text.Equals(value, StringComparison.InvariantCultureIgnoreCase);

        public static bool IsFrontCaselsss(this string text, string value) => text.StartsWith(value, StringComparison.InvariantCultureIgnoreCase);

        public static bool IsTailCaselsss(this string text, string value) => text.EndsWith(value, StringComparison.InvariantCultureIgnoreCase);

        public static bool ContainsCaselsss(this string text, string value) => text.Contains(value, StringComparison.InvariantCultureIgnoreCase);

        public static int FitCaseless(this string text, string value) => string.Compare(text, value, StringComparison.InvariantCultureIgnoreCase);
    }
}
