namespace Qwilight
{
    public struct ValueTextID<T> : IEquatable<ValueTextID<T>> where T : IEquatable<T>
    {
        public T value;
        public string textFormat;

        public override bool Equals(object obj) => obj is ValueTextID<T> valueTextID && Equals(valueTextID);

        public bool Equals(ValueTextID<T> other) => value.Equals(other.value) && textFormat == other.textFormat;

        public override int GetHashCode() => HashCode.Combine(value, textFormat);

        public static bool operator ==(ValueTextID<T> left, ValueTextID<T> right) => left.Equals(right);

        public static bool operator !=(ValueTextID<T> left, ValueTextID<T> right) => !(left == right);
    }
}
