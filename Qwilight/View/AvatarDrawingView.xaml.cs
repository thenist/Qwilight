using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace Qwilight.View
{
    [INotifyPropertyChanged]
    public partial class AvatarDrawingView
    {
        public static readonly DependencyProperty AvatarWwwValueProperty = DependencyProperty.Register(nameof(AvatarWwwValue), typeof(AvatarWww), typeof(AvatarDrawingView));

        public AvatarDrawingView() => InitializeComponent();

        public AvatarWww AvatarWwwValue
        {
            get => GetValue(AvatarWwwValueProperty) as AvatarWww;

            set => SetValue(AvatarWwwValueProperty, value);
        }

        public Thickness MarginValue => new Thickness(-EdgeX, -EdgeY, -EdgeX, -EdgeY);

        public double EdgeLength => double.IsNaN(Width) ? 0.0 : Width * Levels.EdgeMargin;

        public double EdgeHeight => double.IsNaN(Height) ? 0.0 : Height * Levels.EdgeMargin;

        public double EdgeX => double.IsNaN(Width) ? 0.0 : Width * Levels.EdgeXY;

        public double EdgeY => double.IsNaN(Height) ? 0.0 : Height * Levels.EdgeXY;

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            OnPropertyChanged(nameof(MarginValue));
            OnPropertyChanged(nameof(EdgeLength));
            OnPropertyChanged(nameof(EdgeHeight));
            OnPropertyChanged(nameof(EdgeX));
            OnPropertyChanged(nameof(EdgeY));
        }
    }
}
