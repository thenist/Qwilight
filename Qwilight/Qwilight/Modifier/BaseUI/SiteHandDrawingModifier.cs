using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class SiteHandDrawingModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (bool)value ? BaseUI.Instance.AvatarConfigureDrawings[1] : null;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}