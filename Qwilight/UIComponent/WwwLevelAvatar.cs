using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct WwwLevelAvatar
    {
        public AvatarWww AvatarWwwValue { get; init; }

        public string AvatarName { get; init; }

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public override bool Equals(object obj) => obj is WwwLevelAvatar wwwLevelAvatar && AvatarWwwValue == wwwLevelAvatar.AvatarWwwValue;

        public override int GetHashCode() => AvatarWwwValue.GetHashCode();

        public static bool operator ==(WwwLevelAvatar left, WwwLevelAvatar right) => left.Equals(right);

        public static bool operator !=(WwwLevelAvatar left, WwwLevelAvatar right) => !(left == right);
    }
}