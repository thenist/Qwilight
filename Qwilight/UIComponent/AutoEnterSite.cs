namespace Qwilight.UIComponent
{
    public struct AutoEnterSite : IEquatable<AutoEnterSite>
    {
        public const int AutoEnter = 0;
        public const int AutoEnterSignedIn = 1;
        public const int WaitSite = 2;

        public int Data { get; init; }

        public override bool Equals(object obj) => obj is AutoEnterSite autoEnterSite && Equals(autoEnterSite);

        public bool Equals(AutoEnterSite other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString() => Data switch
        {
            AutoEnter => LanguageSystem.Instance.AutoEnterContents,
            AutoEnterSignedIn => LanguageSystem.Instance.AutoEnterSignedInContents,
            WaitSite => LanguageSystem.Instance.WaitSiteContents,
            _ => default
        };

        public static bool operator ==(AutoEnterSite left, AutoEnterSite right) => left.Equals(right);

        public static bool operator !=(AutoEnterSite left, AutoEnterSite right) => !(left == right);
    }
}