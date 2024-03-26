using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.PaintComponent
{
    public sealed class JudgmentPaint : BasePaintComponent
    {
        public const int Band1 = 9;
        public const int Last = 10;
        public const int Yell1 = 11;

        readonly Component.Judged _judged;
        readonly int _judgmentSystem;

        public JudgmentPaint(DefaultCompute defaultComputer, Component.Judged judged, bool isIn2P, int judgmentSystem, double judgmentPosition0, double judgmentPosition1, int frame, double framerate, double paintLength, double paintHeight) : base(frame, framerate)
        {
            var drawingComponentValue = defaultComputer.DrawingComponentValue;
            PaintPosition0 = judgmentPosition0 + (isIn2P ? drawingComponentValue.p1BuiltLength + drawingComponentValue.p2Position : 0.0);
            PaintPosition1 = judgmentPosition1;
            PaintLength = paintLength;
            PaintHeight = paintHeight;
            _judged = judged;
            _judgmentSystem = judgmentSystem;
        }

        public override void Paint(ref Bound r, CanvasDrawingSession targetSession, float faint, CanvasComposite drawingComposition)
        {
            switch (_judgmentSystem)
            {
                case 0:
                    r.Set(PaintPosition0, PaintPosition1, PaintLength, PaintHeight);
                    break;
                case 1:
                    r.Set(PaintPosition0 - PaintLength / 2, PaintPosition1, PaintLength, PaintHeight);
                    break;
                case 2:
                    r.Set(PaintPosition0 - PaintLength, PaintPosition1, PaintLength, PaintHeight);
                    break;
            }
            targetSession.PaintDrawing(ref r, UI.Instance.JudgmentDrawings[(int)_judged][DrawingPaintFrame], faint, drawingComposition);
        }
    }
}