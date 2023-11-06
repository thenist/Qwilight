namespace Qwilight
{
    public struct FormattedTextID : IEquatable<FormattedTextID>
    {
        public string textFormat;
        public string param0;
        public string param1;
        public string param2;
        public string param3;

        public override bool Equals(object obj) => obj is FormattedTextID formattedTextID && Equals(formattedTextID);

        public bool Equals(FormattedTextID other) => textFormat == other.textFormat &&
            param0 == other.param0 &&
            param1 == other.param1 &&
            param2 == other.param2 &&
            param3 == other.param3;

        public override int GetHashCode() => HashCode.Combine(textFormat, param0, param1, param2, param3);

        public static bool operator ==(FormattedTextID left, FormattedTextID right) => left.Equals(right);

        public static bool operator !=(FormattedTextID left, FormattedTextID right) => !(left == right);
    }
}
