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
            Not, Clear, Band1, Total, F, HigherClear, HighestClear
        }

        public enum NoteVariety
        {
            Total, BMS, BMSON = 3, EventNote = 5
        }

        public enum Level
        {
            Level0, Level1, Level2, Level3, Level4, Level5
        }

        public static IEnumerable<BaseNoteFile> GetNoteFiles(string noteFilePath, DefaultEntryItem defaultEntryItem, EntryItem entryItem, int dataID)
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
        string _save128;
        string _save256;
        string _save512;

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
                        var noteDrawingPath = Utility.GetAvailable(NoteDrawingPath, Utility.AvailableFlag.Drawing);
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
                        var bannerDrawingPath = Utility.GetAvailable(BannerDrawingPath, Utility.AvailableFlag.Drawing);
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

        public int DataID { get; set; }

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

            set => SetProperty(ref _handledValue, value, nameof(HandledValue));
        }

        public Action OnLevyNoteFile { get; init; }

        public BaseNoteFile(string noteFilePath, DefaultEntryItem defaultEntryItem, EntryItem entryItem)
        {
            NoteFilePath = noteFilePath;
            FileName = Path.GetFileName(NoteFilePath);
            DefaultEntryItem = defaultEntryItem;
            EntryItem = entryItem;
            LongNoteModeDate = Component.LatestLongNoteMode;
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
                var judgment0 = Math.Round(Component.GetJudgmentMillis(judged, modeComponentValue, JudgmentStage, Component.LatestJudgmentMode, Component.LatestJudgmentMap, Component.LatestLongNoteAssist, 0), 3);
                var judgment1 = Math.Round(Component.GetJudgmentMillis(judged, modeComponentValue, JudgmentStage, Component.LatestJudgmentMode, Component.LatestJudgmentMap, Component.LatestLongNoteAssist, 1), 3);
                return Math.Abs(judgment0) != Math.Abs(judgment1) ? $"{judgment0} ~ {judgment1} ms" : $"{judgment1} ms";
            }
        }

        public async Task SetConfigure()
        {
            var (audioWait, mediaWait, media) = await DB.Instance.GetWait(this);
            Configure.Instance.AudioWait = audioWait;
            Configure.Instance.MediaWait = mediaWait;
            Configure.Instance.Media = media != false;
            Configure.Instance.NoteFormatID = await DB.Instance.GetFormat(this);
        }

        public virtual string GetNoteID128(byte[] data = null)
        {
            if (_save128 == null)
            {
                try
                {
                    _save128 = $"{Utility.GetID128s(data ?? GetContents())}:{DataID}";
                }
                catch
                {
                    _save128 = string.Empty;
                }
            }
            return _save128;
        }

        public virtual string GetNoteID256(byte[] data = null)
        {
            if (_save256 == null)
            {
                try
                {
                    _save256 = $"{Utility.GetID256s(data ?? GetContents())}:{DataID}";
                }
                catch
                {
                    _save256 = string.Empty;
                }
            }
            return _save256;
        }

        public string GetNoteID512(byte[] data = null)
        {
            if (_save512 == null)
            {
                SetNoteID512(null, data);
            }
            return _save512;
        }

        void SetNoteID512(string noteID512 = null, byte[] data = null)
        {
            _save512 = noteID512 ?? $"{GetNoteID512Impl(data)}:{DataID}";
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
                return Utility.GetID512s(data ?? GetContents());
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
            _save128 = noteID128;
            _save256 = noteID256;
            SetNoteID512(noteID512);
        }

        public void SetData(int salt, CancellationTokenSource setCancelCompiler = null)
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