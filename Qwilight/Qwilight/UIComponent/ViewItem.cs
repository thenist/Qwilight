namespace Qwilight.UIComponent
{
    public struct ViewItem : IEquatable<ViewItem>
    {
        public const int Always = 0;
        public const int NotAutoCompute = 1;
        public const int Not = 2;

        public int Data { get; init; }

        public override bool Equals(object obj) => obj is ViewItem viewItem && Equals(viewItem);

        public bool Equals(ViewItem other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString() => Data switch
        {
            Always => LanguageSystem.Instance.AlwaysViewContents,
            NotAutoCompute => LanguageSystem.Instance.NotAutoComputeViewContents,
            Not => LanguageSystem.Instance.NotViewContents,
            _ => default
        };

        public static bool operator ==(ViewItem left, ViewItem right) => left.Equals(right);

        public static bool operator !=(ViewItem left, ViewItem right) => !(left == right);
    }
}