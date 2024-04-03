namespace Qwilight.Note
{
    public struct JudgedNoteData
    {
        public enum ID
        {
            Not,
            NoteJudgment,
            TrapNoteJudgment,
            LevyLongNoteJudgment,
            QuitLongNoteJudgment,
            AutoLongNoteJudgment,
            FailedLongNoteJudgment,
            HandleVoid,
            HandleMeter
        }

        public ID IDValue { get; init; }

        public double JudgmentMeter { get; init; }

        public Component.Judged Judged { get; init; }

        public string MeterText { get; init; }
    }
}