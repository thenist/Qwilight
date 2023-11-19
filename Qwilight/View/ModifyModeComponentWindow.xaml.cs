using Qwilight.UIComponent;
using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class ModifyModeComponentWindow
    {
        public ModifyModeComponentWindow() => InitializeComponent();

        void OnPointLower(object sender, MouseButtonEventArgs e) => (DataContext as ModifyModeComponentViewModel).OnPointLower();

        void OnVConfigure(object sender, RoutedEventArgs e)
        {
            e.Handled = true;
            ((sender as FrameworkElement).DataContext as ModifyModeComponentItem).OnVConfigure();
        }
    }
}