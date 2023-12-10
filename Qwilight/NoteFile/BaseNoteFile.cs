using Qwilight.Compiler;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.IO;
using System.Windows.Media;

namespace Qwilight.NoteFile
{
    public abstract class BaseNoteFile : Computing
    {
        public enum Handled
        {
            Not, Clear, Band1, F = 4, HigherClear, HighestClear
        }

        public enum NoteVariety
        {
            Total, BMS, BMSON = 3, EventNote = 5
        }

        public enum Level
        {
            Level0, Level1, Level2, Level3, Level4, Level5
        }

        public static BaseNoteFile[] GetNoteFiles(string noteFilePath, DefaultEntryItem defaultEntryItem, EntryItem entryItem, int dataID)
        {
            if (noteFilePath.IsTailCaselsss(".bmson"))
            {
                return new[] { new BMSONFile(noteFilePath, defaultEntryItem, entryItem) };
            }
            else if (noteFilePath.IsTailCaselsss(".pms"))
            {
                return new[] { new PMSFile(noteFilePath, defaultEntryItem, entryItem) };
            }
            else
            {
                return new[] { new BMSFile(noteFilePath, defaultEntryItem, entryItem) };
            }
        }

        readonly string[] _judgmentMillisText = new string[6];
        string _fittedText;
        ImageSource _noteDrawing;
        ImageSource _bannerDrawing;
        bool _wantNoteDrawing = true;
        bool _wantBannerDrawing = true;
        Handled _handledValue;
        string _noteID128;
        string _noteID256;
        string _noteID512;

        public virtual int DataID => 0;

        public string FittedText
        {
            get => _fittedText;

            set => SetProperty(ref _fittedText, value, nameof(FittedText));
        }

        public virtual bool IsLogical { get; }

        public bool IsAltDrawing => LogicalDrawing != null || EssentialDrawing != null;

        public virtual ImageSource LogicalDrawing { get; }

        public virtual ImageSource EssentialDrawing { get; }

        public virtual string LogicalAudioFileName => "Default";

        public Color LevelColor => BaseUI.Instance.LevelColors[(int)LevelValue];

        public virtual ImageSource HandledWallDrawing => BaseUI.Instance.HandledWallDrawings[(int)HandledValue];

        public ImageSource NoteDrawing
        {
            get
            {
                if (_wantNoteDrawing && !string.IsNullOrEmpty(NoteDrawingPath))
                {
                    _wantNoteDrawing = false;
                    Task.Run(() =>
                    {
                        var noteDrawingPath = Utility.GetFilePath(NoteDrawingPath, Utility.FileFormatFlag.Drawing);
                        if (!string.IsNullOrEmpty(noteDrawingPath))
                        {
                            try
                            {
                                SetProperty(ref _noteDrawing, DrawingSystem.Instance.LoadDefault(noteDrawingPath, null), nameof(NoteDrawing));
                            }
                            catch
                            {
                            }
                        }
                    });
                }
                return _noteDrawing ?? DrawingSystem.Instance.DefaultDrawing.DefaultDrawing;
            }
        }

        public ImageSource BannerDrawing
        {
            get
            {
                if (_wantBannerDrawing && !string.IsNullOrEmpty(BannerDrawingPath))
                {
                    _wantBannerDrawing = false;
                    Task.Run(() =>
                    {
                        var bannerDrawingPath = Utility.GetFilePath(BannerDrawingPath, Utility.FileFormatFlag.Drawing);
                        if (!string.IsNullOrEmpty(bannerDrawingPath))
                        {
                            try
                            {
                                SetProperty(ref _bannerDrawing, DrawingSystem.Instance.LoadDefault(bannerDrawingPath, null), nameof(BannerDrawing));
                            }
                            catch
                            {
                            }
                        }
                    });
                }
                return _bannerDrawing;
            }
        }

        public string WantLevelID { get; set; } = string.Empty;

        public string HighestJudgmentMillisText
        {
            get => _judgmentMillisText[(int)Component.Judged.Highest];

            set => SetProperty(ref _judgmentMillisText[(int)Component.Judged.Highest], value, nameof(HighestJudgmentMillisText));
        }

        public string HigherJudgmentMillisText
        {
            get => _judgmentMillisText[(int)Component.Judged.Higher];

            set => SetProperty(ref _judgmentMillisText[(int)Component.Judged.Higher], value, nameof(HigherJudgmentMillisText));
        }

        public string HighJudgmentMillisText
        {
            get => _judgmentMillisText[(int)Component.Judged.High];

            set => SetProperty(ref _judgmentMillisText[(int)Component.Judged.High], value, nameof(HighJudgmentMillisText));
        }

        public string LowJudgmentMillisText
        {
            get => _judgmentMillisText[(int)Component.Judged.Low];

            set => SetProperty(ref _judgmentMillisText[(int)Component.Judged.Low], value, nameof(LowJudgmentMillisText));
        }

        public string LowerJudgmentMillisText
        {
            get => _judgmentMillisText[(int)Component.Judged.Lower];

            set => SetProperty(ref _judgmentMillisText[(int)Component.Judged.Lower], value, nameof(LowerJudgmentMillisText));
        }

        public string LowestJudgmentMillisText
        {
            get => _judgmentMillisText[(int)Component.Judged.Lowest];

            set => SetProperty(ref _judgmentMillisText[(int)Component.Judged.Lowest], value, nameof(LowestJudgmentMillisText));
        }

        public DateTime? LatestDate { get; set; }

        public int HandledCount { get; set; }

