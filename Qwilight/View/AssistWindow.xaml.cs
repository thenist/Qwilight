using Qwilight.Utilities;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class AssistWindow
    {
        public AssistWindow() => InitializeComponent();

        void OnViewItems(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                Utility.ViewItems(sender as FrameworkElement);
            }
        }
    }
}