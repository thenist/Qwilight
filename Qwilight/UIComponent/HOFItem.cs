using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct HOFItem : IEquatable<HOFItem>
    {
        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string AvatarID { get; init; }

        public string AvatarName { get; init; }

        public string Value { get; init; }

        public AvatarWww AvatarWwwValue { get; }

        public HOFItem(JSON.TwilightWwwHOF twilightWwwHOF, Func<double, string> onValue)
        {
            AvatarID = twilightWwwHOF.avatarID;
            AvatarName = twilightWwwHOF.avatarName;
            AvatarWwwValue = new AvatarWww(twilightWwwHOF.avatarID);
            Value = onValue(twilightWwwHOF.value);
        }

        public override bool Equals(object obj) => obj is HOFItem hofItem && Equals(hofItem);

        public bool Equals(HOFItem other) => AvatarID == other.AvatarID;

        public override int GetHashCode() => HashCode.Combine(AvatarID);

        public static bool operator ==(HOFItem left, HOFItem right) => left.Equals(right);

        public static bool operator !=(HOFItem left, HOFItem right) => !(left == right);
    }
}