        public string FileName { get; set; }

        public bool HasNoteDrawing => _noteDrawing != null;

        public HashSet<DefaultEntryItem> FavoriteEntryItems { get; } = new();

        public DefaultEntryItem DefaultEntryItem { get; }

        public string NoteFilePath { get; }

        public EntryItem EntryItem { get; }

        public Handled HandledValue
        {
            get => _handledValue;

            set => SetProperty(ref _handledValue, value, nameof(HandledWallDrawing));
        }

        public Action OnLevyNoteFile { get; init; }

        public BaseNoteFile(string noteFilePath, DefaultEntryItem defaultEntryItem, EntryItem entryItem)
        {
            NoteFilePath = noteFilePath;
            FileName = Path.GetFileName(NoteFilePath);
            DefaultEntryItem = defaultEntryItem;
            EntryItem = entryItem;
            LongNoteModeDate = Component.LatestLongNoteModeDate;
        }

        public override bool Equals(object obj) => obj is BaseNoteFile noteFile && GetNoteID512() == noteFile.GetNoteID512();

        public override int GetHashCode() => GetNoteID512().GetHashCode();

        public void SetJudgmentMillisTexts(ModeComponent modeComponentValue)
        {
            HighestJudgmentMillisText = GetMillisText(Component.Judged.Highest);
            HigherJudgmentMillisText = GetMillisText(Component.Judged.Higher);
            HighJudgmentMillisText = GetMillisText(Component.Judged.High);
            LowJudgmentMillisText = GetMillisText(Component.Judged.Low);
            LowerJudgmentMillisText = GetMillisText(Component.Judged.Lower);
            LowestJudgmentMillisText = GetMillisText(Component.Judged.Lowest);

            string GetMillisText(Component.Judged judged)
            {
                var judgment0 = Math.Round(Component.GetJudgmentMillis(judged, modeComponentValue, JudgmentStage, Component.LatestJudgmentModeDate, Component.LatestJudgmentMapDate, Component.LatestLongNoteAssistDate, 0), 3);
                var judgment1 = Math.Round(Component.GetJudgmentMillis(judged, modeComponentValue, JudgmentStage, Component.LatestJudgmentModeDate, Component.LatestJudgmentMapDate, Component.LatestLongNoteAssistDate, 1), 3);
                return Math.Abs(judgment0) != Math.Abs(judgment1) ? $"{judgment0} ~ {judgment1} ms" : $"{judgment1} ms";
            }
        }

        public void SetConfigure()
        {
            var (audioWait, mediaWait, media) = DB.Instance.GetWait(this);
            Configure.Instance.AudioWait = audioWait;
            Configure.Instance.MediaWait = mediaWait;
            Configure.Instance.Media = media != false;
            Configure.Instance.NoteFormatID = DB.Instance.GetFormat(this);
        }

        public virtual string GetNoteID128(byte[] data = null)
        {
            if (_noteID128 == null)
            {
                try
                {
                    _noteID128 = Utility.GetID128(data ?? GetContents());
                }
                catch
                {
                    _noteID128 = string.Empty;
                }
            }
            return _noteID128;
        }

        public virtual string GetNoteID256(byte[] data = null)
        {
            if (_noteID256 == null)
            {
                try
                {
                    _noteID256 = Utility.GetID256(data ?? GetContents());
                }
                catch
                {
                    _noteID256 = string.Empty;
                }
            }
            return _noteID256;
        }

        public string GetNoteID512(byte[] data = null)
        {
            if (_noteID512 == null)
            {
                _noteID512 = $"{GetNoteID512Impl(data)}:{DataID}";
            }
            return _noteID512;
        }

        public void SetData()
        {
            FavoriteEntryItems.Clear();
            foreach (var favoriteEntryItem in DB.Instance.GetFavoriteEntryItems(this))
            {
                FavoriteEntryItems.Add(favoriteEntryItem);
            }
            HandledValue = DB.Instance.GetHandled(this);
            (LatestDate, HandledCount) = DB.Instance.GetDate(this, default);
        }

        string GetNoteID512Impl(byte[] data = null)
        {
            try
            {
                return Utility.GetID512(data ?? GetContents());
            }
            catch
            {
                return string.Empty;
            }
        }

        public byte[] GetContents()
        {
            try
            {
                return File.Exists(NoteFilePath) ? File.ReadAllBytes(NoteFilePath) : Array.Empty<byte>();
            }
            catch
            {
                return Array.Empty<byte>();
            }
        }

        public void SetNoteIDs(string noteID128, string noteID256, string noteID512)
        {
            _noteID128 = noteID128;
            _noteID256 = noteID256;
            _noteID512 = noteID512;
        }

        public void Compile(int salt, CancellationTokenSource setCancelCompiler = null)
        {
            if (!IsLogical)
            {
                var data = GetContents();
                GetNoteID128(data);
                GetNoteID256(data);
                GetNoteID512(data);
                InitCompiled();
                BaseCompiler.GetCompiler(this, setCancelCompiler).Compile(this, data, salt);
            }
        }

        public bool IsValid() => GetNoteID512() == $"{GetNoteID512Impl()}:{DataID}";

        public bool IsContinuous(BaseNoteFile noteFile) => noteFile != null && (EntryItem == noteFile.EntryItem || Utility.GetTitle(Title) == Utility.GetTitle(noteFile.Title));

        public override void OnFault(Exception e) => throw e;

        public override void OnCompiled() => FastDB.Instance.SetNoteFile(this);
    }
}