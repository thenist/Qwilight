using CommunityToolkit.Mvvm.Messaging;
using Qwilight.UIComponent;
using Qwilight.ViewModel;

namespace Qwilight.View
{
    public sealed partial class VoteWindow : IRecipient<ICC>
    {
        public VoteWindow()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
            EdgeView.NavigationStarting += (sender, e) => (DataContext as VoteViewModel).OnEdgeViewLoading(true);
            EdgeView.NavigationCompleted += (sender, e) => (DataContext as VoteViewModel).OnEdgeViewLoading(false);
        }

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.SetVoteWindowEdgeView:
                    EdgeView.Source = new Uri(message.Contents as string);
                    break;
            }
        }
    }
}