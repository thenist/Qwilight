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

        public override JudgedNoteData? Judge(int input, double wait, ModeComponent modeComponentValue, double judgmentStage, int judgmentModeDate, int judgmentMapDate, int longNoteAssistDate, int trapNoteJudgmentDate, bool isAutoLongNote)
        {
            switch (trapNoteJudgmentDate)
            {
                case Component.TrapNoteJudgmentDate1146:
                    if (_isPowered && input > 0)
                    {
                        var judgmentMeter = (wait - Wait) / modeComponentValue.AudioMultiplier;
                        var judged = Component.GetJudged(judgmentMeter, modeComponentValue, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, Component.JudgmentAssist.Default);
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
            var modeComponentValue = defaultComputer.ModeComponentValue;
            var drawingComponentValue = defaultComputer.DrawingComponentValue;
            var noteHeight = GetNoteHeight(defaultComputer);
            r.Set(GetPosition(defaultComputer), GetY(defaultComputer, GetMultiplierAsNoteMobility(modeComponentValue, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue)) - noteHeight + drawingComponentValue.noteHeightJudgments[TargetInput], GetNoteLength(defaultComputer), noteHeight);
            if (r.Position1 + r.Height > 0.0)
            {
                var trapNoteDrawing = UI.Instance.NoteDrawings[(int)defaultComputer.InputMode]?.ElementAt(TargetInput)?.ElementAt(defaultComputer.NoteFrame)?.ElementAt(TrapNoteContents)?.ElementAt(LongNote.LongNoteBefore);
                if (trapNoteDrawing.HasValue)
                {
                    targetSession.PaintDrawing(ref r, trapNoteDrawing, GetFaint(modeComponentValue, drawingComponentValue.judgmentMainPosition, defaultComputer.FaintCosine));
                    defaultComputer.NewNetDrawing(isValidNetDrawings, Event.Types.NetDrawing.Types.Variety.Note, trapNoteDrawing.Value.AverageColor, r.Position0 - drawingComponentValue.mainPosition, r.Position1, r.Length, r.Height * trapNoteDrawing.Value.StandardHeight);
                }
            }
        }
    }
}