using Qwilight.Utilities;
using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class UnitModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => Utility.FormatLength((long)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}