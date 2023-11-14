using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class LevelPaintModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => BaseUI.Instance.LevelPaints[(int)value];

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}