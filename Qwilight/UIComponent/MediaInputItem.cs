namespace Qwilight.UIComponent
{
    public struct MediaInputItem : IEquatable<MediaInputItem>
    {
        public string ID { get; init; }

        public string Name { get; init; }

        public override bool Equals(object obj) => obj is MediaInputItem mediaInputItem && Equals(mediaInputItem);

        public bool Equals(MediaInputItem other) => ID == other.ID;

        public override int GetHashCode() => ID.GetHashCode();

        public override string ToString() => Name;

        public static bool operator ==(MediaInputItem left, MediaInputItem right) => left.Equals(right);

        public static bool operator !=(MediaInputItem left, MediaInputItem right) => !(left == right);
    }
}