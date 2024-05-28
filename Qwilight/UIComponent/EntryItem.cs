using Qwilight.NoteFile;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class EntryItem : Model
    {
        public enum LogicalVariety
        {
            Salt, Level, DefaultEntryItem, DefaultEntryConfigure
        }

        public static readonly EntryItem DefaultEntryConfigureEntryItem = new()
        {
            IsLogical = true,
            NoteFiles = [ModifyDefaultEntryNoteFile.Instance],
            WellNoteFiles = new([ModifyDefaultEntryNoteFile.Instance]),
            LogicalVarietyValue = LogicalVariety.DefaultEntryConfigure
        };

        public static readonly EntryItem SaltEntryItem = new()
        {
            IsLogical = true,
            NoteFiles = [SaltNoteFile.Instance],
            WellNoteFiles = new([SaltNoteFile.Instance]),
            LogicalVarietyValue = LogicalVariety.Salt
        };

        string _eventNoteName;
        string _fittedText;

        public BaseNoteFile NoteFile => NoteFiles[NotePosition];

        public string PositionText => WellNoteFiles.Count >= 2 ? $"{NotePosition - NoteFiles.SkipLast(NoteFiles.Length - NotePosition).Count(noteFile => !WellNoteFiles.Contains(noteFile)) + 1}／{WellNoteFiles.Count}" : string.Empty;

        public List<BaseNoteFile> WellNoteFiles { get; set; }

        public bool WasNotePositionModified { get; set; }

        public int NotePosition { get; set; }

        public void ModifyNotePosition(int notePosition)
        {
            if (NotePosition != notePosition)
            {
                NotePosition = notePosition;
                WasNotePositionModified = true;
            }
        }

        public DateTime ModifiedDate { get; init; }

        public string WantLevelID { get; set; } = string.Empty;

        public DateTime? LatestDate { get; set; }

        public int HandledCount { get; set; }

        public DefaultEntryItem DefaultEntryItem { get; init; }

        public string Title { get; set; } = string.Empty;

        public string EntryPath { get; init; } = string.Empty;

        public string EventNoteID { get; init; } = string.Empty;

        public bool IsLogical { get; init; }

        public LogicalVariety LogicalVarietyValue { get; init; }

        public string EventNoteName
        {
            get => _eventNoteName;

            set => SetProperty(ref _eventNoteName, value, nameof(EventNoteName));
        }

        public string FittedText
        {
            get => _fittedText;

            set => SetProperty(ref _fittedText, value, nameof(FittedText));
        }

        public ImageSource DrawingInNoteFileWindow => NoteFile.EssentialDrawing ?? NoteFile.LogicalDrawing ?? BaseUI.Instance.DefaultEntryDrawings[(int)NoteFile.DefaultEntryItem.DefaultEntryVarietyValue];

        public bool IsBanned => NoteFile.IsBanned || !string.IsNullOrEmpty(EventNoteID);

        public DB.EventNoteVariety EventNoteVariety { get; init; }

        public int EntryItemID { get; init; }

        public double BPM { get; set; }

        public string Artist { get; set; } = string.Empty;

        public double Length { get; set; }

        public double HitPointsValue { get; set; }

        public BaseNoteFile.Handled HandledValue { get; set; }

        public int TotalNotes { get; set; }

        public double LevelTextValue { get; set; }

        public int HighestInputCount { get; set; }

        public double AverageInputCount => Length > 0.0 ? TotalNotes / Length : TotalNotes;

        public BaseNoteFile[] NoteFiles { get; set; }

        public BaseNoteFile[] CompatibleNoteFiles { get; set; }

        public bool CanWipeNoteFile => CompatibleNoteFiles.Length > 1;

        public override bool Equals(object obj) => obj is EntryItem entryItem && entryItem.EntryItemID == EntryItemID;

        public override int GetHashCode() => EntryItemID.GetHashCode();

        public bool HigherNoteFile()
        {
            var notePosition = NotePosition + 1;
            while (true)
            {
                if (notePosition < NoteFiles.Length)
                {
                    if (WellNoteFiles.Contains(NoteFiles[notePosition]))
                    {
                        ModifyNotePosition(notePosition);
                        return true;
                    }
                    else
                    {
                        ++notePosition;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        public bool LowerNoteFile()
        {
            var notePosition = NotePosition - 1;
            while (true)
            {
                if (notePosition < 0)
                {
                    return false;
                }
                else
                {
                    if (WellNoteFiles.Contains(NoteFiles[notePosition]))
                    {
                        ModifyNotePosition(notePosition);
                        return true;
                    }
                    else
                    {
                        --notePosition;
                    }
                }
            }
        }

        public void WipeFavoriteEntry()
        {
            foreach (var noteFile in NoteFiles)
            {
                if (!noteFile.IsLogical)
                {
                    noteFile.FavoriteEntryItems.Clear();
                    noteFile.NotifyHasFavoriteEntryItem();
                }
            }
        }

        public Brush LevelPaint => NoteFile?.LevelPaint;

        public Color LevelColor => NoteFile?.LevelColor ?? Colors.Transparent;
    }
}