namespace Qwilight.UIComponent
{
    public struct NotifyUbuntuCompetence : IEquatable<NotifyUbuntuCompetence>
    {
        public const int NotifyUbuntu = 0;
        public const int NotNotifyUbuntu = 1;

        public int Data { get; init; }

        public override bool Equals(object obj) => obj is NotifyUbuntuCompetence toNotifyUbuntuCompetence && Equals(toNotifyUbuntuCompetence);

        public bool Equals(NotifyUbuntuCompetence other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString() => Data switch
        {
            NotifyUbuntu => LanguageSystem.Instance.NotifyUbuntuContents,
            NotNotifyUbuntu => LanguageSystem.Instance.NotNotifyUbuntuContents,
            _ => default
        };

        public static bool operator ==(NotifyUbuntuCompetence left, NotifyUbuntuCompetence right) => left.Equals(right);

        public static bool operator !=(NotifyUbuntuCompetence left, NotifyUbuntuCompetence right) => !(left == right);
    }
}