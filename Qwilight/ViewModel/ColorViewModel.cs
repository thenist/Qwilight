using System.Windows;
using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight.ViewModel
{
    public sealed class ColorViewModel : BaseViewModel
    {
        readonly byte[] _valueColors = new byte[4];

        public override double TargetLength => 0.3;

        public override double TargetHeight => 0.2;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public Action<Color> OnLazyCollapsed { get; set; }

        public void SetColor(Color value)
        {
            Color0 = value.R;
            Color1 = value.G;
            Color2 = value.B;
            Color3 = value.A;
        }

        public byte Color0
        {
            get => _valueColors[0];

            set
            {
                if (SetProperty(ref _valueColors[0], value, nameof(Color0)))
                {
                    OnPropertyChanged(nameof(PaintValue));
                }
            }
        }

        public byte Color1
        {
            get => _valueColors[1];

            set
            {
                if (SetProperty(ref _valueColors[1], value, nameof(Color1)))
                {
                    OnPropertyChanged(nameof(PaintValue));
                }
            }
        }

        public byte Color2
        {
            get => _valueColors[2];

            set
            {
                if (SetProperty(ref _valueColors[2], value, nameof(Color2)))
                {
                    OnPropertyChanged(nameof(PaintValue));
                }
            }
        }

        public byte Color3
        {
            get => _valueColors[3];

            set
            {
                if (SetProperty(ref _valueColors[3], value, nameof(Color3)))
                {
                    OnPropertyChanged(nameof(PaintValue));
                }
            }
        }

        public Brush PaintValue => DrawingSystem.Instance.GetDefaultPaint(new Color
        {
            R = Color0,
            G = Color1,
            B = Color2,
            A = Color3,
        });

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            OnLazyCollapsed(new Color
            {
                R = Color0,
                G = Color1,
                B = Color2,
                A = Color3
            });
        }
    }
}