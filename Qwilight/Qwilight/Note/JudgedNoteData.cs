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

        public ID IDValue { get; set; }

        public double JudgmentMeter { get; set; }

        public Component.Judged Judged { get; set; }
    }
}