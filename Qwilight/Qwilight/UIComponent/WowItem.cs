using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct WowItem : IEquatable<WowItem>
    {
        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string AvatarID { get; init; }

        public string AvatarName { get; init; }

        public string Value { get; init; }

        public AvatarWww AvatarWwwValue { get; }

        public WowItem(JSON.TwilightWwwWow.Avatar wwwWowAvatar, Func<double, string> onValue)
        {
            AvatarID = wwwWowAvatar.avatarID;
            AvatarName = wwwWowAvatar.avatarName;
            AvatarWwwValue = new AvatarWww(wwwWowAvatar.avatarID);
            Value = onValue(wwwWowAvatar.value);
        }

        public override bool Equals(object obj) => obj is WowItem wowItem && Equals(wowItem);

        public bool Equals(WowItem other) => AvatarID == other.AvatarID;

        public override int GetHashCode() => HashCode.Combine(AvatarID);

        public static bool operator ==(WowItem left, WowItem right) => left.Equals(right);

        public static bool operator !=(WowItem left, WowItem right) => !(left == right);
    }
}
