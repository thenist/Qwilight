namespace Qwilight.UIComponent
{
    public struct IOCompetence : IEquatable<IOCompetence>
    {
        public const int IOCallable = 0;
        public const int IOAvatar = 3;
        public const int IOUbuntu = 1;
        public const int IOVoid = 2;

        public int Data { get; init; }

        public override bool Equals(object obj) => obj is IOCompetence ioCompetence && Equals(ioCompetence);

        public bool Equals(IOCompetence other) => Data == other.Data;

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString() => Data switch
        {
            IOCallable => LanguageSystem.Instance.IOCallableContents,
            IOAvatar => LanguageSystem.Instance.IOAvatarContents,
            IOUbuntu => LanguageSystem.Instance.IOUbuntuContents,
            IOVoid => LanguageSystem.Instance.IOVoidContents,
            _ => default
        };

        public static bool operator ==(IOCompetence left, IOCompetence right) => left.Equals(right);

        public static bool operator !=(IOCompetence left, IOCompetence right) => !(left == right);
    }
}