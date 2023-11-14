using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class AvatarEdgeWindow
    {
        public AvatarEdgeWindow() => InitializeComponent();

        void OnPointLower(object sender, MouseButtonEventArgs e) => (DataContext as AvatarEdgeViewModel).OnPointLower();
    }
}
