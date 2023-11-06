using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class InputTextWindow
    {
        public InputTextWindow() => InitializeComponent();

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as InputTextViewModel).OnInputLower(e);
    }
}
