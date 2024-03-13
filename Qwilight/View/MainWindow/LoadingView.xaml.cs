using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Animation;

namespace Qwilight.View
{
    public sealed partial class LoadingView
    {
        public LoadingView()
        {
            InitializeComponent();
            StrongReferenceMessenger.Default.Register<FadeLoadingView>(this, (recipient, message) => UIHandler.Instance.HandleParallel(() =>
            {
                var fadingElement = new DoubleAnimation
                {
                    To = 0.0
                };
                var fadingViewComputer = new Storyboard();
                fadingViewComputer.Children.Add(fadingElement);
                Storyboard.SetTarget(fadingElement, recipient as DependencyObject);
                Storyboard.SetTargetProperty(fadingElement, new(OpacityProperty));
                fadingViewComputer.Completed += (sender, e) => (Parent as Panel).Children.Remove(this);
                fadingViewComputer.Begin();
            }));
        }
    }
}