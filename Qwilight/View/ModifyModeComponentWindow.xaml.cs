using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class ModifyModeComponentWindow
    {
        public ModifyModeComponentWindow() => InitializeComponent();

        void OnPointLower(object sender, MouseButtonEventArgs e) => (DataContext as ModifyModeComponentViewModel).OnPointLower();
    }
}