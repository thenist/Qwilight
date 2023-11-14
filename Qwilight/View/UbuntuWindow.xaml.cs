using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class UbuntuWindow
    {
        public UbuntuWindow() => InitializeComponent();

        void OnUbuntuView(object sender, KeyEventArgs e) => (DataContext as UbuntuViewModel).OnUbuntuView(e);
    }
}