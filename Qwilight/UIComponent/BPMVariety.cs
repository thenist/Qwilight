namespace Qwilight.UIComponent
{
    public struct BPMVariety : IEquatable<BPMVariety>
    {
        public const int BPM = 0;
        public const int AudioMultiplier = 1;
        public const int Multiplier = 2;

        public int Data { get; init; }

        public override bool Equals(object obj) => obj is BPMVariety bpmVariety && Equals(bpmVariety);

        public bool Equals(BPMVariety other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString() => Data switch
        {
            BPM => "BPM",
            AudioMultiplier => LanguageSystem.Instance.BPMVarietyAudioMultiplierText,
            Multiplier => LanguageSystem.Instance.BPMVarietyMultiplierText,
            _ => default
        };

        public static bool operator ==(BPMVariety left, BPMVariety right) => left.Equals(right);

        public static bool operator !=(BPMVariety left, BPMVariety right) => !(left == right);
    }
}