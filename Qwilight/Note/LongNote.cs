using Microsoft.Graphics.Canvas;
using Qwilight.Compute;
using Qwilight.Utilities;

namespace Qwilight.Note
{
    public sealed class LongNote : InputNote
    {
        /// <summary>
        /// 위
        /// </summary>
        public const int LongNoteTail = 1;
        public const int LongNoteContents = 2;
        /// <summary>
        /// 아래
        /// </summary>
        public const int LongNoteFront = 3;

        public const int LongNoteBefore = 0;
        public const int LongNoteHandling = 1;
        public const int LongNoteFailed = 2;

        public override int LogicalLayer => 2;

        public LongNote(double logicalY, double wait, ICollection<AudioNote> audioNotes, int input, double longWait, double longHeight) : base(logicalY, wait, audioNotes, input)
        {
            LongWait = longWait;
            LongHeight = longHeight;
        }

        public override JudgedNoteData? Judge(int input, double wait, ModeComponent modeComponent, double judgmentStage, Component.JudgmentModeDate judgmentModeDate, Component.JudgmentMapDate judgmentMapDate, Component.LongNoteAssistDate longNoteAssistDate, Component.TrapNoteJudgmentDate trapNoteJudgmentDate, bool isAutoLongNote)
        {
            if (input > 0)
            {
                if (Judged == Component.Judged.Not)
                {
                    var judgmentMeter = (wait - Wait) / modeComponent.AudioMultiplier;
                    var judged = Component.GetJudged(judgmentMeter, modeComponent, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate);
                    if (judged != Component.Judged.Not)
                    {
                        return new JudgedNoteData
                        {
                            IDValue = JudgedNoteData.ID.LevyLongNoteJudgment,
                            JudgmentMeter = judgmentMeter,
                            Judged = judged
                        };
                    }
                }
            }
            else if (Judged != Component.Judged.Not)
            {
                var longJudgmentMeter = (wait - (Wait + LongWait)) / modeComponent.AudioMultiplier;
                if ((isAutoLongNote && modeComponent.LongNoteModeValue != ModeComponent.LongNoteMode.Input) || modeComponent.LongNoteModeValue == ModeComponent.LongNoteMode.Auto)
                {
                    if (Component.GetIsJudgment(longJudgmentMeter, Component.Judged.Lower, modeComponent, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, Component.JudgmentAssist.LongNoteUp))
                    {
                        return new JudgedNoteData
                        {
                            IDValue = JudgedNoteData.ID.AutoLongNoteJudgment,
                            JudgmentMeter = longJudgmentMeter,
                            Judged = Judged
                        };
                    }
                    return new JudgedNoteData
                    {
                        IDValue = JudgedNoteData.ID.FailedLongNoteJudgment
                    };
                }
                else
                {
                    var judged = Component.GetJudged(longJudgmentMeter, modeComponent, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, Component.JudgmentAssist.LongNoteUp);
                    if (judged != Component.Judged.Not)
                    {
                        return new JudgedNoteData
                        {
                            IDValue = JudgedNoteData.ID.QuitLongNoteJudgment,
                            JudgmentMeter = longJudgmentMeter,
                            Judged = judged
                        };
                    }
                    return new JudgedNoteData
                    {
                        IDValue = JudgedNoteData.ID.FailedLongNoteJudgment,
                        Judged = judged
                    };
                }
            }
            return null;
        }

        public override bool IsTooLong(double wait, ModeComponent modeComponent, double judgmentStage, Component.JudgmentModeDate judgmentModeDate, Component.JudgmentMapDate judgmentMapDate, Component.LongNoteAssistDate longNoteAssistDate) => (wait - (Wait + LongWait)) / modeComponent.AudioMultiplier > Component.GetJudgmentMillis(Component.Judged.Lower, modeComponent, judgmentStage, judgmentModeDate, judgmentMapDate, longNoteAssistDate, 1, Component.JudgmentAssist.LongNoteUp);

