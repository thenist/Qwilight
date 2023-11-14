using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class NotifyXaml
    {
        public NotifyXaml() => InitializeComponent();

        void OnPointLower(object sender, MouseButtonEventArgs e) => (DataContext as NotifyXamlViewModel).OnPointLower(sender, e);
    }
}
