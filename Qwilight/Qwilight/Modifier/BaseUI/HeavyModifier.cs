using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class HeavyModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? FontWeights.Bold : FontWeights.Normal;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}