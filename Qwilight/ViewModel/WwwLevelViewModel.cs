using CommunityToolkit.Mvvm.Input;
using Qwilight.Compute;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.Data.SQLite;
using System.Net;
using System.Security.Cryptography;

namespace Qwilight.ViewModel
{
    public sealed partial class WwwLevelViewModel : BaseViewModel
    {
        public sealed class WwwLevelComputing : BaseNoteFile
        {
            public override BaseNoteFile.NoteVariety NoteVarietyValue { get; }

            public override void OnCompiled()
            {
            }

            public override void OnFault(Exception e)
            {
            }

            public bool HaveIt { get; }

            public WwwLevelComputing(JSON.Computing data) : base(ViewModels.Instance.MainValue.NoteID512s.GetValueOrDefault(data.noteID)?.NoteFilePath, ViewModels.Instance.MainValue.NoteID512s.GetValueOrDefault(data.noteID)?.DefaultEntryItem, ViewModels.Instance.MainValue.NoteID512s.GetValueOrDefault(data.noteID)?.EntryItem)
            {
                SetNoteIDs(null, null, data.noteID);
                HaveIt = ViewModels.Instance.MainValue.NoteID512s.TryGetValue(GetNoteID512(), out var noteFile);
                if (HaveIt)
                {
                    NoteVarietyValue = noteFile.NoteVarietyValue;
                    Title = noteFile.Title;
                    Artist = noteFile.Artist;
                    Genre = noteFile.Genre;
                    LevelValue = noteFile.LevelValue;
                    LevelText = noteFile.LevelText;
                    NoteDrawingPath = noteFile.NoteDrawingPath;
                    BannerDrawingPath = noteFile.BannerDrawingPath;
                }
                else
                {
                    NoteVarietyValue = data.noteVariety;
                    if (NoteVarietyValue == BaseNoteFile.NoteVariety.EventNote)
                    {
                        Title = new('❌', 1 + RandomNumberGenerator.GetInt32(10));
                        Artist = new('❌', 1 + RandomNumberGenerator.GetInt32(10));
                        Genre = new('❌', 1 + RandomNumberGenerator.GetInt32(10));
                        LevelText = new('❌', 1 + RandomNumberGenerator.GetInt32(10));
                    }
                    else
                    {
                        Title = data.title;
                        Artist = data.artist;
                        Genre = data.genre;
                        LevelValue = data.level;
                        LevelText = data.levelText;
                    }
                }
                HandledValue = data.handled;
            }
        }

        readonly WwwLevelData _wwwLevelData = new();
        readonly double[] _audioMultipliers = new double[2];
        WwwLevelAvatar? _wwwLevelAvatar;
        WwwLevelAvatar? _lastWwwLevelAvatar;
        string _levelName;
        string _lastLevelName;
        bool _isAvatarIDsLoading;
        bool _isLevelNamesLoading;
        bool _isLevelNameLoading;
        WwwLevelItem _wwwLevelItem;
        WwwLevelItem _lastWwwLevelItem;
        string _levelID;
        string _title;
        string _comment;
        string _drawing;
        bool _allowPause;
        bool _isLevelItemLoading;

        [RelayCommand]
        void OnWwwLevelTest() => WwwLevelAvatarValue = null;

