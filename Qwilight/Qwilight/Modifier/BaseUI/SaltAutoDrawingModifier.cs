using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class SaltAutoDrawingModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => BaseUI.Instance.SaltAutoDrawings[(bool)value ? 1 : 0];

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}