namespace Qwilight
{
    public struct TargetID : IEquatable<TargetID>
    {
        public float targetLength;
        public float targetHeight;

        public override bool Equals(object obj) => obj is TargetID targetID && Equals(targetID);

        public bool Equals(TargetID other) => targetLength == other.targetLength &&
            targetHeight == other.targetHeight;

        public override int GetHashCode() => HashCode.Combine(targetLength, targetHeight);

        public static bool operator ==(TargetID left, TargetID right) => left.Equals(right);

        public static bool operator !=(TargetID left, TargetID right) => !(left == right);
    }
}