        [RelayCommand]
        void OnGetWwwLevel()
        {
            try
            {
                DB.Instance.SetEventNoteData(WwwLevelComputingCollection);
                DB.Instance.SetEventNote(GetEventNoteID(), WwwLevelItemValue.Title, DateTime.Now, DB.EventNoteVariety.Qwilight);
                var mainViewModel = ViewModels.Instance.MainValue;
                mainViewModel.LoadEventNoteEntryItems();
                OnPropertyChanged(nameof(HasEventNote));
                mainViewModel.Want();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.OpenedEventNotes);
            }
            catch (SQLiteException)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.BeforeEventNoteContents);
            }
        }

        [RelayCommand]
        void OnLevyWwwLevel()
        {
            ViewModels.Instance.WwwLevelValue.Close();
            var mainViewModel = ViewModels.Instance.MainValue;
            var defaultModeComponentValue = mainViewModel.ModeComponentValue.Clone();
            var modeComponent = mainViewModel.ModeComponentValue;
            if (!IsAutoModeCompatible)
            {
                modeComponent.AutoModeValue = AutoModes.First().Value;
            }
            if (!IsNoteSaltModeCompatible)
            {
                modeComponent.NoteSaltModeValue = NoteSaltModes.First().Value;
            }
            modeComponent.AudioMultiplier = Math.Clamp(modeComponent.AudioMultiplier, _audioMultipliers[0], _audioMultipliers[1]);
            if (!IsFaintNoteModeCompatible)
            {
                modeComponent.FaintNoteModeValue = FaintNoteModes.First().Value;
            }
            if (!IsJudgmentModeCompatible)
            {
                modeComponent.JudgmentModeValue = JudgmentModes.First().Value;
            }
            if (!IsHitPointsModeCompatible)
            {
                modeComponent.HitPointsModeValue = HitPointsModes.First().Value;
            }
            if (!IsNoteMobilityModeCompatible)
            {
                modeComponent.NoteMobilityModeValue = NoteMobilityModes.First().Value;
            }
            if (!IsLongNoteModeCompatible)
            {
                modeComponent.LongNoteModeValue = LongNoteModes.First().Value;
            }
            if (!IsInputFavorModeCompatible)
            {
                modeComponent.InputFavorModeValue = InputFavorModes.First().Value;
            }
            if (!IsNoteModifyModeCompatible)
            {
                modeComponent.NoteModifyModeValue = NoteModifyModes.First().Value;
            }
            if (!IsBPMModeCompatible)
            {
                modeComponent.BPMModeValue = BPMModes.First().Value;
            }
            if (!IsWaveModeCompatible)
            {
                modeComponent.WaveModeValue = WaveModes.First().Value;
            }
            if (!IsSetNoteModeCompatible)
            {
                modeComponent.SetNoteModeValue = SetNoteModes.First().Value;
            }
            if (!IsLowestJudgmentConditionModeCompatible)
            {
                modeComponent.LowestJudgmentConditionModeValue = LowestJudgmentConditionModes.First().Value;
            }
            modeComponent.PutCopyNotesValueV2 = ModeComponent.PutCopyNotes.Default;
            var entryItem = mainViewModel.EventNoteEntryItems[GetEventNoteID()];
            mainViewModel.HandleLevyNoteFile(entryItem.NoteFile, entryItem, _wwwLevelData, defaultModeComponentValue);
        }

        string GetEventNoteID() => string.Join('/', WwwLevelComputingCollection.Select(wwwLevelComputing => wwwLevelComputing.GetNoteID512()));

        public bool HasEventNote => ViewModels.Instance.MainValue.EventNoteEntryItems.ContainsKey(GetEventNoteID());

        public bool IsLevelItemLoading
        {
            get => _isLevelItemLoading;

            set => SetProperty(ref _isLevelItemLoading, value, nameof(IsLevelItemLoading));
        }

        public ObservableCollection<WwwLevelItem> WwwLevelItemCollection { get; } = new();

        public ObservableCollection<WwwLevelComputing> WwwLevelComputingCollection { get; } = new();

        public ObservableCollection<AvatarEdgeItem> ClearedEdgeItemCollection { get; } = new();

        public ObservableCollection<AvatarTitleItem> ClearedTitleItemCollection { get; } = new();

        public bool HasClearedEdgeItem => ClearedEdgeItemCollection.Count > 0;

        public bool HasClearedTitleItem => ClearedTitleItemCollection.Count > 0;

        public string LevelID
        {
            get => _levelID;

            set => SetProperty(ref _levelID, value, nameof(LevelID));
        }

        public string Title
        {
            get => _title;

            set => SetProperty(ref _title, value, nameof(Title));
        }

        public string Comment
        {
            get => _comment;

            set => SetProperty(ref _comment, value, nameof(Comment));
        }

        public string Drawing
        {
            get => _drawing;

            set => SetProperty(ref _drawing, value, nameof(Drawing));
        }

        public bool AllowPause
        {
            get => _allowPause;

            set => SetProperty(ref _allowPause, value, nameof(AllowPause));
        }

        public string[] StandContents { get; } = new string[2];

        public string[] PointContents { get; } = new string[2];

        public string[] BandContents { get; } = new string[2];

        public string[] AudioMultiplierContents { get; } = new string[2];

        public string[][] JudgmentContents { get; } = new string[6][];

        public bool IsAutoModeCompatible => AutoModes.Count == 0 || AutoModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.AutoModeValue);

        public bool IsNoteSaltModeCompatible => NoteSaltModes.Count == 0 || NoteSaltModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.NoteSaltModeValue);

        public bool IsFaintNoteModeCompatible => FaintNoteModes.Count == 0 || FaintNoteModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.FaintNoteModeValue);

        public bool IsJudgmentModeCompatible => JudgmentModes.Count == 0 || JudgmentModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.JudgmentModeValue);

        public bool IsHitPointsModeCompatible => HitPointsModes.Count == 0 || HitPointsModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.HitPointsModeValue);

        public bool IsNoteMobilityModeCompatible => NoteMobilityModes.Count == 0 || NoteMobilityModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.NoteMobilityModeValue);

        public bool IsLongNoteModeCompatible => LongNoteModes.Count == 0 || LongNoteModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.LongNoteModeValue);

        public bool IsInputFavorModeCompatible => InputFavorModes.Count == 0 || InputFavorModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.InputFavorModeValue);

        public bool IsNoteModifyModeCompatible => NoteModifyModes.Count == 0 || NoteModifyModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.NoteModifyModeValue);

        public bool IsBPMModeCompatible => BPMModes.Count == 0 || BPMModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.BPMModeValue);

        public bool IsWaveModeCompatible => WaveModes.Count == 0 || WaveModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.WaveModeValue);

        public bool IsSetNoteModeCompatible => SetNoteModes.Count == 0 || SetNoteModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.SetNoteModeValue);

        public bool IsLowestJudgmentConditionModeCompatible => LowestJudgmentConditionModes.Count == 0 || LowestJudgmentConditionModes.Any(wwwLevelModeComponent => wwwLevelModeComponent.Value == ViewModels.Instance.MainValue.ModeComponentValue.LowestJudgmentConditionModeValue);

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.AutoMode>> AutoModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.NoteSaltMode>> NoteSaltModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.FaintNoteMode>> FaintNoteModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.JudgmentMode>> JudgmentModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.HitPointsMode>> HitPointsModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.NoteMobilityMode>> NoteMobilityModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.LongNoteMode>> LongNoteModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.InputFavorMode>> InputFavorModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.NoteModifyMode>> NoteModifyModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.BPMMode>> BPMModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.WaveMode>> WaveModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.SetNoteMode>> SetNoteModes { get; } = new();

        public ObservableCollection<WwwLevelModeComponent<ModeComponent.LowestJudgmentConditionMode>> LowestJudgmentConditionModes { get; } = new();

        public ObservableCollection<WwwLevelClearedAvatar> ClearedAvatars { get; } = new();

        public WwwLevelItem WwwLevelItemValue
        {
            get => _wwwLevelItem;

            set
            {
                if (SetProperty(ref _wwwLevelItem, value, nameof(WwwLevelItemValue)) && value != null)
                {
                    _lastWwwLevelItem = value;
                    _ = Awaitable();
                    async Task Awaitable()
                    {
                        var levelID = value.LevelID;
                        IsLevelItemLoading = true;
                        var twilightWwwLevel = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwLevel?>($"{QwilightComponent.QwilightAPI}/level?levelID={levelID}&language={Configure.Instance.Language}");
                        if (twilightWwwLevel.HasValue && WwwLevelItemValue?.LevelID == levelID)
                        {
                            var twilightWwwLevelValue = twilightWwwLevel.Value;

                            LevelID = value.LevelID;
                            Title = value.Title;
                            Comment = value.Comment;
                            Drawing = $"{QwilightComponent.QwilightAPI}/drawing?levelID={LevelID}";

                            Array.Fill(StandContents, null);
                            if (twilightWwwLevelValue.stand != null)
                            {
                                if (twilightWwwLevelValue.stand[0] != -1)
                                {
                                    StandContents[0] = string.Format(LanguageSystem.Instance.WwwLevelStandContents0, twilightWwwLevelValue.stand[0].ToString("#,##0"));
                                }
                                if (twilightWwwLevelValue.stand[1] != -1)
                                {
                                    StandContents[1] = string.Format(LanguageSystem.Instance.WwwLevelStandContents1, twilightWwwLevelValue.stand[1].ToString("#,##0"));
                                }
                            }
                            OnPropertyChanged(nameof(StandContents));
                            _wwwLevelData.Stand = twilightWwwLevelValue.stand;
                            _wwwLevelData.StandContents = string.Join(" ", StandContents).Trim();

                            Array.Fill(PointContents, null);
                            if (twilightWwwLevelValue.point != null)
                            {
                                if (twilightWwwLevelValue.point[0] != -1)
                                {
                                    PointContents[0] = string.Format(LanguageSystem.Instance.WwwLevelPointContents0, 100.0 * twilightWwwLevelValue.point[0]);
                                }
                                if (twilightWwwLevelValue.point[1] != -1)
                                {
                                    PointContents[1] = string.Format(LanguageSystem.Instance.WwwLevelPointContents1, 100.0 * twilightWwwLevelValue.point[1]);
                                }
                            }
                            OnPropertyChanged(nameof(PointContents));
                            _wwwLevelData.Point = twilightWwwLevelValue.point;
                            _wwwLevelData.PointContents = string.Join(" ", PointContents).Trim();

                            Array.Fill(BandContents, null);
                            if (twilightWwwLevelValue.band != null)
                            {
                                if (twilightWwwLevelValue.band[0] != -1)
                                {
                                    BandContents[0] = string.Format(LanguageSystem.Instance.WwwLevelBandContents0, twilightWwwLevelValue.band[0]);
                                }
                                if (twilightWwwLevelValue.band[1] != -1)
                                {
                                    BandContents[1] = string.Format(LanguageSystem.Instance.WwwLevelBandContents1, twilightWwwLevelValue.band[1]);
                                }
                            }
                            OnPropertyChanged(nameof(BandContents));
                            _wwwLevelData.Band = twilightWwwLevelValue.band;
                            _wwwLevelData.BandContents = string.Join(" ", BandContents).Trim();

                            Array.Fill(AudioMultiplierContents, null);
                            if (twilightWwwLevelValue.audioMultiplier != null)
                            {
                                if (twilightWwwLevelValue.audioMultiplier[0] != -1)
                                {
                                    AudioMultiplierContents[0] = string.Format(LanguageSystem.Instance.WwwLevelAudioMultiplierContents0, twilightWwwLevelValue.audioMultiplier[0].ToString("0.00"));
                                    _audioMultipliers[0] = twilightWwwLevelValue.audioMultiplier[0];
                                }
                                else
                                {
                                    _audioMultipliers[0] = double.NegativeInfinity;
                                }
                                if (twilightWwwLevelValue.audioMultiplier[1] != -1)
                                {
                                    AudioMultiplierContents[1] = string.Format(LanguageSystem.Instance.WwwLevelAudioMultiplierContents1, twilightWwwLevelValue.audioMultiplier[1].ToString("0.00"));
                                    _audioMultipliers[1] = twilightWwwLevelValue.audioMultiplier[1];
                                }
                                else
                                {
                                    _audioMultipliers[1] = double.PositiveInfinity;
                                }
                            }
                            OnPropertyChanged(nameof(AudioMultiplierContents));
                            _wwwLevelData.LowestAudioMultiplier = (twilightWwwLevelValue.audioMultiplier?[0] == -1.0 ? null : twilightWwwLevelValue.audioMultiplier?[0] as double?) ?? 0.5;
                            _wwwLevelData.HighestAudioMultiplier = (twilightWwwLevelValue.audioMultiplier?[1] == -1.0 ? null : twilightWwwLevelValue.audioMultiplier?[1] as double?) ?? 2.0;

                            Array.Fill(JudgmentContents, null);
                            if (twilightWwwLevelValue.judgments != null)
                            {
                                for (var i = twilightWwwLevelValue.judgments.Length - 1; i >= 0; --i)
                                {
                                    if (twilightWwwLevelValue.judgments[i] != null)
                                    {
                                        JudgmentContents[i] = new string[twilightWwwLevelValue.judgments[i].Length];
                                        if (twilightWwwLevelValue.judgments[i][0] != -1)
                                        {
                                            JudgmentContents[i][0] = string.Format(LanguageSystem.Instance.WwwLevelJudgmentContents0, twilightWwwLevelValue.judgments[i][0]);
                                        }
                                        if (twilightWwwLevelValue.judgments[i][1] != -1)
                                        {
                                            JudgmentContents[i][1] = string.Format(LanguageSystem.Instance.WwwLevelJudgmentContents1, twilightWwwLevelValue.judgments[i][1]);
                                        }
                                    }
                                }
                            }
                            OnPropertyChanged(nameof(JudgmentContents));
                            _wwwLevelData.Judgments = twilightWwwLevelValue.judgments;
                            _wwwLevelData.JudgmentContents[(int)Component.Judged.Highest] = JudgmentContents[(int)Component.Judged.Highest] != null ? $"Yell! {string.Join(" ", JudgmentContents[(int)Component.Judged.Highest]).Trim()}" : null;
                            _wwwLevelData.JudgmentContents[(int)Component.Judged.Higher] = JudgmentContents[(int)Component.Judged.Higher] != null ? $"Yell {string.Join(" ", JudgmentContents[(int)Component.Judged.Higher]).Trim()}" : null;
                            _wwwLevelData.JudgmentContents[(int)Component.Judged.High] = JudgmentContents[(int)Component.Judged.High] != null ? $"Cool {string.Join(" ", JudgmentContents[(int)Component.Judged.High]).Trim()}" : null;
                            _wwwLevelData.JudgmentContents[(int)Component.Judged.Low] = JudgmentContents[(int)Component.Judged.Low] != null ? $"Good {string.Join(" ", JudgmentContents[(int)Component.Judged.Low]).Trim()}" : null;
                            _wwwLevelData.JudgmentContents[(int)Component.Judged.Lower] = JudgmentContents[(int)Component.Judged.Lower] != null ? $"Poor {string.Join(" ", JudgmentContents[(int)Component.Judged.Lower]).Trim()}" : null;
                            _wwwLevelData.JudgmentContents[(int)Component.Judged.Lowest] = JudgmentContents[(int)Component.Judged.Lowest] != null ? $"Failed {string.Join(" ", JudgmentContents[(int)Component.Judged.Lowest]).Trim()}" : null;

                            AllowPause = twilightWwwLevelValue.allowPause;
                            _wwwLevelData.AllowPause = twilightWwwLevelValue.allowPause;

                            var mainViewModel = ViewModels.Instance.MainValue;
                            var siteContainerViewModel = ViewModels.Instance.SiteContainerValue;
                            var toModifyModeComponentViewModel = ViewModels.Instance.ModifyModeComponentValue;

                            WwwLevelComputingCollection.Clear();
                            foreach (var data in twilightWwwLevelValue.levelNote)
                            {
                                WwwLevelComputingCollection.Add(new(data));
                            }
                            OnPropertyChanged(nameof(HasEventNote));

                            AutoModes.Clear();
                            if (twilightWwwLevelValue.autoMode != null)
                            {
                                foreach (var autoMode in twilightWwwLevelValue.autoMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.AutoModeVariety].FindIndex(modeComponent => (ModeComponent.AutoMode)modeComponent.Value == value)))
                                {
                                    AutoModes.Add(new()
                                    {
                                        Value = autoMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.AutoModeValue = autoMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            OnPropertyChanged(nameof(IsAutoModeCompatible));
                                        })
                                    });
                                }
                            }

                            NoteSaltModes.Clear();
                            if (twilightWwwLevelValue.noteSaltMode != null)
                            {
                                foreach (var noteSaltMode in twilightWwwLevelValue.noteSaltMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.NoteSaltModeVariety].FindIndex(modeComponent => (ModeComponent.NoteSaltMode)modeComponent.Value == value)))
                                {
                                    NoteSaltModes.Add(new()
                                    {
                                        Value = noteSaltMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.NoteSaltModeValue = noteSaltMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            mainViewModel.HandleAutoComputer();
                                            OnPropertyChanged(nameof(IsNoteSaltModeCompatible));
                                        })
                                    });
                                }
                            }

                            FaintNoteModes.Clear();
                            if (twilightWwwLevelValue.faintNoteMode != null)
                            {
                                foreach (var faintNoteMode in twilightWwwLevelValue.faintNoteMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.FaintNoteModeVariety].FindIndex(modeComponent => (ModeComponent.FaintNoteMode)modeComponent.Value == value)))
                                {
                                    FaintNoteModes.Add(new()
                                    {
                                        Value = faintNoteMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.FaintNoteModeValue = faintNoteMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            OnPropertyChanged(nameof(IsFaintNoteModeCompatible));
                                        })
                                    });
                                }
                            }

                            JudgmentModes.Clear();
                            if (twilightWwwLevelValue.judgmentMode != null)
                            {
                                foreach (var judgmentMode in twilightWwwLevelValue.judgmentMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.JudgmentModeVariety].FindIndex(modeComponent => (ModeComponent.JudgmentMode)modeComponent.Value == value)))
                                {
                                    JudgmentModes.Add(new()
                                    {
                                        Value = judgmentMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.JudgmentModeValue = judgmentMode;
                                            mainViewModel.OnJudgmentMeterMillisModified();
                                            siteContainerViewModel.CallSetModeComponent();
                                            OnPropertyChanged(nameof(IsJudgmentModeCompatible));
                                        })
                                    });
                                }
                            }

                            HitPointsModes.Clear();
                            if (twilightWwwLevelValue.hitPointsMode != null)
                            {
                                foreach (var hitPointsMode in twilightWwwLevelValue.hitPointsMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.HitPointsModeVariety].FindIndex(modeComponent => (ModeComponent.HitPointsMode)modeComponent.Value == value)))
                                {
                                    HitPointsModes.Add(new()
                                    {
                                        Value = hitPointsMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.HitPointsModeValue = hitPointsMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            OnPropertyChanged(nameof(IsHitPointsModeCompatible));
                                        })
                                    });
                                }
                            }

                            NoteMobilityModes.Clear();
                            if (twilightWwwLevelValue.noteMobilityMode != null)
                            {
                                foreach (var noteMobilityMode in twilightWwwLevelValue.noteMobilityMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.NoteMobilityModeVariety].FindIndex(modeComponent => (ModeComponent.NoteMobilityMode)modeComponent.Value == value)))
                                {
                                    NoteMobilityModes.Add(new()
                                    {
                                        Value = noteMobilityMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.NoteMobilityModeValue = noteMobilityMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            OnPropertyChanged(nameof(IsNoteMobilityModeCompatible));
                                        })
                                    });
                                }
                            }

                            LongNoteModes.Clear();
                            if (twilightWwwLevelValue.longNoteMode != null)
                            {
                                foreach (var longNoteMode in twilightWwwLevelValue.longNoteMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.LongNoteModeVariety].FindIndex(modeComponent => (ModeComponent.LongNoteMode)modeComponent.Value == value)))
                                {
                                    LongNoteModes.Add(new()
                                    {
                                        Value = longNoteMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.LongNoteModeValue = longNoteMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            mainViewModel.HandleAutoComputer();
                                            OnPropertyChanged(nameof(IsLongNoteModeCompatible));
                                        })
                                    });
                                }
                            }

                            InputFavorModes.Clear();
                            if (twilightWwwLevelValue.inputFavorMode != null)
                            {
                                foreach (var inputFavorMode in twilightWwwLevelValue.inputFavorMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.InputFavorModeVariety].FindIndex(modeComponent => (ModeComponent.InputFavorMode)modeComponent.Value == value)))
                                {
                                    InputFavorModes.Add(new()
                                    {
                                        Value = inputFavorMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.InputFavorModeValue = inputFavorMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            mainViewModel.HandleAutoComputer();
                                            OnPropertyChanged(nameof(IsInputFavorModeCompatible));
                                        })
                                    });
                                }
                            }

                            NoteModifyModes.Clear();
                            if (twilightWwwLevelValue.noteModifyMode != null)
                            {
                                foreach (var noteModifyMode in twilightWwwLevelValue.noteModifyMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.NoteModifyModeVariety].FindIndex(modeComponent => (ModeComponent.NoteModifyMode)modeComponent.Value == value)))
                                {
                                    NoteModifyModes.Add(new()
                                    {
                                        Value = noteModifyMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.NoteModifyModeValue = noteModifyMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            mainViewModel.HandleAutoComputer();
                                            OnPropertyChanged(nameof(IsNoteModifyModeCompatible));
                                        })
                                    });
                                }
                            }

                            BPMModes.Clear();
                            if (twilightWwwLevelValue.bpmMode != null)
                            {
                                foreach (var bpmMode in twilightWwwLevelValue.bpmMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.BPMModeVariety].FindIndex(modeComponent => (ModeComponent.BPMMode)modeComponent.Value == value)))
                                {
                                    BPMModes.Add(new()
                                    {
                                        Value = bpmMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.BPMModeValue = bpmMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            mainViewModel.HandleAutoComputer();
                                            OnPropertyChanged(nameof(IsBPMModeCompatible));
                                        })
                                    });
                                }
                            }

                            WaveModes.Clear();
                            if (twilightWwwLevelValue.waveMode != null)
                            {
                                foreach (var waveMode in twilightWwwLevelValue.waveMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.WaveModeVariety].FindIndex(modeComponent => (ModeComponent.WaveMode)modeComponent.Value == value)))
                                {
                                    WaveModes.Add(new()
                                    {
                                        Value = waveMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.WaveModeValue = waveMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            mainViewModel.HandleAutoComputer();
                                            OnPropertyChanged(nameof(IsWaveModeCompatible));
                                        })
                                    });
                                }
                            }

                            SetNoteModes.Clear();
                            if (twilightWwwLevelValue.setNoteMode != null)
                            {
                                foreach (var setNoteMode in twilightWwwLevelValue.setNoteMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.SetNoteModeVariety].FindIndex(modeComponent => (ModeComponent.SetNoteMode)modeComponent.Value == value)))
                                {
                                    SetNoteModes.Add(new()
                                    {
                                        Value = setNoteMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.SetNoteModeValue = setNoteMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            mainViewModel.HandleAutoComputer();
                                            OnPropertyChanged(nameof(IsSetNoteModeCompatible));
                                        })
                                    });
                                }
                            }

                            LowestJudgmentConditionModes.Clear();
                            if (twilightWwwLevelValue.lowestJudgmentConditionMode != null)
                            {
                                foreach (var lowestJudgmentConditionMode in twilightWwwLevelValue.lowestJudgmentConditionMode.OrderBy(value => toModifyModeComponentViewModel.ModifyModeComponentItems[ModifyModeComponentViewModel.LowestJudgmentConditionModeVariety].FindIndex(modeComponent => (ModeComponent.LowestJudgmentConditionMode)modeComponent.Value == value)))
                                {
                                    LowestJudgmentConditionModes.Add(new()
                                    {
                                        Value = lowestJudgmentConditionMode,
                                        OnInput = new(() =>
                                        {
                                            mainViewModel.ModeComponentValue.LowestJudgmentConditionModeValue = lowestJudgmentConditionMode;
                                            siteContainerViewModel.CallSetModeComponent();
                                            OnPropertyChanged(nameof(IsLowestJudgmentConditionModeCompatible));
                                        })
                                    });
                                }
                            }

                            ClearedAvatars.Clear();
                            if (twilightWwwLevelValue.avatars != null)
                            {
                                foreach (var avatar in twilightWwwLevelValue.avatars)
                                {
                                    ClearedAvatars.Add(new()
                                    {
                                        AvatarWwwValue = new(avatar.avatarID),
                                        AvatarName = avatar.avatarName,
                                        Date = DateTime.UnixEpoch.ToLocalTime().AddMilliseconds(avatar.date).ToString()
                                    });
                                }
                            }

                            Utility.SetUICollection(ClearedTitleItemCollection, twilightWwwLevelValue.titles.Select(title => new AvatarTitleItem
                            {
                                Title = title.title,
                                TitleID = title.titleID,
                                TitlePaint = Utility.GetTitlePaint(title.titleColor),
                                TitleColor = Utility.GetTitleColor(title.titleColor)
                            }).ToArray());
                            OnPropertyChanged(nameof(HasClearedTitleItem));

                            Utility.SetUICollection(ClearedEdgeItemCollection, twilightWwwLevelValue.edgeIDs.Select(data => new AvatarEdgeItem
                            {
                                EdgeID = data,
                            }).ToArray());
                            OnPropertyChanged(nameof(HasClearedEdgeItem));

                            NotifyIsCompatible();
                        }
                        IsLevelItemLoading = false;
                    }
                }
            }
        }

        public void NotifyIsCompatible()
        {
            OnPropertyChanged(nameof(IsAutoModeCompatible));
            OnPropertyChanged(nameof(IsNoteSaltModeCompatible));
            OnPropertyChanged(nameof(IsFaintNoteModeCompatible));
            OnPropertyChanged(nameof(IsJudgmentModeCompatible));
            OnPropertyChanged(nameof(IsHitPointsModeCompatible));
            OnPropertyChanged(nameof(IsNoteMobilityModeCompatible));
            OnPropertyChanged(nameof(IsLongNoteModeCompatible));
            OnPropertyChanged(nameof(IsInputFavorModeCompatible));
            OnPropertyChanged(nameof(IsNoteModifyModeCompatible));
            OnPropertyChanged(nameof(IsBPMModeCompatible));
            OnPropertyChanged(nameof(IsWaveModeCompatible));
            OnPropertyChanged(nameof(IsSetNoteModeCompatible));
            OnPropertyChanged(nameof(IsLowestJudgmentConditionModeCompatible));
        }

        public bool IsLevelNameLoading
        {
            get => _isLevelNameLoading;

            set => SetProperty(ref _isLevelNameLoading, value, nameof(IsLevelNameLoading));
        }

        public override double TargetLength => 0.9;

        public string[] HandledLevelIDs { get; set; }

        public WwwLevelAvatar? WwwLevelAvatarValue
        {
            get => _wwwLevelAvatar;

            set
            {
                if (SetProperty(ref _wwwLevelAvatar, value, nameof(WwwLevelAvatarValue)))
                {
                    _lastWwwLevelAvatar = value;
                    _lastLevelName = null;
                    _lastWwwLevelItem = null;
                    _ = Awaitable();
                    async Task Awaitable()
                    {
                        var avatarID = value?.AvatarWwwValue?.AvatarID ?? string.Empty;
                        IsLevelNamesLoading = true;
                        var levelNames = await TwilightSystem.Instance.GetWwwParallel<string[]>($"{QwilightComponent.QwilightAPI}/level?avatarID={WebUtility.UrlEncode(avatarID)}");
                        if (levelNames != null && (WwwLevelAvatarValue?.AvatarWwwValue?.AvatarID ?? string.Empty) == avatarID)
                        {
                            Utility.SetUICollection(LevelNameCollection, levelNames);
                            LevelName ??= _lastLevelName ?? LevelNameCollection.FirstOrDefault();
                        }
                        IsLevelNamesLoading = false;
                    }
                }
            }
        }

        public ObservableCollection<string> LevelNameCollection { get; } = new();

        public ObservableCollection<WwwLevelAvatar> WwwLevelAvatarCollection { get; } = new();

        public bool IsAvatarIDsLoading
        {
            get => _isAvatarIDsLoading;

            set => SetProperty(ref _isAvatarIDsLoading, value, nameof(IsAvatarIDsLoading));
        }

        public bool IsLevelNamesLoading
        {
            get => _isLevelNamesLoading;

            set => SetProperty(ref _isLevelNamesLoading, value, nameof(IsLevelNamesLoading));
        }

        public string LevelName
        {
            get => _levelName;

            set
            {
                if (SetProperty(ref _levelName, value, nameof(LevelName)) && !string.IsNullOrEmpty(value))
                {
                    _lastLevelName = value;
                    _lastWwwLevelItem = null;
                    _ = Awaitable();
                    async Task Awaitable()
                    {
                        IsLevelNameLoading = true;
                        var twilightWwwLevels = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwLevels[]>($"{QwilightComponent.QwilightAPI}/level?levelName={WebUtility.UrlEncode(value)}");
                        if (twilightWwwLevels != null && LevelName == value)
                        {
                            Utility.SetUICollection(WwwLevelItemCollection, twilightWwwLevels.Select(twilightWwwLevel =>
                            {
                                var levelID = twilightWwwLevel.levelID;
                                return new WwwLevelItem
                                {
                                    LevelID = levelID,
                                    Title = twilightWwwLevel.title,
                                    Comment = twilightWwwLevel.comment,
                                    LevelText = twilightWwwLevel.levelText,
                                    LevelValue = twilightWwwLevel.level,
                                    Handled = HandledLevelIDs.Contains(levelID),
                                    Avatars = twilightWwwLevel.avatars
                                };
                            }).ToArray());
                            WwwLevelItemValue ??= _lastWwwLevelItem ?? WwwLevelItemCollection.FirstOrDefault();
                        }
                        IsLevelNameLoading = false;
                    }
                }
            }
        }

        public override bool OpeningCondition => ViewModels.Instance.MainValue.IsNoteFileMode;

        public override void OnOpened()
        {
            base.OnOpened();
            UIHandler.Instance.HandleParallel(async () =>
            {
                IsAvatarIDsLoading = true;
                var twilightWwwLevelAvatars = await TwilightSystem.Instance.GetWwwParallel<JSON.TwilightWwwLevelAvatars?>($"{QwilightComponent.QwilightAPI}/level?avatarIDMe={WebUtility.UrlEncode(TwilightSystem.Instance.AvatarID)}");
                if (twilightWwwLevelAvatars.HasValue)
                {
                    var twilightWwwLevelAvatarsValue = twilightWwwLevelAvatars.Value;
                    HandledLevelIDs = twilightWwwLevelAvatarsValue.levelIDs;
                    Utility.SetUICollection(WwwLevelAvatarCollection, twilightWwwLevelAvatarsValue.avatars.Select(avatar => new WwwLevelAvatar
                    {
                        AvatarWwwValue = new(avatar.avatarID),
                        AvatarName = avatar.avatarName
                    }).ToArray());
                    WwwLevelAvatarValue ??= _lastWwwLevelAvatar ?? WwwLevelAvatarCollection.FirstOrDefault();
                }
                IsAvatarIDsLoading = false;
            });
        }

        public override void OnCollasped()
        {
            base.OnCollasped();
            ViewModels.Instance.MainValue.Want();
        }
    }
}