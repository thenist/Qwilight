using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class AvatarWindow
    {
        public AvatarWindow() => InitializeComponent();

        void OnAvatarDrawing(object sender, MouseButtonEventArgs e) => _ = (DataContext as AvatarViewModel).OnAvatarDrawing();
    }
}