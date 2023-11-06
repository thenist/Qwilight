using Windows.Media.MediaProperties;

namespace Qwilight.UIComponent
{
    public struct MediaInputQuality : IEquatable<MediaInputQuality>, IComparable<MediaInputQuality>
    {
        public VideoEncodingProperties Data { get; init; }

        public int CompareTo(MediaInputQuality other) => (other.Data.Width * other.Data.Height).CompareTo(Data.Width * Data.Height);

        public override bool Equals(object obj) => obj is MediaInputQuality mediaInputQuality && Equals(mediaInputQuality);

        public bool Equals(MediaInputQuality other) => other.Data.Width == Data.Width && other.Data.Height == Data.Height && other.Data.Subtype == Data.Subtype;

        public override int GetHashCode() => HashCode.Combine(Data.Width, Data.Height, Data.Subtype);

        public static bool operator ==(MediaInputQuality left, MediaInputQuality right) => left.Equals(right);

        public static bool operator !=(MediaInputQuality left, MediaInputQuality right) => !(left == right);

        public override string ToString() => $"{Data.Width}×{Data.Height} ({Data.Subtype})";
    }
}
