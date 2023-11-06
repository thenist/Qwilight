using Qwilight.ViewModel;
using System.Windows;
using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight.View
{
    public partial class ColorInput
    {
        public static readonly DependencyProperty ColorProperty = DependencyProperty.Register(nameof(ColorValue), typeof(Color), typeof(ColorInput));
        public static readonly DependencyProperty PaintProperty = DependencyProperty.Register(nameof(PaintValue), typeof(Brush), typeof(ColorInput));

        public Color ColorValue
        {
            get => (Color)GetValue(ColorProperty);

            set => SetValue(ColorProperty, value);
        }

        public Brush PaintValue
        {
            get => GetValue(PaintProperty) as Brush;

            set => SetValue(PaintProperty, value);
        }

        public ColorInput() => InitializeComponent();

        protected override void OnClick()
        {
            ViewModels.Instance.ColorValue.SetColor(ColorValue);
            ViewModels.Instance.ColorValue.OnLazyCollasped = value => HandlingUISystem.Instance.HandleParallel(() => ColorValue = value);
            ViewModels.Instance.ColorValue.Open();
        }
    }
}
