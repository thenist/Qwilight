using Qwilight.ViewModel;
using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class ModeComponentInputModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => ViewModels.Instance.ModifyModeComponentValue.ModifyModeComponentItems[(int)parameter]?.Single(modeComponentItem => modeComponentItem.Value == (int)value)?.OnConfigure;

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}