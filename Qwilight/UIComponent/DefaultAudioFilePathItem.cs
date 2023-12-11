using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct DefaultAudioFilePathItem : IEquatable<DefaultAudioFilePathItem>
    {
        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string Value { get; init; }

        public override bool Equals(object obj) => obj is DefaultAudioFilePathItem item && Equals(item);

        public bool Equals(DefaultAudioFilePathItem other) => Value == other.Value;

        public override int GetHashCode() => Value.GetHashCode();

        public static bool operator ==(DefaultAudioFilePathItem left, DefaultAudioFilePathItem right) => left.Equals(right);

        public static bool operator !=(DefaultAudioFilePathItem left, DefaultAudioFilePathItem right) => !(left == right);
    }
}
