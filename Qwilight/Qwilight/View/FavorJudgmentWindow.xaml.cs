using Qwilight.ViewModel;
using System.Windows.Input;

namespace Qwilight.View
{
    public sealed partial class FavorJudgmentWindow
    {
        public FavorJudgmentWindow() => InitializeComponent();

        void OnInputLower(object sender, KeyEventArgs e) => (DataContext as FavorJudgmentViewModel).OnInputLower(e);
    }
}