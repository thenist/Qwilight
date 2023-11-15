using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;

namespace Qwilight.View
{
    public sealed partial class VoteWindow
    {
        public VoteWindow()
        {
            InitializeComponent();

            EdgeView.NavigationStarting += (sender, e) => (DataContext as VoteViewModel).OnEdgeViewLoading(true);
            EdgeView.NavigationCompleted += (sender, e) => (DataContext as VoteViewModel).OnEdgeViewLoading(false);

            StrongReferenceMessenger.Default.Register<SetVoteWindowEdgeView>(this, (recipient, message) => EdgeView.Source = new Uri(message.Www));
        }
    }
}