using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class InputModeDrawingModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => BaseUI.Instance.InputModeDrawings[(int)value]?.DefaultDrawing;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}