namespace Qwilight.UIComponent
{
    public struct HwMode : IEquatable<HwMode>, IComparable<HwMode>
    {
        public HwMode(uint dmPelsWidth, uint dmPelsHeight, uint dmDisplayFrequency)
        {
            Length = dmPelsWidth;
            Height = dmPelsHeight;
            Hz = dmDisplayFrequency;
        }

        public uint Length { get; init; }

        public uint Height { get; init; }

        public uint Hz { get; init; }

        public int CompareTo(HwMode other) => (other.Length * other.Height * other.Hz).CompareTo(Length * Height * Hz);

        public override bool Equals(object obj) => obj is HwMode hwMode && Equals(hwMode);

        public bool Equals(HwMode other) => other.Length == Length && other.Height == Height && other.Hz == Hz;

        public override int GetHashCode() => HashCode.Combine(Length, Height, Hz);

        public override string ToString() => $"{Length}×{Height} ({Hz} Hz)";

        public static bool operator ==(HwMode left, HwMode right) => left.Equals(right);

        public static bool operator !=(HwMode left, HwMode right) => !(left == right);
    }
}
