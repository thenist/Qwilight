using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class FavorHitPointsWindow
    {
        public FavorHitPointsWindow() => InitializeComponent();

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as FavorHitPointsViewModel).OnInputLower(e);
    }
}