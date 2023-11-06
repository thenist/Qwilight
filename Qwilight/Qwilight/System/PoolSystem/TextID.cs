using Microsoft.Graphics.Canvas.Text;

namespace Qwilight
{
    public struct TextID : IEquatable<TextID>
    {
        public string text;
        public float fontLength;
        public float textLength;
        public float textHeight;
        public CanvasHorizontalAlignment textSystem0;
        public CanvasVerticalAlignment textSystem1;

        public override bool Equals(object obj) => obj is TextID textID && Equals(textID);

        public bool Equals(TextID other) => text == other.text &&
            fontLength == other.fontLength &&
            textLength == other.textLength &&
            textHeight == other.textHeight &&
            textSystem0 == other.textSystem0 &&
            textSystem1 == other.textSystem1;

        public override int GetHashCode() => HashCode.Combine(text, fontLength, textLength, textHeight, textSystem0, textSystem1);

        public static bool operator ==(TextID left, TextID right) => left.Equals(right);

        public static bool operator !=(TextID left, TextID right) => !(left == right);
    }
}
