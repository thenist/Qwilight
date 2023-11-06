using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Igniter.Modifier
{
    public sealed class VisibilityModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case bool valueBool when valueBool:
                case string text when !string.IsNullOrEmpty(text):
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}