namespace Qwilight.UIComponent
{
    public struct SilentSiteCompetence : IEquatable<SilentSiteCompetence>
    {
        public const int SilentSiteCallable = 0;
        public const int SilentSiteAvatar = 3;
        public const int SilentSiteUbuntu = 1;
        public const int SilentSiteVoid = 2;

        public int Data { get; init; }

        public override bool Equals(object obj) => obj is SilentSiteCompetence silentSiteCompetence && Equals(silentSiteCompetence);

        public bool Equals(SilentSiteCompetence other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString() => Data switch
        {
            SilentSiteCallable => LanguageSystem.Instance.SilentSiteCallableContents,
            SilentSiteAvatar => LanguageSystem.Instance.SilentSiteAvatarContents,
            SilentSiteUbuntu => LanguageSystem.Instance.SilentSiteUbuntuContents,
            SilentSiteVoid => LanguageSystem.Instance.SilentSiteVoidContents,
            _ => default
        };

        public static bool operator ==(SilentSiteCompetence left, SilentSiteCompetence right) => left.Equals(right);

        public static bool operator !=(SilentSiteCompetence left, SilentSiteCompetence right) => !(left == right);
    }
}