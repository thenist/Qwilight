using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight.UIComponent
{
    public struct AvatarTitleItem : IEquatable<AvatarTitleItem>
    {
        public Brush PointedPaint => TitlePaint;

        public string TitleID { get; init; }

        public string Title { get; init; }

        public Brush TitlePaint { get; init; }

        public Color TitleColor { get; init; }

        public override bool Equals(object obj) => obj is AvatarTitleItem avatarTitleItem && Equals(avatarTitleItem);

        public bool Equals(AvatarTitleItem other) => TitleID == other.TitleID;

        public override int GetHashCode() => TitleID.GetHashCode();

        public static bool operator ==(AvatarTitleItem left, AvatarTitleItem right) => left.Equals(right);

        public static bool operator !=(AvatarTitleItem left, AvatarTitleItem right) => !(left == right);
    }
}
