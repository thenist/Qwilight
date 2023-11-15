using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace Qwilight.View
{
    public partial class WPFView
    {
        public WPFView()
        {
            InitializeComponent();

            StrongReferenceMessenger.Default.Register<PointZMaxView>(this, (recipient, message) => HandlingUISystem.Instance.HandleParallel(() =>
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
            }));
            StrongReferenceMessenger.Default.Register<GetWPFView>(this, (recipient, message) => message.Reply(this));
        }
    }
}
