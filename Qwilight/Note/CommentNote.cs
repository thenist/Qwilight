using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.Note
{
    public sealed class CommentNote : InputNote
    {
        public const int CommentNoteContents = 9;

        public override bool HasStand => false;

        public override int LogicalLayer => 6;

        public CommentNote(double logicalY, double wait, int input) : base(logicalY, wait, Array.Empty<AudioNote>(), input)
        {
        }

        public override void Paint(CanvasDrawingSession targetSession, bool isValidNetDrawings, DefaultCompute defaultComputer, ref Bound r)
        {
            if (Configure.Instance.UICommentNote)
            {
                var modeComponentValue = defaultComputer.ModeComponentValue;
                var drawingComponentValue = defaultComputer.DrawingComponentValue;
                var noteHeight = GetNoteHeight(defaultComputer);
                r.Set(GetPosition(defaultComputer), GetY(defaultComputer, GetMultiplierAsNoteMobility(modeComponentValue, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue)) - noteHeight + drawingComponentValue.noteHeightJudgments[TargetInput], GetNoteLength(defaultComputer), noteHeight);
                if (r.Position1 + r.Height > 0.0)
                {
                    targetSession.PaintDrawing(ref r, UI.Instance.NoteDrawings[(int)defaultComputer.InputMode][TargetInput][defaultComputer.NoteFrame][CommentNoteContents][LongNote.LongNoteBefore], GetFaint(modeComponentValue, drawingComponentValue.judgmentMainPosition, defaultComputer.FaintCosine));
                }
            }
        }

        public override JudgedNoteData? Judge(int input, double wait, ModeComponent modeComponentValue, double judgmentStage, int judgmentModeDate, int judgmentMapDate, int longNoteAssistDate, int trapNoteJudgmentDate, bool isAutoLongNote) => default;

        public override JudgedNoteData? AutoJudge(double wait)
        {
            if (Wait <= wait)
            {
                Judged = Component.Judged.Highest;
                return new JudgedNoteData
                {
                    IDValue = JudgedNoteData.ID.Not
                };
            }
            return default;
        }
    }
}