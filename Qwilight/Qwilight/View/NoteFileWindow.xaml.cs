using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class NoteFileWindow
    {
        public NoteFileWindow() => InitializeComponent();

        void OnPointLower(object sender, MouseButtonEventArgs e) => (DataContext as NoteFileViewModel).OnPointLower();

        void OnViewItems(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Utility.ViewItems(sender as FrameworkElement);
            }
        }
    }
}