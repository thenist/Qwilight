using CommunityToolkit.Mvvm.Messaging;
using Qwilight.UIComponent;
using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Qwilight.View
{
    public partial class WPFView : IRecipient<ICC>
    {
        public WPFView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.GetWPFView:
                    (message.Contents as Action<IInputElement>)(this);
                    break;
                case ICC.ID.PointZMaxView:
                    HandlingUISystem.Instance.HandleParallel(() =>
                    {
                        var mainViewModel = DataContext as MainViewModel;
                        if (mainViewModel.IsWPFViewVisible)
                        {
                            var zMaxValue = int.MinValue;
                            UIElement zMaxView = null;
                            foreach (UIElement windowView in WindowViews.Children)
                            {
                                var zValue = Canvas.GetZIndex(windowView);
                                if (windowView.Visibility == Visibility.Visible && zMaxValue < zValue)
                                {
                                    zMaxValue = zValue;
                                    zMaxView = windowView;
                                }
                            }
                            if (zMaxView != null)
                            {
                                if (mainViewModel.HasPoint)
                                {
                                    zMaxView.Focus();
                                }
                            }
                            else
                            {
                                mainViewModel.PointEntryView();
                            }
                        }
                    });
                    break;
            }
        }
    }
}
