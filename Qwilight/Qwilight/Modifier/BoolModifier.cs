using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class BoolModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (parameter)
            {
                case int valueInt:
                    switch (value)
                    {
                        case int valueIntValue when valueIntValue != valueInt:
                            return false;
                    }
                    break;
                case bool valueBool:
                    switch (value)
                    {
                        case bool valueBoolValue when !valueBoolValue || !valueBool:
                            return false;
                    }
                    break;
                case null:
                    switch (value)
                    {
                        case string textValue when string.IsNullOrEmpty(textValue):
                        case bool valueBoolValue when !valueBoolValue:
                        case null:
                            return false;
                    }
                    break;
            }
            return true;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}