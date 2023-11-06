using System.Globalization;
using System.Windows.Data;

namespace Qwilight.Modifier
{
    public sealed class FormatModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var format = parameter as string;
            switch (value)
            {
                case uint valueUInt:
                    return valueUInt.ToString(format);
                case int valueInt:
                    return valueInt.ToString(format);
                case float valueFloat32:
                    if (format.Contains("％"))
                    {
                        valueFloat32 *= 100;
                    }
                    return valueFloat32.ToString(format);
                case double valueFloat64:
                    if (format.Contains("％"))
                    {
                        valueFloat64 *= 100;
                    }
                    return valueFloat64.ToString(format);
                case DateTime date:
                    return date.ToString(format);
                case string text:
                    return string.Format(format, text);
            }
            return default;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}