using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class MultiplyModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => value switch
        {
            double valueFloat64 => valueFloat64 * (double)parameter,
            int valueInt => valueInt * (double)parameter,
            _ => default,
        };

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => null;
    }
}