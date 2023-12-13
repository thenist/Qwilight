using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Messaging;
using Qwilight.System16.MSG;
using Qwilight.ViewModel;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace Qwilight.System16.View
{
    [INotifyPropertyChanged]
    public sealed partial class System16View : IAudioContainer, IAudioHandler
    {
        public sealed class FallingItem
        {
            public Bound Area;

            public double AreaX { get; set; }

            public double AreaY { get; set; }

            public RotateTransform Transform { get; } = new RotateTransform();

            public FallingItem(double areaLength, double areaHeight)
            {
                var randomArea = 0.5 + 0.5 * Random.Shared.NextDouble();
                Area.Length = randomArea * areaLength;
                Area.Height = randomArea * areaHeight;
                Area.Position0 = ViewModels.Instance.MainValue.DefaultLength * Random.Shared.NextDouble();
                Area.Position1 = -Area.Height;
                AreaX = 2.0 * Random.Shared.NextDouble() - 1.0;
                AreaY = 0.5 + 0.5 * Random.Shared.NextDouble();
                Transform.Angle = 360.0 * Random.Shared.NextDouble();
            }

            public bool Move()
            {
                Area.Position0 += AreaX;
                Area.Position1 += AreaY;
                Transform.Angle += 1.0;
                Transform.CenterX = Area.Position0 + Area.Length / 2;
                Transform.CenterY = Area.Position1 + Area.Height / 2;
                var mainViewModel = ViewModels.Instance.MainValue;
                return mainViewModel.DefaultLength <= Area.Position0 || Area.Position0 <= Area.Length || mainViewModel.DefaultHeight <= Area.Position1;
            }
        }

        readonly DrawingGroup _target = new();
        readonly List<FallingItem> _fallingItems = new(64);

        public ImageSource Falling { get; set; }

        public System16View()
        {
            InitializeComponent();

            if (System16Components.Is1221)
            {
                Falling = new BitmapImage(new Uri(Path.Combine(QwilightComponent.AssetsEntryPath, "System 16", "Drawing", "1221.png"), UriKind.Relative));
                Falling.Freeze();
            }
            else if (System16Components.Is1225)
            {
                Falling = new BitmapImage(new Uri(Path.Combine(QwilightComponent.AssetsEntryPath, "System 16", "Drawing", "1225.png"), UriKind.Relative));
                Falling.Freeze();
            }
            for (var i = _fallingItems.Capacity - 1; i >= 0; --i)
            {
                _fallingItems.Add(new(Falling.Width, Falling.Height));
            }

            StrongReferenceMessenger.Default.Register<WipeSystem16View>(this, (recipient, message) =>
            {
                if (System16Components.Is1221 || System16Components.Is1225)
                {
                    System16Components.Is1221 = false;
                    System16Components.Is1225 = false;
                    UIHandler.Instance.HandleParallel(() => ((recipient as FrameworkElement).Parent as Panel).Children.Remove(this));
                }
            });
        }

        void OnVisibilityModified(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                CompositionTarget.Rendering += OnPaint;
            }
            else
            {
                CompositionTarget.Rendering -= OnPaint;
            }
        }

        void OnPaint(object sender, EventArgs e)
        {
            using (var targetSession = _target.Open())
            {
                for (var i = _fallingItems.Count - 1; i >= 0; --i)
                {
                    var fallingItem = _fallingItems[i];
                    targetSession.PushTransform(fallingItem.Transform);
                    targetSession.DrawImage(Falling, fallingItem.Area);
                    targetSession.Pop();
                    if (fallingItem.Move())
                    {
                        _fallingItems.RemoveAt(i);
                        _fallingItems.Add(new(Falling.Width, Falling.Height));
                    }
                }
            }
        }

        protected override void OnRender(DrawingContext dc) => dc.DrawDrawing(_target);
    }
}