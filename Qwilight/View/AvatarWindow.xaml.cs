using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class AvatarWindow
    {
        public AvatarWindow() => InitializeComponent();

        void OnViewItems(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Utility.ViewItems(sender as FrameworkElement);
            }
        }

        void OnLevyNoteFile(object sender, MouseButtonEventArgs e)
        {
            ((sender as FrameworkElement).DataContext as AvatarViewModel.AvatarComputing).OnLevyNoteFile(e);
        }
    }
}