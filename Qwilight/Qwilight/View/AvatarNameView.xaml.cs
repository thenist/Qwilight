using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.View
{
    [INotifyPropertyChanged]
    public partial class AvatarNameView
    {
        public static readonly DependencyProperty AvatarWwwValueProperty = DependencyProperty.Register(nameof(AvatarWwwValue), typeof(AvatarWww), typeof(AvatarNameView));
        public static readonly DependencyProperty AvatarNameProperty = DependencyProperty.Register(nameof(AvatarName), typeof(string), typeof(AvatarNameView));
        public static readonly DependencyProperty FontLengthProperty = DependencyProperty.Register(nameof(FontLength), typeof(double), typeof(AvatarNameView), new FrameworkPropertyMetadata(Levels.FontLevel0));
        public static readonly DependencyProperty TextPaintProperty = DependencyProperty.Register(nameof(TextPaint), typeof(Brush), typeof(AvatarNameView), new FrameworkPropertyMetadata(Paints.Paint4));
        public static readonly DependencyProperty HeavyTextProperty = DependencyProperty.Register(nameof(HeavyText), typeof(FontWeight), typeof(AvatarNameView), new FrameworkPropertyMetadata(default));

        public AvatarNameView() => InitializeComponent();

        public AvatarWww AvatarWwwValue
        {
            get => GetValue(AvatarWwwValueProperty) as AvatarWww;

            set => SetValue(AvatarWwwValueProperty, value);
        }

        public string AvatarName
        {
            get => GetValue(AvatarNameProperty) as string;

            set => SetValue(AvatarNameProperty, value);
        }

        public double FontLength
        {
            get => (double)GetValue(FontLengthProperty);

            set => SetValue(FontLengthProperty, value);
        }

        public Brush TextPaint
        {
            get => GetValue(TextPaintProperty) as Brush;

            set => SetValue(TextPaintProperty, value);
        }

        public FontWeight HeavyText
        {
            get => (FontWeight)GetValue(HeavyTextProperty);

            set => SetValue(HeavyTextProperty, value);
        }
    }
}
