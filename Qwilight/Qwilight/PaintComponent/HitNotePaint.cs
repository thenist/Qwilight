using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.PaintComponent
{
    public sealed class HitNotePaint : BasePaintComponent
    {
        readonly Component.InputMode _inputMode;
        readonly int _input;

        public HitNotePaint(DefaultCompute defaultComputer, int input, int frame, double framerate) : base(frame, framerate)
        {
            _inputMode = defaultComputer.InputMode;
            var drawingComponentValue = defaultComputer.DrawingComponentValue;
            PaintPosition0 = defaultComputer.GetPosition(input) + drawingComponentValue.DrawingNoteLengthMap[input] / 2 + drawingComponentValue.hitNotePaintPosition0s[input];
            PaintPosition1 = defaultComputer.DrawingComponentValue.judgmentMainPosition + drawingComponentValue.hitNotePaintPosition1s[input];
            PaintLength = drawingComponentValue.hitNotePaintLengths[input];
            PaintHeight = drawingComponentValue.hitNotePaintHeights[input];
            _input = input;
        }

        public override void Paint(ref Bound r, CanvasDrawingSession targetSession, float faint, CanvasComposite drawingComposition)
        {
            r.Set(PaintPosition0, PaintPosition1, PaintLength, PaintHeight);
            targetSession.PaintDrawing(ref r, UI.Instance.NoteHitDrawings[(int)_inputMode][_input][DrawingPaintFrame], faint, drawingComposition);
        }
    }
}