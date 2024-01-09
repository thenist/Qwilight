using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.Note
{
    public class TrapNote : InputNote
    {
        public const int TrapNoteContents = 4;

        readonly bool _isPowered;

        public override bool HasStand => false;

        public override int LogicalLayer => 4;

        public TrapNote(double logicalY, double wait, ICollection<AudioNote> audioNotes, int input, bool isPowered = true) : base(logicalY, wait, audioNotes, input)
        {
            _isPowered = isPowered;
        }

        public override JudgedNoteData? Judge(int input, double wait, ModeComponent modeComponent, double judgmentStage, Component.JudgmentModeDate judgmentModeDate, Component.JudgmentMapDate judgmentMapDate, Component.LongNoteAssistDate longNoteAssistDate, Component.TrapNoteJudgmentDate trapNoteJudgmentDate, bool isAutoLongNote)
        {
            switch (trapNoteJudgmentDate)
            {
                case Component.TrapNoteJudgmentDate._1_14_6:
                    if (_isPowered && input > 0)
                    {
                        var judgmentMeter = (wait - Wait) / modeComponent.AudioMultiplier;
                        var judged = Component.GetJudged(judgmentMeter, modeComponent, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, Component.JudgmentAssist.Default);
                        if (Component.Judged.Highest <= judged && judged <= Component.Judged.Lower)
                        {
                            return new JudgedNoteData
                            {
                                IDValue = JudgedNoteData.ID.TrapNoteJudgment,
                                JudgmentMeter = judgmentMeter,
                                Judged = Component.Judged.Lowest
                            };
                        }
                    }
                    break;
            }
            return null;
        }

        public override JudgedNoteData? AutoJudge(double wait)
        {
            if (Wait <= wait)
            {
                return new JudgedNoteData
                {
                    IDValue = JudgedNoteData.ID.Not
                };
            }
            return null;
        }

        public override void Paint(CanvasDrawingSession targetSession, bool isValidNetDrawings, DefaultCompute defaultComputer, ref Bound r)
        {
            var modeComponent = defaultComputer.ModeComponentValue;
            var drawingComponentValue = defaultComputer.DrawingComponentValue;
            var noteHeight = GetNoteHeight(defaultComputer);
            r.Set(GetPosition(defaultComputer), GetY(defaultComputer, GetMultiplierAsNoteMobility(modeComponent, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue)) - noteHeight + drawingComponentValue.noteHeightJudgments[TargetInput], GetNoteLength(defaultComputer), noteHeight);
            if (r.Position1 + r.Height > 0.0)
            {
                var trapNoteDrawing = UI.Instance.NoteDrawings[(int)defaultComputer.InputMode]?.ElementAt(TargetInput)?.ElementAt(defaultComputer.NoteFrame)?.ElementAt(TrapNoteContents)?.ElementAt(LongNote.LongNoteBefore);
                if (trapNoteDrawing.HasValue)
                {
                    targetSession.PaintDrawing(ref r, trapNoteDrawing, GetFaint(modeComponent, drawingComponentValue.judgmentMainPosition, defaultComputer.FaintCosine));
                    defaultComputer.NewNetDrawing(isValidNetDrawings, Event.Types.NetDrawing.Types.Variety.Note, trapNoteDrawing.Value.AverageColor, r.Position0 - drawingComponentValue.mainPosition, r.Position1, r.Length, r.Height * trapNoteDrawing.Value.AverageHeight);
                }
            }
        }
    }
}