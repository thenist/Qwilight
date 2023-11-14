using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct FontFamilyItem : IEquatable<FontFamilyItem>
    {
        public FontFamily FontFamilyValue { get; init; }

        public ICollection<string> FontFamilyNames => FontFamilyValue.FamilyNames.Values;

        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public override bool Equals(object obj) => obj is FontFamilyItem fontFamilyItem && Equals(fontFamilyItem);

        public bool Equals(FontFamilyItem other) => FontFamilyValue == other.FontFamilyValue;

        public override int GetHashCode() => FontFamilyValue.GetHashCode();

        public override string ToString() => FontFamilyValue.ToString();

        public static bool operator ==(FontFamilyItem left, FontFamilyItem right) => left.Equals(right);

        public static bool operator !=(FontFamilyItem left, FontFamilyItem right) => !(left == right);
    }
}