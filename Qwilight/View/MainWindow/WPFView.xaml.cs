﻿using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using Qwilight.System16;
using Qwilight.System16.View;
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

            if (System16Components.Is1221 || System16Components.Is1225)
            {
                MainView.Children.Insert(3, new System16View());
            }

            StrongReferenceMessenger.Default.Register<PointZMaxView>(this, (recipient, message) => UIHandler.Instance.HandleParallel(() =>
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
