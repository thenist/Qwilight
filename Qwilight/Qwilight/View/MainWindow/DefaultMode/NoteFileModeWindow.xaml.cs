using Qwilight.Utilities;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class NoteFileModeWindow
    {
        public NoteFileModeWindow() => InitializeComponent();

        void OnViewItems(object sender, MouseButtonEventArgs e) => Utility.ViewItems(sender as FrameworkElement);
    }
}
