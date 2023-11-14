using Microsoft.Graphics.Canvas;

namespace Qwilight.PaintComponent
{
    public abstract class BasePaintComponent
    {
        double _framerate;
        double _millis;

        public int DrawingPaintFrame { get; set; }

        public int PaintFrame { get; set; }

        public double PaintPosition0 { get; set; }

        public double PaintPosition1 { get; set; }

        public double PaintLength { get; set; }

        public double PaintHeight { get; set; }

        public BasePaintComponent(int frame, double framerate)
        {
            DrawingPaintFrame = 0;
            PaintFrame = frame;
            _framerate = 1000.0 / framerate;
        }

        public bool IsPaintAsToo(double millisLoopUnit)
        {
            _millis += millisLoopUnit;
            while (_millis >= _framerate)
            {
                _millis -= _framerate;
                if (DrawingPaintFrame + 1 < PaintFrame)
                {
                    ++DrawingPaintFrame;
                }
                else
                {
                    return true;
                }
            }
            return false;
        }

        public abstract void Paint(ref Bound r, CanvasDrawingSession targetSession, float faint, CanvasComposite drawingComposition);
    }
}