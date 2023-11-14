namespace Qwilight.UIComponent
{
    public struct HunterVariety : IEquatable<HunterVariety>
    {
        public const int HunterVariety1st = 0;
        public const int HunterVarietyHigher = 1;
        public const int HunterVarietyLower = 2;
        public const int HunterVarietyFavor = 3;
        public const int HunterVarietyMe = 4;

        public int Mode { get; init; }

        public override bool Equals(object obj) => obj is HunterVariety hunterVariety && Equals(hunterVariety);

        public bool Equals(HunterVariety other) => Mode == other.Mode;

        public override int GetHashCode() => Mode.GetHashCode();

        public override string ToString() => Mode switch
        {
            HunterVariety1st => LanguageSystem.Instance.HunterVariety1stText,
            HunterVarietyHigher => LanguageSystem.Instance.HunterVarietyHigherText,
            HunterVarietyLower => LanguageSystem.Instance.HunterVarietyLowerText,
            HunterVarietyFavor => LanguageSystem.Instance.HunterVarietyFavorText,
            HunterVarietyMe => LanguageSystem.Instance.HunterVarietyMeText,
            _ => default
        };

        public static bool operator ==(HunterVariety left, HunterVariety right) => left.Equals(right);

        public static bool operator !=(HunterVariety left, HunterVariety right) => !(left == right);
    }
}