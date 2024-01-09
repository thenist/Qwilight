using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.Note
{
    public sealed class VoidNote : InputNote
    {
        public const int VoidNoteContents = 14;

        public override bool HasStand => false;

        public override bool HasContents => false;

        public override int LogicalLayer => 5;

        public VoidNote(double logicalY, double wait, ICollection<AudioNote> audioNotes, int input) : base(logicalY, wait, audioNotes, input)
        {
        }

        public override void Paint(CanvasDrawingSession targetSession, bool isValidNetDrawings, DefaultCompute defaultComputer, ref Bound r)
        {
            if (defaultComputer.IsHellBPM)
            {
                var modeComponent = defaultComputer.ModeComponentValue;
                var drawingComponentValue = defaultComputer.DrawingComponentValue;
                var noteHeight = GetNoteHeight(defaultComputer);
                r.Set(GetPosition(defaultComputer), GetY(defaultComputer, GetMultiplierAsNoteMobility(modeComponent, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue)) - noteHeight + drawingComponentValue.noteHeightJudgments[TargetInput], GetNoteLength(defaultComputer), noteHeight);
                if (r.Position1 + r.Height > 0.0)
                {
                    var voidNoteDrawing = UI.Instance.NoteDrawings[(int)defaultComputer.InputMode]?.ElementAt(TargetInput)?.ElementAt(defaultComputer.NoteFrame)?.ElementAt(VoidNoteContents)?.ElementAt(LongNote.LongNoteBefore);
                    if (voidNoteDrawing.HasValue)
                    {
                        targetSession.PaintDrawing(ref r, voidNoteDrawing, GetFaint(modeComponent, drawingComponentValue.judgmentMainPosition, defaultComputer.FaintCosine));
                    }
                }
            }
        }

        public override JudgedNoteData? Judge(int input, double wait, ModeComponent modeComponent, double judgmentStage, Component.JudgmentModeDate judgmentModeDate, Component.JudgmentMapDate judgmentMapDate, Component.LongNoteAssistDate longNoteAssistDate, Component.TrapNoteJudgmentDate trapNoteJudgmentDate, bool isAutoLongNote) => null;

        public override JudgedNoteData? AutoJudge(double wait)
        {
            if (Wait <= wait)
            {
                Judged = Component.Judged.Highest;
                return new JudgedNoteData
                {
                    IDValue = JudgedNoteData.ID.HandleVoid
                };
            }
            return null;
        }
    }
}