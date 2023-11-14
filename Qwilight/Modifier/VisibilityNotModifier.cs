using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class VisibilityNotModifier : IValueConverter
    {
        static readonly IValueConverter _visibilityModifier = QwilightComponent.GetBuiltInData<IValueConverter>("VisibilityModifier");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return _visibilityModifier.Convert(value, targetType, parameter, culture).Equals(Visibility.Visible) ? Visibility.Collapsed : Visibility.Visible;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}