namespace Qwilight.UIComponent
{
    public struct NotifySaveBundleCompetence : IEquatable<NotifySaveBundleCompetence>
    {
        public const int NotifySaveBundleCallable = 0;
        public const int NotifySaveBundleAvatar = 1;
        public const int NotifySaveBundleUbuntu = 2;
        public const int NotifySaveBundleVoid = 3;

        public int Data { get; init; }

        public override bool Equals(object obj) => obj is NotifySaveBundleCompetence toNotifySaveBundleCompetence && Equals(toNotifySaveBundleCompetence);

        public bool Equals(NotifySaveBundleCompetence other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString() => Data switch
        {
            NotifySaveBundleCallable => LanguageSystem.Instance.NotifySaveBundleCallableContents,
            NotifySaveBundleAvatar => LanguageSystem.Instance.NotifySaveBundleAvatarContents,
            NotifySaveBundleUbuntu => LanguageSystem.Instance.NotifySaveBundleUbuntuContents,
            NotifySaveBundleVoid => LanguageSystem.Instance.NotifySaveBundleVoidContents,
            _ => default
        };

        public static bool operator ==(NotifySaveBundleCompetence left, NotifySaveBundleCompetence right) => left.Equals(right);

        public static bool operator !=(NotifySaveBundleCompetence left, NotifySaveBundleCompetence right) => !(left == right);
    }
}