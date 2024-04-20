namespace Qwilight.ViewModel
{
    public sealed class LevelVoteViewModel : BaseViewModel
    {
        public override double TargetLength => 0.9;

        Uri _www0;
        Uri _www1;
        bool _isWww0Visible;
        bool _isWww1Visible;
        bool _isEdgeView0Loading;
        bool _isEdgeView1Loading;

        public Uri Www0
        {
            get => _www0;

            set => SetProperty(ref _www0, value, nameof(Www0));
        }

        public Uri Www1
        {
            get => _www1;

            set => SetProperty(ref _www1, value, nameof(Www1));
        }

        public bool IsWww0Visible
        {
            get => _isWww0Visible;

            set => SetProperty(ref _isWww0Visible, value, nameof(IsWww0Visible));
        }

        public bool IsWww1Visible
        {
            get => _isWww1Visible;

            set => SetProperty(ref _isWww1Visible, value, nameof(IsWww1Visible));
        }

        public bool IsEdgeView0Loading
        {
            get => _isEdgeView0Loading;

            set => SetProperty(ref _isEdgeView0Loading, value, nameof(IsEdgeView0Loading));
        }

        public bool IsEdgeView1Loading
        {
            get => _isEdgeView1Loading;

            set => SetProperty(ref _isEdgeView1Loading, value, nameof(IsEdgeView1Loading));
        }

        public void OnEdgeView0Loading(bool isLoading) => IsEdgeView0Loading = isLoading;

        public void OnEdgeView1Loading(bool isLoading) => IsEdgeView1Loading = isLoading;

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            ViewModels.Instance.MainValue.HandleAutoComputer();
        }

        public override void OnOpened()
        {
            base.OnOpened();
            ViewModels.Instance.MainValue.CloseAutoComputer();
        }
    }
}