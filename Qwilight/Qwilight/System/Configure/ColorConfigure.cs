using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.UI;
using System.Text.Json.Serialization;
using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight
{
    public partial class Configure
    {
        Color _meterNoteColor;
        Color _limiterColor;
        Color _audioVisualizerMainColor;
        Color _audioVisualizerInputColor;
        Color _inputNoteCountViewColor;
        Color _autoableInputNoteCountViewColor;

        public Color InputNoteCountViewColor
        {
            get => _inputNoteCountViewColor;

            set
            {
                if (SetProperty(ref _inputNoteCountViewColor, value, nameof(InputNoteCountViewColor)))
                {
                    OnSetInputNoteCountViewColor();
                }
            }
        }

        void OnSetInputNoteCountViewColor()
        {
            if (_isLoaded)
            {
                InputNoteCountViewPaint = DrawingSystem.Instance.GetDefaultPaint(InputNoteCountViewColor);
                OnPropertyChanged(nameof(InputNoteCountViewPaint));
            }
        }

        [JsonIgnore]
        public Brush InputNoteCountViewPaint { get; set; }

        public Color AutoableInputNoteCountViewColor
        {
            get => _autoableInputNoteCountViewColor;

            set
            {
                if (SetProperty(ref _autoableInputNoteCountViewColor, value, nameof(AutoableInputNoteCountViewColor)))
                {
                    OnSetAutoableInputNoteCountViewColor();
                }
            }
        }

        public void OnSetAutoableInputNoteCountViewColor()
        {
            if (_isLoaded)
            {
                AutoableInputNoteCountViewPaint = DrawingSystem.Instance.GetDefaultPaint(AutoableInputNoteCountViewColor);
                OnPropertyChanged(nameof(AutoableInputNoteCountViewPaint));
            }
        }

        [JsonIgnore]
        public Brush AutoableInputNoteCountViewPaint { get; set; }

        public Color AudioVisualizerMainColor
        {
            get => _audioVisualizerMainColor;

            set
            {
                if (SetProperty(ref _audioVisualizerMainColor, value, nameof(AudioVisualizerMainColor)))
                {
                    OnSetAudioVisualizerMainColor();
                }
            }
        }

        void OnSetAudioVisualizerMainColor()
        {
            if (_isLoaded)
            {
                for (var i = AudioVisualizerMainPaints.Length - 1; i >= 0; --i)
                {
                    AudioVisualizerMainPaints[i] = DrawingSystem.Instance.GetDefaultPaint(AudioVisualizerMainColor, i);
                }
                OnPropertyChanged(nameof(AudioVisualizerMainPaints));
                SetFaintPaints(AudioVisualizerMainColor, DrawingSystem.Instance.AudioVisualizerMainPaints);
            }
        }

        [JsonIgnore]
        public Brush[] AudioVisualizerMainPaints { get; } = new Brush[101];

        public Color AudioVisualizerInputColor
        {
            get => _audioVisualizerInputColor;

            set
            {
                if (SetProperty(ref _audioVisualizerInputColor, value, nameof(AudioVisualizerInputColor)))
                {
                    OnSetAudioVisualizerInputColor();
                }
            }
        }

        void OnSetAudioVisualizerInputColor()
        {
            if (_isLoaded)
            {
                for (var i = AudioVisualizerInputPaints.Length - 1; i >= 0; --i)
                {
                    AudioVisualizerInputPaints[i] = DrawingSystem.Instance.GetDefaultPaint(AudioVisualizerInputColor, i);
                }
                OnPropertyChanged(nameof(AudioVisualizerInputPaints));
                SetFaintPaints(AudioVisualizerInputColor, DrawingSystem.Instance.AudioVisualizerInputPaints);
            }
        }

        [JsonIgnore]
        public Brush[] AudioVisualizerInputPaints { get; } = new Brush[101];

        public Color MeterNoteColor
        {
            get => _meterNoteColor;

            set
            {
                if (SetProperty(ref _meterNoteColor, value, nameof(MeterNoteColor)))
                {
                    OnSetMeterNoteColor();
                }
            }
        }

        void OnSetMeterNoteColor()
        {
            if (_isLoaded)
            {
                MeterNotePaint = DrawingSystem.Instance.GetDefaultPaint(MeterNoteColor);
                OnPropertyChanged(nameof(MeterNotePaint));
                SetFaintPaints(MeterNoteColor, DrawingSystem.Instance.MeterNotePaints);
                DrawingSystem.Instance.MeterNoteAverageColor = (uint)(16777216 * MeterNoteColor.B + 65536 * MeterNoteColor.G + 256 * MeterNoteColor.R + MeterNoteColor.A);
            }
        }

        [JsonIgnore]
        public Brush MeterNotePaint { get; set; }

        public Color LimiterColor
        {
            get => _limiterColor;

            set
            {
                if (SetProperty(ref _limiterColor, value, nameof(LimiterColor)))
                {
                    OnSetLimiterColor();
                }
            }
        }

        public void OnSetLimiterColor()
        {
            if (_isLoaded)
            {
                LimiterPaint = DrawingSystem.Instance.GetDefaultPaint(LimiterColor);
                OnPropertyChanged(nameof(LimiterPaint));
            }
        }

        [JsonIgnore]
        public Brush LimiterPaint { get; set; }

        void SetFaintPaints(Color valueColor, ICanvasBrush[] d2dPaints)
        {
            foreach (var d2dPaint in d2dPaints)
            {
                d2dPaint?.Dispose();
            }
            DrawingSystem.Instance.SetFaintPaints(null, d2dPaints, valueColor);
        }

        public void InitColors(int level)
        {
            if ((level & 1) == 1)
            {
                MeterNoteColor = Colors.White;
                LimiterColor = Colors.White;
                AudioVisualizerMainColor = Colors.LightGray;
                AudioVisualizerInputColor = Colors.DarkGray;
            }
            if ((level & 2) == 2)
            {
                InputNoteCountViewColor = new Color
                {
                    R = 255,
                    G = 255,
                    B = 255,
                    A = 127
                };
                AutoableInputNoteCountViewColor = new Color
                {
                    R = 255,
                    A = 127
                };
            }
        }
    }
}
