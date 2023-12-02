using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class AvatarWindow
    {
        public AvatarWindow() => InitializeComponent();

        void OnAvatarDrawing(object sender, MouseButtonEventArgs e) => _ = (DataContext as AvatarViewModel).OnAvatarDrawing();

        void OnViewItems(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Utility.ViewItems(sender as FrameworkElement);
            }
        }
    }
}