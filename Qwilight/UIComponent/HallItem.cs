using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct HallItem : IEquatable<HallItem>
    {
        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string AvatarID { get; init; }

        public string AvatarName { get; init; }

        public string Value { get; init; }

        public AvatarWww AvatarWwwValue { get; }

        public HallItem(JSON.TwilightWwwHall twilightWwwHall, Func<double, string> onValue)
        {
            AvatarID = twilightWwwHall.avatarID;
            AvatarName = twilightWwwHall.avatarName;
            AvatarWwwValue = new AvatarWww(twilightWwwHall.avatarID);
            Value = onValue(twilightWwwHall.value);
        }

        public override bool Equals(object obj) => obj is HallItem hallItem && Equals(hallItem);

        public bool Equals(HallItem other) => AvatarID == other.AvatarID;

        public override int GetHashCode() => HashCode.Combine(AvatarID);

        public static bool operator ==(HallItem left, HallItem right) => left.Equals(right);

        public static bool operator !=(HallItem left, HallItem right) => !(left == right);
    }
}
