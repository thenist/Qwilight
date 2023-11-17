using CommunityToolkit.Mvvm.Messaging;
using Qwilight.MSG;
using System.Windows;
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
                var fadingComputer = new Storyboard();
                fadingComputer.Children.Add(fadingElement);
                Storyboard.SetTarget(fadingElement, this);
                Storyboard.SetTargetProperty(fadingElement, new PropertyPath(OpacityProperty));
                fadingComputer.Completed += (sender, e) => Visibility = Visibility.Collapsed;
                fadingComputer.Begin();
            }));
        }
    }
}