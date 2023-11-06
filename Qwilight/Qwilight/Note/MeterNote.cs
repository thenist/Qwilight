using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.Note
{
    public sealed class MeterNote : BaseNote
    {
        const double MeterNoteHeight = 1.0;

        readonly string _meter;

        public override bool HasContents => false;

        public override bool HasStand => false;

        public override bool HasInput => false;

        public override int Layer { get; set; }

        public override int LogicalLayer => 3;

        public MeterNote(double logicalY, double wait, int meter) : base(logicalY, wait)
        {
            _meter = meter.ToString();
        }

        public override bool IsVisible(DefaultCompute defaultComputer)
        {
            return GetY(defaultComputer, GetMultiplierAsNoteMobility(defaultComputer.ModeComponentValue, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue)) + MeterNoteHeight >= 0.0;
        }

        public override bool IsVisibleHalf(DefaultCompute defaultComputer)
        {
            var yHeight = GetY(defaultComputer, GetMultiplierAsNoteMobility(defaultComputer.ModeComponentValue, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue)) + MeterNoteHeight;
            return yHeight >= 0.0 && yHeight <= defaultComputer.DrawingComponentValue.judgmentMainPosition / 2;
        }

        public override void MoveInputMillis(double millisLoopUnit, DefaultCompute defaultComputer)
        {
        }

        public override void SetLayer(DefaultCompute defaultComputer)
        {
        }

        public override JudgedNoteData? Judge(int input, double wait, ModeComponent modeComponentValue, double judgmentStage, int judgmentModeDate, int judgmentMapDate, int longNoteAssistDate, int trapNoteJudgmentDate, bool isAutoLongNote) => default;

        public override JudgedNoteData? AutoJudge(double wait)
        {
            if (Wait <= wait)
            {
                Judged = Component.Judged.Highest;
                return new JudgedNoteData
                {
                    IDValue = JudgedNoteData.ID.HandleMeter
                };
            }
            return default;
        }

        public override void Paint(CanvasDrawingSession targetSession, bool isValidNetDrawings, DefaultCompute defaultComputer, ref Bound r)
        {
            var drawingComponentValue = defaultComputer.DrawingComponentValue;
            var p1BuiltLength = (float)drawingComponentValue.p1BuiltLength;
            r.Set(drawingComponentValue.mainPosition, GetY(defaultComputer, GetMultiplierAsNoteMobility(defaultComputer.ModeComponentValue, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue)) - MeterNoteHeight / 2, p1BuiltLength, MeterNoteHeight);
            if (r.Position1 + r.Height > 0.0)
            {
                var faint = (int)(100 * GetFaint(defaultComputer.ModeComponentValue, drawingComponentValue.judgmentMainPosition, defaultComputer.FaintCosine));
                var meterNotePaint = DrawingSystem.Instance.MeterNotePaints[faint];
                var has2P = defaultComputer.Has2P;
                p1BuiltLength += drawingComponentValue.p2Position;
                defaultComputer.NewNetDrawing(isValidNetDrawings, Event.Types.NetDrawing.Types.Variety.Meter, DrawingSystem.Instance.MeterNoteAverageColor, 0.0, r.Position1, 0.0, 0.0);
                targetSession.FillRectangle(r, meterNotePaint);
                if (has2P)
                {
                    r.Position0 += p1BuiltLength;
                    targetSession.FillRectangle(r, meterNotePaint);
                }

                var isMeterVisible = defaultComputer.IsMeterVisible;
                if (isMeterVisible)
                {
                    r.Position0 -= p1BuiltLength;

                    var faintClearPaint = DrawingSystem.Instance.FaintClearedPaints[faint];
                    var textItem = PoolSystem.Instance.GetTextItem(_meter, DrawingSystem.Instance.MeterFont);
                    r.Position0 += Levels.StandardMargin;
                    r.Position1 -= Levels.StandardMargin + textItem.LayoutBounds.Height;
                    targetSession.PaintText(textItem, ref r, faintClearPaint);

                    if (has2P)
                    {
                        r.Position0 += p1BuiltLength;
                        targetSession.PaintText(textItem, ref r, faintClearPaint);
                    }
                }
            }
        }
    }
}