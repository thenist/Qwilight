using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class ModeComponentDrawingModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => BaseUI.Instance.ModeComponentDrawings[(int)parameter][(int)value]?.DefaultDrawing;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}