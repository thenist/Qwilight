using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.PaintComponent
{
    public sealed class HitLongNotePaint : BasePaintComponent
    {
        readonly Component.InputMode _inputMode;
        readonly int _input;

        public HitLongNotePaint(DefaultCompute defaultComputer, int input, int frame, double framerate) : base(frame, framerate)
        {
            _inputMode = defaultComputer.InputMode;
            var drawingComponentValue = defaultComputer.DrawingComponentValue;
            PaintPosition0 = defaultComputer.GetPosition(input) + drawingComponentValue.DrawingNoteLengthMap[input] / 2 + drawingComponentValue.hitLongNotePaintPosition0s[input];
            PaintPosition1 = defaultComputer.DrawingComponentValue.judgmentMainPosition + drawingComponentValue.hitLongNotePaintPosition1s[input];
            PaintLength = drawingComponentValue.hitLongNotePaintLengths[input];
            PaintHeight = drawingComponentValue.hitLongNotePaintHeights[input];
            _input = input;
        }

        public override void Paint(ref Bound r, CanvasDrawingSession targetSession, float faint, CanvasComposite drawingComposition)
        {
            r.Set(PaintPosition0, PaintPosition1, PaintLength, PaintHeight);
            targetSession.PaintDrawing(ref r, UI.Instance.LongNoteHitDrawings[(int)_inputMode][_input][DrawingPaintFrame], faint, drawingComposition);
        }
    }
}