        public override void Paint(CanvasDrawingSession targetSession, bool isValidNetDrawings, DefaultCompute defaultComputer, ref Bound r)
        {
            var modeComponent = defaultComputer.ModeComponentValue;
            var multiplier = GetMultiplierAsNoteMobility(modeComponent, defaultComputer.NoteMobilityCosine, defaultComputer.NoteMobilityValue);
            var drawingComponentValue = defaultComputer.DrawingComponentValue;
            var judgmentMainPosition = drawingComponentValue.judgmentMainPosition;
            var y = GetY(defaultComputer, multiplier);
            var pointHeight = LongHeight * multiplier;
            var longNoteFrontEdgeHeight = drawingComponentValue.longNoteFrontEdgeHeights[TargetInput];
            var longNoteTailEdgeHeight = drawingComponentValue.longNoteTailEdgeHeights[TargetInput];
            var longNoteFrontEdgePosition = drawingComponentValue.longNoteFrontEdgePositions[TargetInput];
            var longNoteTailEdgePosition = drawingComponentValue.longNoteTailEdgePositions[TargetInput];
            var noteLength = GetNoteLength(defaultComputer);
            var notePosition = GetPosition(defaultComputer);
            var faint = GetFaint(modeComponent, judgmentMainPosition, defaultComputer.FaintCosine);
            var longNoteTailContentsHeight = drawingComponentValue.longNoteTailContentsHeights[TargetInput];
            var longNoteFrontContentsHeight = drawingComponentValue.longNoteFrontContentsHeights[TargetInput];
            var longNoteContentsHeight = pointHeight + longNoteTailContentsHeight + longNoteFrontContentsHeight;
            var longNoteDrawings = UI.Instance.NoteDrawings[(int)defaultComputer.InputMode][TargetInput][defaultComputer.NoteFrame];
            if (!IsFailed && Judged != Component.Judged.Not)
            {
                var noteWait = Configure.Instance.UIConfigureValue.NoteWait;
                var longHeight = longNoteContentsHeight + judgmentMainPosition + noteWait - y;
                if (longHeight > 0.0)
                {
                    r.Set(notePosition, y - pointHeight - longNoteTailContentsHeight, noteLength, longHeight);
                    PaintLongNoteContents(ref r);

                    r.Set(notePosition, y - pointHeight - Math.Max(0.0, longNoteTailEdgeHeight) - longNoteTailEdgePosition, noteLength, Math.Abs(longNoteTailEdgeHeight));
                    PaintLongNoteTail(ref r);
                }
                if (UI.Instance.MaintainLongNoteFrontEdge)
                {
                    r.Set(notePosition, judgmentMainPosition - Math.Max(0.0, -longNoteFrontEdgeHeight) + longNoteFrontEdgePosition + noteWait, noteLength, Math.Abs(longNoteFrontEdgeHeight));
                    PaintLongNoteFront(ref r);
                }
            }
            else
            {
                if (IsFailed)
                {
                    faint /= 2;
                }
                if (longNoteContentsHeight > 0.0)
                {
                    r.Set(notePosition, y - pointHeight - longNoteTailContentsHeight, noteLength, longNoteContentsHeight);
                    PaintLongNoteContents(ref r);

                    r.Set(notePosition, y - pointHeight - Math.Max(0.0, longNoteTailEdgeHeight) - longNoteTailEdgePosition, noteLength, Math.Abs(longNoteTailEdgeHeight));
                    PaintLongNoteTail(ref r);
                }
                r.Set(notePosition, y - Math.Max(0.0, -longNoteFrontEdgeHeight) + longNoteFrontEdgePosition, noteLength, Math.Abs(longNoteFrontEdgeHeight));
                PaintLongNoteFront(ref r);
            }

            void PaintLongNoteContents(ref Bound r)
            {
                if (r.Position1 + r.Height > 0.0)
                {
                    var longNoteDrawing = longNoteDrawings?[LongNoteContents]?[IsFailed ? LongNoteFailed : LongNoteBefore];
                    if (longNoteDrawing.HasValue)
                    {
                        targetSession.PaintDrawing(ref r, longNoteDrawing, faint);
                        defaultComputer.NewNetDrawing(isValidNetDrawings, Event.Types.NetDrawing.Types.Variety.Note, longNoteDrawing.Value.AverageColor, r.Position0 - drawingComponentValue.mainPosition, r.Position1, r.Length, r.Height * longNoteDrawing.Value.AverageHeight);
                    }
                    if (PostableItemValue != null)
                    {
                        var postableItemNoteDrawing = longNoteDrawings?[PostableItemValue.IsPositive switch
                        {
                            true => PositivePostableItemNoteContents,
                            false => NegativePostableItemNoteContents,
                            null => NeutralPostableItemNoteContents
                        }]?[IsFailed ? LongNoteFailed : LongNoteBefore];
                        if (postableItemNoteDrawing.HasValue)
                        {
                            var postableItemNoteDrawingValue = postableItemNoteDrawing.Value;
                            var postableItemNoteDrawingBound = postableItemNoteDrawingValue.DrawingBound;
                            r.Height = r.Length * postableItemNoteDrawingBound.Height / postableItemNoteDrawingBound.Length;
                            targetSession.PaintDrawing(ref r, postableItemNoteDrawingValue, GetFaint(modeComponent, drawingComponentValue.judgmentMainPosition, defaultComputer.FaintCosine));
                        }
                    }
                }
            }

            void PaintLongNoteTail(ref Bound r)
            {
                if (r.Position1 + r.Height > 0.0)
                {
                    targetSession.PaintDrawing(ref r, longNoteDrawings?[LongNoteTail]?[IsFailed ? LongNoteFailed : LongNoteBefore], faint);
                }
            }

            void PaintLongNoteFront(ref Bound r)
            {
                if (r.Position1 + r.Height > 0.0)
                {
                    targetSession.PaintDrawing(ref r, longNoteDrawings?[LongNoteFront]?[IsFailed ? LongNoteFailed : LongNoteBefore], faint);
                }
            }
        }
    }
}