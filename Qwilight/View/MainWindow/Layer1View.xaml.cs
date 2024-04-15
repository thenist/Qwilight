using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.View
{
    public sealed partial class Layer1View
    {
        readonly VisualCollection _targets;
        readonly DrawingVisual _target = new();
        readonly Stopwatch _loopingHandler = Stopwatch.StartNew();
        readonly string[] _textGCs = new string[QwilightComponent.HeapCount];
        double _distanceMillisMax = double.MinValue;
        double _frametime;
        double _frameCount;
        string _framerate = string.Empty;
        string _framerateLowest = string.Empty;
        string _textHeap = string.Empty;
        double _lastMillis;
        long _lastHeap;

        public Layer1View()
        {
            _targets = new(this);
            _targets.Add(_target);

            Array.Fill(_textGCs, string.Empty);

            InitializeComponent();
        }

        protected override int VisualChildrenCount => _targets.Count;

        protected override Visual GetVisualChild(int index) => _targets[index];

        void OnVisibilityModified(object sender, DependencyPropertyChangedEventArgs e)
        {
            if ((bool)e.NewValue)
            {
                WPF.Paint += OnPaint;
            }
            else
            {
                WPF.Paint -= OnPaint;
            }
        }

        void OnPaint(object sender, object e)
        {
            var millis = _loopingHandler.GetMillis();
            var distanceMillis = millis - _lastMillis;
            _lastMillis = millis;

            var allowFramerate = TelnetSystem.Instance.IsAvailable;
            using (var targetSession = _target.RenderOpen())
            {
                var mainViewModel = (DataContext as MainViewModel);

                var fadingValue = mainViewModel.FadingValue;
                var fadingStatus = fadingValue.Status;

                if (fadingStatus < 1.0)
                {
                    if (mainViewModel.IsNoteFileMode)
                    {
                        var noteFile = mainViewModel.EntryItemValue?.NoteFile;
                        var autoComputer = mainViewModel.AutoComputer;

                        foreach (var paintProperty in BaseUI.Instance.PaintProperties)
                        {
                            if (paintProperty?.Layer == 1)
                            {
                                paintProperty.Paint(targetSession, distanceMillis, noteFile, autoComputer);
                            }
                        }
                    }
                }

                if (fadingStatus > 0.0)
                {
                    BaseUI.Instance.FadingProperties[(int)mainViewModel.ModeValue]?[fadingValue.Layer].Paint(targetSession, fadingStatus);
                }

                if (allowFramerate)
                {
                    var r = new Bound();
                    r.SetPosition(Levels.StandardMarginFloat32, Levels.StandardMarginFloat32);

                    var textItem = PoolSystem.Instance.GetDefaultTextItem(_framerate, Levels.FontLevel0, Paints.Paint1);
                    targetSession.PaintText(textItem, ref r);
                    r.Position1 += (float)(textItem.Height + Levels.StandardMarginFloat32);

                    textItem = PoolSystem.Instance.GetDefaultTextItem(_framerateLowest, Levels.FontLevel0, Paints.Paint1);
                    targetSession.PaintText(textItem, ref r);
                    r.Position1 += (float)(textItem.Height + Levels.StandardMarginFloat32);

                    for (var i = 0; i < QwilightComponent.HeapCount; ++i)
                    {
                        textItem = PoolSystem.Instance.GetDefaultTextItem(_textGCs[i], Levels.FontLevel0, Paints.Paint1);
                        targetSession.PaintText(textItem, ref r);
                        r.Position1 += (float)(textItem.Height + Levels.StandardMarginFloat32);
                    }

                    textItem = PoolSystem.Instance.GetDefaultTextItem(_textHeap, Levels.FontLevel0, Paints.Paint1);
                    targetSession.PaintText(textItem, ref r);
                }
            }

            if (allowFramerate)
            {
                _distanceMillisMax = Math.Max(_distanceMillisMax, distanceMillis);
                ++_frameCount;
                _frametime += distanceMillis;
                if (_frametime >= 1000.0)
                {
                    _framerate = PoolSystem.Instance.GetValueText(Math.Round(1000.0 * _frameCount / _frametime), "0 frame/s");
                    _framerateLowest = PoolSystem.Instance.GetValueText(Math.Round(1000.0 / _distanceMillisMax), "0 frame/s (\\0％ Low)");
                    _frameCount = 0;
                    _frametime = 0.0;
                    _distanceMillisMax = 0.0;
                    for (var i = 0; i < QwilightComponent.HeapCount; ++i)
                    {
                        _textGCs[i] = PoolSystem.Instance.GetFormattedText("GC{0} {1}", PoolSystem.Instance.GetValueText(i, string.Empty), PoolSystem.Instance.GetValueText(GC.CollectionCount(i), string.Empty));
                    }
                    var valueHeap = GC.GetTotalMemory(false);
                    var distanceHeap = valueHeap - _lastHeap;
                    _textHeap = PoolSystem.Instance.GetFormattedText("Heap {0} ({1}{2}/s)", PoolSystem.Instance.GetFormattedUnitText(valueHeap), distanceHeap >= 0 ? "＋" : "－", PoolSystem.Instance.GetFormattedUnitText(Math.Abs(distanceHeap)));
                    _lastHeap = valueHeap;
                }
            }
        }
    }
}