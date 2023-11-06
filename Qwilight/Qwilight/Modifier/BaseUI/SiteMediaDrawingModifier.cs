using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class SiteMediaDrawingModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => BaseUI.Instance.SiteMediaDrawings[(bool)value ? 1 : 0];

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}