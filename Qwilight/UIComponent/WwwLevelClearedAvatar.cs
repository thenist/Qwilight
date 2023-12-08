using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct WwwLevelClearedAvatar
    {
        public AvatarWww AvatarWwwValue { get; init; }

        public string AvatarName { get; init; }

        public string Date { get; init; }

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public override bool Equals(object obj) => obj is WwwLevelClearedAvatar wwwLevelClearedAvatar && AvatarWwwValue == wwwLevelClearedAvatar.AvatarWwwValue;

        public override int GetHashCode() => AvatarWwwValue.GetHashCode();

        public static bool operator ==(WwwLevelClearedAvatar left, WwwLevelClearedAvatar right) => left.Equals(right);

        public static bool operator !=(WwwLevelClearedAvatar left, WwwLevelClearedAvatar right) => !(left == right);
    }
}