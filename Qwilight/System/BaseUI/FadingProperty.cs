using Microsoft.Graphics.Canvas;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Windows.Media;
using Colors = Microsoft.UI.Colors;

namespace Qwilight
{
    public sealed class FadingProperty
    {
        public int Frame { get; init; }

        public double Framerate { get; init; }

        public double Millis { get; init; }

        public double DrawingStatus { get; set; }

        public HandledDrawingItem?[] HandledDrawingItems { get; set; }

        public void Paint(DrawingContext targetSession, double fadingStatus)
        {
            if (Frame > 0)
            {
                var r = new Bound();
                var mainViewModel = ViewModels.Instance.MainValue;
                var defaultLength = mainViewModel.DefaultLength;
                var defaultHeight = mainViewModel.DefaultHeight;
                if (DrawingStatus <= fadingStatus)
                {
                    var fadingViewDrawing = (mainViewModel.FadingViewComputer?.NoteDrawing ?? DrawingSystem.Instance.DefaultDrawing).DefaultDrawing;
                    if (fadingViewDrawing != null)
                    {
                        r.SetArea(defaultLength, defaultHeight);
                        targetSession.DrawRectangle(Paints.Paint0, null, r);
                        Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, fadingViewDrawing.Width, fadingViewDrawing.Height, 0.0, 0.0, defaultLength, defaultHeight);
                        targetSession.PaintDrawing(ref r, fadingViewDrawing);
                    }
                }
                r.SetArea(defaultLength, defaultHeight);
                targetSession.PaintDrawing(ref r, HandledDrawingItems[(int)Math.Floor(fadingStatus * (Frame - 1))]?.DefaultDrawing);
            }
        }

        public void Paint(CanvasDrawingSession targetSession, double fadingStatus)
        {
            if (Frame > 0)
            {
                var r = new Bound();
                var mainViewModel = ViewModels.Instance.MainValue;
                var defaultLength = mainViewModel.DefaultLength;
                var defaultHeight = mainViewModel.DefaultHeight;
                if (DrawingStatus <= fadingStatus)
                {
                    var hasContents = false;
                    var fadingViewComputer = mainViewModel.FadingViewComputer;
                    if (fadingViewComputer != null)
                    {
                        lock (fadingViewComputer.LoadedCSX)
                        {
                            var fadingViewDrawing = fadingViewComputer.NoteDrawing;
                            hasContents = fadingViewComputer.HasContents && fadingViewDrawing != null;
                            if (hasContents)
                            {
                                HandleDrawing(fadingViewDrawing);
                            }
                        }
                    }
                    if (!hasContents)
                    {
                        HandleDrawing(DrawingSystem.Instance.DefaultDrawing);
                    }

                    void HandleDrawing(HandledDrawingItem? fadingViewDrawing)
                    {
                        var drawing = fadingViewDrawing?.Drawing;
                        if (drawing.HasValue)
                        {
                            r.SetArea(defaultLength, defaultHeight);
                            targetSession.FillRectangle(r, Colors.Black);
                            var drawingBound = drawing.Value.DrawingBound;
                            Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, drawingBound.Length, drawingBound.Height, 0.0, 0.0, defaultLength, defaultHeight);
                            targetSession.PaintDrawing(ref r, drawing);
                        }
                    }
                }
                r.SetArea(defaultLength, defaultHeight);
                targetSession.PaintDrawing(ref r, HandledDrawingItems[(int)Math.Floor(fadingStatus * (Frame - 1))]?.Drawing);
            }
        }
    }
}