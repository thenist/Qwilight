using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct FrontEntryItem : IEquatable<FrontEntryItem>
    {
        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string FrontEntryPath { get; init; }

        public override bool Equals(object obj) => obj is FrontEntryItem frontEntryItem && Equals(frontEntryItem);

        public bool Equals(FrontEntryItem other) => FrontEntryPath == other.FrontEntryPath;

        public override int GetHashCode() => FrontEntryPath.GetHashCode();

        public static bool operator ==(FrontEntryItem left, FrontEntryItem right) => left.Equals(right);

        public static bool operator !=(FrontEntryItem left, FrontEntryItem right) => !(left == right);
    }
}