using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class FaintModeComponentModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)value == (int)parameter ? 0.125 : 1.0;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}