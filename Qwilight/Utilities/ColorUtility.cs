using Microsoft.UI;
using System.Reflection;
using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static Color GetColor(this string text)
        {
            text = text.Replace("\"", string.Empty);
            if (text[0] == '#')
            {
                var value = Convert.ToUInt32(text.Substring(1), 16);
                switch (text.Length)
                {
                    case 9:
                        return Color.FromArgb((byte)(value >> 24), (byte)(value >> 16 & 255U), (byte)(value >> 8 & 255U), (byte)(value & 255U));
                    case 7:
                        return Color.FromArgb(byte.MaxValue, (byte)(value >> 16 & 255U), (byte)(value >> 8 & 255U), (byte)(value & 255U));
                    case 5:
                        var value0 = (byte)(value >> 12);
                        value0 = (byte)(value0 << 4 | value0);
                        var value1 = (byte)(value >> 8 & 15U);
                        value1 = (byte)(value1 << 4 | value1);
                        var value2 = (byte)(value >> 4 & 15U);
                        value2 = (byte)(value2 << 4 | value2);
                        var value3 = (byte)(value & 15U);
                        value3 = (byte)(value3 << 4 | value3);
                        return Color.FromArgb(value0, value1, value2, value3);
                    case 4:
                        value0 = (byte)(value >> 8 & 15U);
                        value0 = (byte)(value0 << 4 | value0);
                        value1 = (byte)(value >> 4 & 15U);
                        value1 = (byte)(value1 << 4 | value1);
                        value2 = (byte)(value & 15U);
                        value2 = (byte)(value2 << 4 | value2);
                        return Color.FromArgb(byte.MaxValue, value0, value1, value2);
                    default:
                        return default;
                }
            }
            else
            {
                var value = typeof(Colors).GetTypeInfo().DeclaredProperties.SingleOrDefault(property => property.Name.EqualsCaseless(text))?.GetValue(null);
                return value != null ? (Color)value : default;
            }
        }

        public static uint GetColor(uint value0, uint value1, uint value2, uint value3) => 16777216U * value3 + 65536U * value2 + 256U * value1 + value0;

        public static Brush GetTitlePaint(string titleColor) => titleColor switch
        {
            "level0" => BaseUI.Instance.LevelPaints[0],
            "level1" => BaseUI.Instance.LevelPaints[1],
            "level2" => BaseUI.Instance.LevelPaints[2],
            "level3" => BaseUI.Instance.LevelPaints[3],
            "level4" => BaseUI.Instance.LevelPaints[4],
            "level5" => BaseUI.Instance.LevelPaints[5],
            "titleLV2000" => DrawingSystem.Instance.GetDefaultPaint(titleColor),
            _ => DrawingSystem.Instance.GetDefaultPaint(titleColor.GetColor())
        };

        public static Color GetTitleColor(string titleColor) => titleColor switch
        {
            "level0" => BaseUI.Instance.D2DLevelColors[0],
            "level1" => BaseUI.Instance.D2DLevelColors[1],
            "level2" => BaseUI.Instance.D2DLevelColors[2],
            "level3" => BaseUI.Instance.D2DLevelColors[3],
            "level4" => BaseUI.Instance.D2DLevelColors[4],
            "level5" => BaseUI.Instance.D2DLevelColors[5],
            "titleLV2000" => "#FFDA70D6".GetColor(),
            _ => titleColor.GetColor()
        };

        public static Color ModifyColor(System.Windows.Media.Color wasColor) => Color.FromArgb(wasColor.A, wasColor.R, wasColor.G, wasColor.B);

        public static System.Windows.Media.Color ModifyColor(Color wasColor) => System.Windows.Media.Color.FromArgb(wasColor.A, wasColor.R, wasColor.G, wasColor.B);
    }
}
