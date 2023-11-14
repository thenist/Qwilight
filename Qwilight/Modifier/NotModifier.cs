using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class NotModifier : IValueConverter
    {
        static readonly IValueConverter _boolModifier = QwilightComponent.GetBuiltInData<IValueConverter>("BoolModifier");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => !(bool)_boolModifier.Convert(value, targetType, parameter, culture);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}