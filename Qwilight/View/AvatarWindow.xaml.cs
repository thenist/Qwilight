using Qwilight.Utilities;
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
    }
}