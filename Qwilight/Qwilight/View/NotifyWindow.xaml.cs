using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class NotifyWindow
    {
        public NotifyWindow() => InitializeComponent();

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as NotifyViewModel).OnInputLower(e);

        void OnPointLower(object sender, MouseButtonEventArgs e) => (DataContext as NotifyViewModel).OnPointLower(e);
    }
}