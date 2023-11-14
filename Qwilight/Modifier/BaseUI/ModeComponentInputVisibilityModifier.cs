using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class ModeComponentInputVisibilityModifier : IValueConverter
    {
        readonly IValueConverter _modeComponentInputModifier = QwilightComponent.GetBuiltInData<IValueConverter>("ModeComponentInputModifier");

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => _modeComponentInputModifier.Convert(value, targetType, parameter, culture) != null ? Visibility.Visible : Visibility.Collapsed;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}