using Qwilight.NoteFile;
using Qwilight.Utilities;
using System.Globalization;
using System.Text;
using System.Windows.Media;

namespace Qwilight
{
    public abstract class Computing : Model
    {
        string _platformText;
        int _longNoteModeDate;
        string _genre;

        public int LongNoteModeDate
        {
            get => _longNoteModeDate;

            set
            {
                _longNoteModeDate = value;
                SetAutoLongNote();
            }
        }

        public bool IsAutoLongNote { get; set; }

        public bool IsLongNoteStand1 => LongNoteModeDate switch
        {
            Component.LongNoteMode11420 => IsAutoLongNote,
            _ => false
        };

        public bool IsBanned { get; set; }

        public Component.InputMode InputMode { get; set; }

        public string Genre
        {
            get => _genre;

            set
            {
                _genre = value;
                GenreText = Utility.GetGenreText(value);
            }
        }

        public string GenreText { get; set; }

        public string Artist { get; set; }

        public string Title { get; set; }

        public string PlatformText
        {
            get => _platformText;

            set
            {
                _platformText = value;

                if (Encoding.UTF8.GetByteCount(value) <= 128)
                {
                    PlatformText128 = value;
                }
                else
                {
                    var builder = new StringBuilder();
                    var dataCount = 0;
                    var enumerator = StringInfo.GetTextElementEnumerator(value);
                    while (enumerator.MoveNext())
                    {
                        var textElement = enumerator.GetTextElement();
                        dataCount += Encoding.UTF8.GetByteCount(textElement);
                        if (dataCount <= 128)
                        {
                            builder.Append(textElement);
                        }
                        else
                        {
                            break;
                        }
                    }
                    PlatformText128 = builder.ToString();
                }
            }
        }

        public string PlatformText128 { get; set; }

        public string LevelText { get; set; }

        public double LevelTextValue { get; set; }

        public double LevyingBPM { get; set; }

        public double BPM { get; set; }

        public double Length { get; set; }

        public int TotalNotes { get; set; }

        public int AutoableNotes { get; set; }

        public int TrapNotes { get; set; }

        public int LongNotes { get; set; }

        public double JudgmentStage { get; set; }

        public double HitPointsValue { get; set; }

        public BaseNoteFile.Level LevelValue { get; set; }

        public string NoteDrawingName { get; set; }

        public string NoteDrawingPath { get; set; }

        public string BannerDrawingName { get; set; }

        public string BannerDrawingPath { get; set; }

        public string TrailerAudioName { get; set; }

        public string TrailerAudioPath { get; set; }

        public string AssistFileName { get; set; }

        public double AudioLevyingPosition { get; set; }

        public bool IsSalt { get; set; }

        public string Tag { get; set; }

        public double LowestBPM { get; set; }

        public double HighestBPM { get; set; }

        public int HighestInputCount { get; set; }

        public bool IsHellBPM { get; set; }

        public double AverageInputCount => Length > 0.0 ? 1000.0 * TotalNotes / Length : TotalNotes;

        public bool HasBPMMap => LowestBPM != HighestBPM;

        public string JudgmentStageContents => $"{JudgmentStage switch
        {
            0.0 => "JUDGE: VEZ",
            3.0 => "JUDGE: EZ",
            5.0 => "JUDGE: NM",
            7.0 => "JUDGE: HD",
            10.0 => "JUDGE: VHD",
            _ => $"LV. {Math.Round(JudgmentStage, 1)}"
        }}, HP: {Math.Round(100 * HitPointsValue, 2)}％";

        public string TotalNotesContents => $"{TotalNotes.ToString("#,##0")} (SC: {AutoableNotes.ToString("#,##0")}, {(IsAutoLongNote ? "LN" : "CN")}: {LongNotes.ToString("#,##0")}, MN: {TrapNotes.ToString("#,##0")})";

        public string BPMMapValue => $"{Math.Round(LowestBPM, 2)} ~ {Math.Round(HighestBPM, 2)} BPM";

        public abstract BaseNoteFile.NoteVariety NoteVarietyValue { get; }

        public Brush LevelPaint => BaseUI.Instance.LevelPaints[(int)LevelValue];

        public Computing() => InitCompiled();

        void SetAutoLongNote()
        {
            switch (LongNoteModeDate)
            {
                case Component.LongNoteMode100:
                    IsAutoLongNote = false;
                    break;
                case Component.LongNoteMode11420:
                case Component.LongNoteMode1164:
                    IsAutoLongNote = true;
                    break;
            }
        }

        public void InitCompiled()
        {
            LongNoteModeDate = Component.LatestLongNoteMode;
            IsBanned = false;
            InputMode = Component.InputMode.InputMode51;
            Genre = string.Empty;
            Artist = string.Empty;
            Title = string.Empty;
            PlatformText = string.Empty;
            LevelText = string.Empty;
            LevelTextValue = double.NaN;
            LevyingBPM = Component.StandardBPM;
            BPM = LevyingBPM;
            Length = 0.0;
            TotalNotes = 0;
            AutoableNotes = 0;
            LongNotes = 0;
            TrapNotes = 0;
            JudgmentStage = 5.0;
            HitPointsValue = 0.01;
            LevelValue = BaseNoteFile.Level.Level0;
            NoteDrawingName = string.Empty;
            NoteDrawingPath = string.Empty;
            BannerDrawingName = string.Empty;
            BannerDrawingPath = string.Empty;
            TrailerAudioName = string.Empty;
            TrailerAudioPath = string.Empty;
            AssistFileName = string.Empty;
            AudioLevyingPosition = double.NaN;
            IsSalt = false;
            Tag = string.Empty;
            LowestBPM = LevyingBPM;
            HighestBPM = LevyingBPM;
            HighestInputCount = default;
            IsHellBPM = false;
        }

        public abstract void OnCompiled();

        public abstract void OnFault(Exception e);
    }
}