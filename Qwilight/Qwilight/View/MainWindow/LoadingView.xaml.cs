using CommunityToolkit.Mvvm.Messaging;
using Qwilight.UIComponent;
using System.Windows;
using System.Windows.Media.Animation;

namespace Qwilight.View
{
    public sealed partial class LoadingView : IRecipient<ICC>
    {
        public LoadingView()
        {
            InitializeComponent();
            WeakReferenceMessenger.Default.Register<ICC>(this);
        }

        public void Receive(ICC message)
        {
            switch (message.IDValue)
            {
                case ICC.ID.FadingLoadingView:
                    HandlingUISystem.Instance.HandleParallel(() =>
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
                    });
                    break;
            }
        }
    }
}