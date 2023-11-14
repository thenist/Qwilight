namespace Qwilight.NoteFile
{
    public sealed class NetNoteFile : BaseNoteFile
    {
        public override NoteVariety NoteVarietyValue => default;

        public NetNoteFile(JSON.TwilightQuitNet.QuitNetItem quitNetItem) : base(default, default, default)
        {
            Title = quitNetItem.title;
            Artist = quitNetItem.artist;
            Genre = quitNetItem.genre;
            LevelValue = quitNetItem.level;
            LevelText = quitNetItem.levelText;
            WantLevelID = quitNetItem.wantLevelID;
            TotalNotes = quitNetItem.totalNotes;
            JudgmentStage = quitNetItem.judgmentStage;
            HitPointsValue = quitNetItem.hitPointsValue;
            HighestInputCount = quitNetItem.highestInputCount;
            Length = quitNetItem.length;
            BPM = quitNetItem.bpm;
            InputMode = quitNetItem.inputMode;
        }
    }
}