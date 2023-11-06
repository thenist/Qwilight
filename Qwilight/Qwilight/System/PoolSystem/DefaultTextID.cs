using System.Windows.Media;

namespace Qwilight
{
    public struct DefaultTextID : IEquatable<DefaultTextID>
    {
        public string text;
        public double fontLength;
        public double textLength;
        public Brush fontPaint;

        public override bool Equals(object obj) => obj is DefaultTextID defaultTextID && Equals(defaultTextID);

        public bool Equals(DefaultTextID other) => text == other.text &&
            fontLength == other.fontLength &&
            textLength == other.textLength &&
            fontPaint == other.fontPaint;

        public override int GetHashCode() => HashCode.Combine(text, fontLength, textLength, fontPaint);

        public static bool operator ==(DefaultTextID left, DefaultTextID right) => left.Equals(right);

        public static bool operator !=(DefaultTextID left, DefaultTextID right) => !(left == right);
    }
}
