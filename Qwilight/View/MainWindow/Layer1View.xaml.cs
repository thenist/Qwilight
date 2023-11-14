using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;
using System.Windows;
using System.Windows.Media;

namespace Qwilight.View
{
    public sealed partial class Layer1View
    {
        readonly DrawingGroup _target = new();
        readonly Stopwatch _loopingHandler = Stopwatch.StartNew();
        readonly string[] _textGCs = new string[QwilightComponent.HeapCount];
        double _distanceMillisMax = double.MinValue;
        double _frametime = 0.0;
        double _frameCount = 0;
        string _framerate = string.Empty;
        string _framerateLowest = string.Empty;
        string _textHeap = string.Empty;
        double _lastMillis = 0.0;
        long _lastHeap = 0L;
        double _distanceMillis = 0.0;

        public Layer1View()
        {
            Array.Fill(_textGCs, string.Empty);
            InitializeComponent();

            IsVisibleChanged += OnVisibilityModified;
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

        void OnPaint(object sender, object e)
        {
            var allowFramerate = TelnetSystem.Instance.IsAvailable;
            using (var targetSession = _target.Open())
            {
                var mainViewModel = (DataContext as MainViewModel);
                var defaultLength = mainViewModel.DefaultLength;
                var r = new Bound();

                var fadingValue = mainViewModel.FadingValue;
                var fadingStatus = fadingValue.Status;

                if (fadingStatus < 1.0)
                {
                    if (mainViewModel.IsNoteFileMode)
                    {
                        var noteFile = mainViewModel.EntryItemValue?.NoteFile;
                        var autoComputer = mainViewModel.AutoComputer;

                        foreach (var paintPropertyValue in BaseUI.Instance.PaintPropertyValues)
                        {
                            if (paintPropertyValue?.Layer == 1)
                            {
                                paintPropertyValue.Paint(targetSession, noteFile, autoComputer, _distanceMillis);
                            }
                        }
                    }
                }

                if (fadingStatus > 0.0)
                {
                    var fadingComputer = mainViewModel.FadingViewComputer;
                    var fadingPropertyValue = BaseUI.Instance.FadingPropertyValues[(int)mainViewModel.ModeValue]?[fadingValue.Layer];
                    var fadingPropertyFrame = fadingPropertyValue?.Frame ?? 0;
                    if (fadingPropertyFrame > 0)
                    {
                        var defaultHeight = mainViewModel.DefaultHeight;
                        if (fadingPropertyValue.DrawingStatus <= fadingStatus)
                        {
                            var fadingViewDrawing = (fadingComputer?.NoteHandledDrawingItem ?? DrawingSystem.Instance.DefaultDrawing).DefaultDrawing;
                            if (fadingViewDrawing != null)
                            {
                                r.SetArea(defaultLength, defaultHeight);
                                targetSession.DrawRectangle(Paints.Paint0, null, r);
                                Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, fadingViewDrawing.Width, fadingViewDrawing.Height, 0.0, 0.0, defaultLength, defaultHeight);
                                targetSession.PaintDrawing(ref r, fadingViewDrawing);
                            }
                        }
                        r.SetArea(defaultLength, defaultHeight);
                        targetSession.PaintDrawing(ref r, fadingPropertyValue.HandledDrawingItems[(int)Math.Floor(fadingStatus * (fadingPropertyFrame - 1))]?.DefaultDrawing);
                    }
                }

                if (allowFramerate)
                {
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

            var millis = _loopingHandler.GetMillis();
            _distanceMillis = millis - _lastMillis;
            _lastMillis = millis;

            if (allowFramerate)
            {
                _distanceMillisMax = Math.Max(_distanceMillisMax, _distanceMillis);
                ++_frameCount;
                _frametime += _distanceMillis;
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

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            dc.DrawDrawing(_target);
        }
    }
}