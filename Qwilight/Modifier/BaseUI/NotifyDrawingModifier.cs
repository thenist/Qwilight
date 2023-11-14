using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class NotifyDrawingModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => BaseUI.Instance.NotifyDrawings[(int)value]?.DefaultDrawing;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}