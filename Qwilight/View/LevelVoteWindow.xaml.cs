using Qwilight.ViewModel;

namespace Qwilight.View
{
    public sealed partial class LevelVoteWindow
    {
        public LevelVoteWindow()
        {
            InitializeComponent();
            EdgeView0.NavigationStarting += (sender, e) => (DataContext as LevelVoteViewModel).OnEdgeView0Loading(true);
            EdgeView0.NavigationCompleted += (sender, e) => (DataContext as LevelVoteViewModel).OnEdgeView0Loading(false);
            EdgeView1.NavigationStarting += (sender, e) => (DataContext as LevelVoteViewModel).OnEdgeView1Loading(true);
            EdgeView1.NavigationCompleted += (sender, e) => (DataContext as LevelVoteViewModel).OnEdgeView1Loading(false);
        }
    }
}