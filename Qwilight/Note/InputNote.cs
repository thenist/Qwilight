using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.Note
{
    public class InputNote : BaseNote
    {
        public const int InputNoteContents = 0;
        public const int PositivePostableItemNoteContents = 15;
        public const int NegativePostableItemNoteContents = 16;
        public const int NeutralPostableItemNoteContents = 17;
        const double InputMillis = 1000.0;

        double _inputMillis;

        public override bool IsFailedAsTooLate(double wait, ModeComponent modeComponent, double judgmentStage, Component.JudgmentModeDate judgmentModeDate, Component.JudgmentMapDate judgmentMapDate, Component.LongNoteAssistDate longNoteAssistDate) => (wait - Wait) / modeComponent.AudioMultiplier >= Component.GetJudgmentMillis(Component.Judged.Lowest, modeComponent, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, 1);

        public override bool HasContents => true;

        public override bool HasStand => true;

        public override bool HasInput => true;

        public override int Layer { get; set; }

        public override int LogicalLayer => 1;

        public InputNote(double logicalY, double wait, ICollection<AudioNote> audioNotes, int input) : base(logicalY, wait)
        {
            LevyingInput = input;
            AudioNotes.AddRange(audioNotes);
        }

        public override bool IsVisible(DefaultCompute defaultComputer)
        {
            return GetY(defaultComputer, GetMultiplierAsNoteMobility(defaultComputer.ModeComponentValue, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue)) + defaultComputer.DrawingComponentValue.noteHeightJudgments[TargetInput] >= 0.0;
        }

        public override bool IsVisibleHalf(DefaultCompute defaultComputer)
        {
            var drawingComponentValue = defaultComputer.DrawingComponentValue;
            var yHeight = GetY(defaultComputer, GetMultiplierAsNoteMobility(defaultComputer.ModeComponentValue, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue)) + drawingComponentValue.noteHeightJudgments[TargetInput];
            return yHeight >= 0.0 && yHeight <= drawingComponentValue.judgmentMainPosition / 2;
        }

        public override void Init()
        {
            base.Init();
            _inputMillis = 0.0;
        }

        public double GetPosition(DefaultCompute defaultComputer)
        {
            return defaultComputer.GetPosition(LevyingInput) + (defaultComputer.GetPosition(TargetInput) - defaultComputer.GetPosition(LevyingInput)) * _inputMillis / InputMillis;
        }

        public double GetNoteLength(DefaultCompute defaultComputer)
        {
            return defaultComputer.DrawingComponentValue.DrawingNoteLengthMap[LevyingInput] + (defaultComputer.DrawingComponentValue.DrawingNoteLengthMap[TargetInput] - defaultComputer.DrawingComponentValue.DrawingNoteLengthMap[LevyingInput]) * _inputMillis / InputMillis;
        }

        public double GetNoteHeight(DefaultCompute defaultComputer)
        {
            return defaultComputer.DrawingComponentValue.noteHeights[LevyingInput] + (defaultComputer.DrawingComponentValue.noteHeights[TargetInput] - defaultComputer.DrawingComponentValue.noteHeights[LevyingInput]) * _inputMillis / InputMillis;
        }

        public override void MoveInputMillis(double millisLoopUnit, DefaultCompute defaultComputer)
        {
            if (TargetInput != LevyingInput)
            {
                _inputMillis = Math.Min(_inputMillis + millisLoopUnit * InputMillis / (defaultComputer.GetIIDXMultiplierMillis(defaultComputer.ModeComponentValue) / 2), InputMillis);
            }
        }

        public override void SetLayer(DefaultCompute defaultComputer)
        {
            if (defaultComputer.DrawingComponentValue.MainNoteLengthMap[TargetInput] > 0.0)
            {
                Layer = LongWait > 0.0 ? 3 : 4;
            }
            else
            {
                Layer = LongWait > 0.0 ? 1 : 2;
            }
        }

        public override JudgedNoteData? AutoJudge(double wait) => null;

        public override JudgedNoteData? Judge(int input, double wait, ModeComponent modeComponent, double judgmentStage, Component.JudgmentModeDate judgmentModeDate, Component.JudgmentMapDate judgmentMapDate, Component.LongNoteAssistDate longNoteAssistDate, Component.TrapNoteJudgmentDate trapNoteJudgmentDate, bool isAutoLongNote)
        {
            if (input > 0)
            {
                var judgmentMeter = (wait - Wait) / modeComponent.AudioMultiplier;
                var judged = Component.GetJudged(judgmentMeter, modeComponent, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, Component.JudgmentAssist.Default);
                if (judged != Component.Judged.Not)
                {
                    return new JudgedNoteData
                    {
                        IDValue = JudgedNoteData.ID.NoteJudgment,
                        JudgmentMeter = judgmentMeter,
                        Judged = judged
                    };
                }
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
                var inputNoteDrawings = UI.Instance.NoteDrawings[(int)defaultComputer.InputMode][TargetInput][defaultComputer.NoteFrame];
                var inputNoteDrawing = inputNoteDrawings[InputNoteContents][LongNote.LongNoteBefore];
                if (inputNoteDrawing.HasValue)
                {
                    targetSession.PaintDrawing(ref r, inputNoteDrawing, GetFaint(modeComponent, drawingComponentValue.judgmentMainPosition, defaultComputer.FaintCosine));
                    defaultComputer.NewNetDrawing(isValidNetDrawings, Event.Types.NetDrawing.Types.Variety.Note, inputNoteDrawing.Value.AverageColor, r.Position0 - drawingComponentValue.mainPosition, r.Position1, r.Length, r.Height * inputNoteDrawing.Value.AverageHeight);
                }
                if (PostableItemValue != null)
                {
                    var postableItemNoteDrawing = inputNoteDrawings[PostableItemValue.IsPositive switch
                    {
                        true => PositivePostableItemNoteContents,
                        false => NegativePostableItemNoteContents,
                        null => NeutralPostableItemNoteContents
                    }]?[LongNote.LongNoteBefore];
                    if (postableItemNoteDrawing.HasValue)
                    {
                        targetSession.PaintDrawing(ref r, postableItemNoteDrawing, GetFaint(modeComponent, drawingComponentValue.judgmentMainPosition, defaultComputer.FaintCosine));
                    }
                }
            }
        }
    }
}