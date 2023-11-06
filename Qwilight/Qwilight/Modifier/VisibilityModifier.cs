using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using System.Globalization;
using System.Windows;
using System.Windows.Data;
using System.Windows.Media;

namespace Qwilight.Modifier
{
    public sealed class VisibilityModifier : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            switch (value)
            {
                case bool valueBool when valueBool:
                case DefaultEntryItem defaultEntryItem when defaultEntryItem != null:
                case BaseNoteFile.NoteVariety noteVariety when noteVariety.Equals(parameter):
                case WwwLevelItem wwwLevelItemValue:
                case int valueInt when valueInt == (int)parameter:
                case string text when !string.IsNullOrEmpty(text):
                case AutoCompute autoComputer when autoComputer != null:
                case ImageSource drawing when drawing != null:
                case byte[] data when data != null:
                case string[] texts when texts != null:
                    return Visibility.Visible;
            }
            return Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => throw new NotImplementedException();
    }
}