using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct BundleCompetence : IEquatable<BundleCompetence>
    {
        public const int BundleCallable = 0;
        public const int BundleAvatar = 3;
        public const int BundleUbuntu = 1;
        public const int BundleVoid = 2;

        public Brush Paint => Data switch
        {
            BundleCallable => Paints.Paint3,
            BundleAvatar => Paints.Paint3,
            BundleUbuntu => Paints.Paint2,
            BundleVoid => Paints.Paint1,
            _ => default
        };

        public int Data { get; init; }

        public override bool Equals(object obj) => obj is BundleCompetence bundleCompetence && Equals(bundleCompetence);

        public bool Equals(BundleCompetence other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString() => Data switch
        {
            BundleCallable => LanguageSystem.Instance.BundleCallableContents,
            BundleAvatar => LanguageSystem.Instance.BundleAvatarContents,
            BundleUbuntu => LanguageSystem.Instance.BundleUbuntuContents,
            BundleVoid => LanguageSystem.Instance.BundleVoidContents,
            _ => default
        };

        public static bool operator ==(BundleCompetence left, BundleCompetence right) => left.Equals(right);

        public static bool operator !=(BundleCompetence left, BundleCompetence right) => !(left == right);
    }
}