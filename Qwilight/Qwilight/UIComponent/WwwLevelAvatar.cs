using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class WwwLevelAvatar
    {
        public AvatarWww AvatarWwwValue { get; init; }

        public string AvatarName { get; init; }

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public override bool Equals(object obj) => obj is WwwLevelAvatar wwwLevelAvatar && AvatarWwwValue == wwwLevelAvatar.AvatarWwwValue;

        public override int GetHashCode() => AvatarWwwValue.GetHashCode();
    }
}