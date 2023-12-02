using Qwilight.NoteFile;
using Qwilight.Utilities;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct FitMode : IEquatable<FitMode>
    {
        public const int BPM = 0;
        public const int Artist = 1;
        public const int ModifiedDate = 2;
        public const int Length = 3;
        public const int Title = 4;
        public const int TotalNotes = 5;
        public const int LevelTextValue = 6;
        public const int HighestInputCount = 7;
        public const int LatestDate = 8;
        public const int HandledCount = 9;
        public const int AverageInputCount = 10;
        public const int EntryPath = 11;

        public ImageSource Drawing => Mode switch
        {
            Title => BaseUI.Instance.TitleFitDrawing,
            Artist => BaseUI.Instance.ArtistFitDrawing,
            ModifiedDate => BaseUI.Instance.ModifiedDateFitDrawing,
            TotalNotes => BaseUI.Instance.TotalNotesFitDrawing,
            Length => BaseUI.Instance.LengthFitDrawing,
            BPM => BaseUI.Instance.BPMFitDrawing,
            LevelTextValue => BaseUI.Instance.LevelTextValueFitDrawing,
            HighestInputCount => BaseUI.Instance.HighestInputCountFitDrawing,
            LatestDate => BaseUI.Instance.LatestDateFitDrawing,
            HandledCount => BaseUI.Instance.HandledCountFitDrawing,
            AverageInputCount => BaseUI.Instance.AverageInputCountFitDrawing,
            EntryPath => BaseUI.Instance.EntryPathFitDrawing,
            _ => default,
        };

        public int Mode { get; init; }

        public override bool Equals(object obj) => obj is FitMode fitMode && Equals(fitMode);

        public bool Equals(FitMode other) => Mode == other.Mode;

        public void Fit(List<EntryItem> entryItems)
        {
            switch (Mode)
            {
                case Title:
                    FitImpl(entryItem => entryItem.Title, null);
                    break;
                case Artist:
                    FitImpl(entryItem => entryItem.Artist, null);
                    break;
                case ModifiedDate:
                    FitImpl(entryItem => entryItem.ModifiedDate, null, false);
                    break;
                case BPM:
                    FitImpl(entryItem => entryItem.BPM, (entryItem, noteFile) => entryItem.BPM == noteFile.BPM);
                    break;
                case TotalNotes:
                    FitImpl(entryItem => entryItem.TotalNotes, (entryItem, noteFile) => entryItem.TotalNotes == noteFile.TotalNotes);
                    break;
                case Length:
                    FitImpl(entryItem => entryItem.Length, (entryItem, noteFile) => entryItem.Length == noteFile.Length);
                    break;
                case LevelTextValue:
                    FitImpl(entryItem => entryItem.LevelTextValue, (entryItem, noteFile) => entryItem.LevelTextValue == noteFile.LevelTextValue);
                    break;
                case HighestInputCount:
                    FitImpl(entryItem => entryItem.HighestInputCount, (entryItem, noteFile) => entryItem.HighestInputCount == noteFile.HighestInputCount);
                    break;
                case LatestDate:
                    FitImpl(entryItem => entryItem.LatestDate ?? DateTime.MinValue, (entryItem, noteFile) => (entryItem.LatestDate ?? DateTime.MinValue) == (noteFile.LatestDate ?? DateTime.MinValue), false);
                    break;
                case HandledCount:
                    FitImpl(entryItem => entryItem.HandledCount, (entryItem, noteFile) => entryItem.HandledCount == noteFile.HandledCount, false);
                    break;
                case AverageInputCount:
                    FitImpl(entryItem => entryItem.AverageInputCount, (entryItem, noteFile) => entryItem.AverageInputCount == noteFile.AverageInputCount);
                    break;
                case EntryPath:
                    FitImpl(entryItem => entryItem.EntryPath, null);
                    break;
            }

            void FitImpl<T>(Func<EntryItem, T> fromEntryItem, Func<EntryItem, BaseNoteFile, bool> onEqual, bool asc = true) where T : IComparable<T>
            {
                var wantLevelItem = Configure.Instance.WantLevelIDs.Length > 0;
                entryItems.Sort((x, y) =>
                {
                    var value = wantLevelItem ? LevelSystem.Instance.WantLevelIDEquality.Compare(x.WantLevelID, y.WantLevelID) : 0;
                    return value != 0 ? value : asc ? fromEntryItem(x).CompareTo(fromEntryItem(y)) : fromEntryItem(y).CompareTo(fromEntryItem(x));
                });

                foreach (var entryItem in entryItems)
                {
                    if (!entryItem.WasNotePositionModified && ((wantLevelItem && entryItem.NoteFile.WantLevelID != entryItem.WantLevelID) || onEqual?.Invoke(entryItem, entryItem.NoteFile) == false))
                    {
                        var wantLevelIDSatisfied = wantLevelItem ? entryItem.WellNoteFiles.Where(noteFile => noteFile.WantLevelID == entryItem.WantLevelID) : entryItem.WellNoteFiles;
                        var equalSatisfied = onEqual != null ? wantLevelIDSatisfied.Where(noteFile => onEqual(entryItem, noteFile)) : wantLevelIDSatisfied;

                        var noteFile = equalSatisfied.FirstOrDefault() ?? wantLevelIDSatisfied.FirstOrDefault();
                        if (noteFile != null)
                        {
                            entryItem.NotePosition = Array.IndexOf(entryItem.NoteFiles, noteFile);
                        }
                    }
                }
            }
        }

        public override int GetHashCode() => Mode.GetHashCode();

        public void SetFittedText(EntryItem entryItem)
        {
            switch (Mode)
            {
                case BPM:
                    var lowestBPM = double.MaxValue;
                    var highestBPM = double.MinValue;
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        var bpm = noteFile.BPM;
                        noteFile.FittedText = $"{bpm} BPM";
                        lowestBPM = Math.Min(lowestBPM, bpm);
                        highestBPM = Math.Max(highestBPM, bpm);
                    }
                    entryItem.FittedText = lowestBPM != highestBPM ? $"{lowestBPM} BPM ~ {highestBPM} BPM" : $"{lowestBPM} BPM";
                    break;
                case Artist:
                case Title:
                case EntryPath:
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        noteFile.FittedText = null;
                    }
                    entryItem.FittedText = null;
                    break;
                case ModifiedDate:
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        noteFile.FittedText = null;
                    }
                    entryItem.FittedText = entryItem.ModifiedDate.ToString();
                    break;
                case LatestDate:
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        noteFile.FittedText = noteFile.LatestDate?.ToString();
                    }
                    entryItem.FittedText = entryItem.LatestDate?.ToString();
                    break;
                case TotalNotes:
                    var lowestTotalNotes = int.MaxValue;
                    var highestTotalNotes = int.MinValue;
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        var totalNotes = noteFile.TotalNotes;
                        noteFile.FittedText = totalNotes.ToString(LanguageSystem.Instance.CountContents);
                        lowestTotalNotes = Math.Min(lowestTotalNotes, totalNotes);
                        highestTotalNotes = Math.Max(highestTotalNotes, totalNotes);
                    }
                    entryItem.FittedText = lowestTotalNotes != highestTotalNotes ? $"{lowestTotalNotes.ToString(LanguageSystem.Instance.CountContents)} ~ {highestTotalNotes.ToString(LanguageSystem.Instance.CountContents)}" : lowestTotalNotes.ToString(LanguageSystem.Instance.CountContents);
                    break;
                case Length:
                    var lowestLength = double.MaxValue;
                    var highestLength = double.MinValue;
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        var length = noteFile.Length;
                        noteFile.FittedText = Utility.GetLengthText(length);
                        lowestLength = Math.Min(lowestLength, length);
                        highestLength = Math.Max(highestLength, length);
                    }
                    entryItem.FittedText = lowestLength != highestLength ? $"{Utility.GetLengthText(lowestLength)} ~ {Utility.GetLengthText(highestLength)}" : Utility.GetLengthText(lowestLength);
                    break;
                case HandledCount:
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        noteFile.FittedText = noteFile.HandledCount.ToString(LanguageSystem.Instance.HandledContents);
                    }
                    var handledCount = string.IsNullOrEmpty(entryItem.EventNoteID) ? entryItem.WellNoteFiles.Sum(noteFile => noteFile.HandledCount) : entryItem.HandledCount;
                    entryItem.FittedText = handledCount > 0 ? handledCount.ToString(LanguageSystem.Instance.HandledContents) : string.Empty;
                    break;
                case LevelTextValue:
                    var lowestLevelTextValue = double.MaxValue;
                    var highestLevelTextValue = double.MinValue;
                    var lowestLevelTextNoteFile = string.Empty;
                    var highestLevelTextNoteFile = string.Empty;
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        noteFile.FittedText = null;
                        var levelTextValue = noteFile.LevelTextValue;
                        var levelText = noteFile.LevelText;
                        if (levelTextValue < lowestLevelTextValue)
                        {
                            lowestLevelTextValue = levelTextValue;
                            lowestLevelTextNoteFile = levelText;
                        }
                        if (levelTextValue > highestLevelTextValue)
                        {
                            highestLevelTextValue = levelTextValue;
                            highestLevelTextNoteFile = levelText;
                        }
                    }
                    entryItem.FittedText = lowestLevelTextValue != highestLevelTextValue ? $"{lowestLevelTextNoteFile} ~ {highestLevelTextNoteFile}" : lowestLevelTextNoteFile;
                    break;
                case HighestInputCount:
                    var lowestHighestInputCount = double.MaxValue;
                    var highestHighestInputCount = double.MinValue;
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        var highestInputCount = noteFile.HighestInputCount;
                        noteFile.FittedText = highestInputCount.ToString("#,##0.## / s");
                        lowestHighestInputCount = Math.Min(lowestHighestInputCount, highestInputCount);
                        highestHighestInputCount = Math.Max(highestHighestInputCount, highestInputCount);
                    }
                    entryItem.FittedText = lowestHighestInputCount != highestHighestInputCount ? $"{lowestHighestInputCount:#,##0.## / s} ~ {highestHighestInputCount:#,##0.## / s}" : lowestHighestInputCount.ToString("#,##0.## / s");
                    break;
                case AverageInputCount:
                    var lowestAverageInputCount = double.MaxValue;
                    var highestAverageInputCount = double.MinValue;
                    foreach (var noteFile in entryItem.WellNoteFiles)
                    {
                        var averageInputCount = noteFile.AverageInputCount;
                        noteFile.FittedText = averageInputCount.ToString("#,##0.## / s");
                        lowestAverageInputCount = Math.Min(lowestAverageInputCount, averageInputCount);
                        highestAverageInputCount = Math.Max(highestAverageInputCount, averageInputCount);
                    }
                    entryItem.FittedText = lowestAverageInputCount != highestAverageInputCount ? $"{lowestAverageInputCount:#,##0.## / s} ~ {highestAverageInputCount:#,##0.## / s}" : lowestAverageInputCount.ToString("#,##0.## / s");
                    break;
            }
        }

        public static bool operator ==(FitMode left, FitMode right) => left.Equals(right);

        public static bool operator !=(FitMode left, FitMode right) => !(left == right);
    }
}