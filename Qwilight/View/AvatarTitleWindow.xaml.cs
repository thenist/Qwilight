using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public partial class AvatarTitleWindow
    {
        public AvatarTitleWindow() => InitializeComponent();

        void OnPointLower(object sender, MouseButtonEventArgs e) => (DataContext as AvatarTitleViewModel).OnPointLower();
    }
}
