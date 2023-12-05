using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Text.Json.Serialization;
using Windows.UI;

namespace Qwilight
{
    public sealed class ModeComponent : Model
    {
        public enum AutoMode
        {
            Default, Autoable
        }

        public enum NoteSaltMode
        {
            Default, Symmetric, Salt, InputSalt = 4, MeterSalt = 11, HalfInputSalt = 13
        }

        public enum FaintNoteMode
        {
            Default, Faint, Fading, TotalFading
        }

        public enum JudgmentMode
        {
            Lower, Default, Higher, Lowest, Highest, Favor
        }

        public enum HitPointsMode
        {
            Lower, Default, Higher, Failed, Lowest, Highest, Favor, Test
        }

        public enum NoteMobilityMode
        {
            Default, _4DHD, ZipHD = 3, _4D, Zip
        }

        public enum LongNoteMode
        {
            Default, Auto, Input
        }

        public enum InputFavorMode
        {
            Default, Mode4 = 4, Mode5, Mode6, Mode7, Mode8, Mode9, Mode51, Mode71, Mode102, Mode142, Mode10, Mode242, Mode484
        }

        public enum NoteModifyMode
        {
            Default, InputNote, LongNote
        }

        public enum BPMMode
        {
            Default, Not
        }

        public enum WaveMode
        {
            Default, Counter
        }

        public enum SetNoteMode
        {
            Default, Put, VoidPut = 3
        }

        public enum LowestJudgmentConditionMode
        {
            Default, Wrong
        }

        public enum PutCopyNotes
        {
            Default, Copy, P1Symmetric, P2Symmetric
        }

        int _salt = Environment.TickCount;
        bool _valueCanModifyMultiplier = true;
        bool _valueCanModifyAudioMultiplier = true;
        Computing _valueComputing;
        double _multiplierValue = 1000.0;
        double _sentMultiplier = 1.0;
        AutoMode _autoMode;
        NoteSaltMode _noteSaltMode;
        double _audioMultiplier = 1.0;
        FaintNoteMode _faintNoteMode;
        JudgmentMode _judgmentMode = JudgmentMode.Default;
        HitPointsMode _hitPointsMode = HitPointsMode.Highest;
        NoteMobilityMode _noteMobilityMode;
        LongNoteMode _longNoteMode;
        InputFavorMode _inputFavorMode;
        NoteModifyMode _noteModifyMode;
        BPMMode _bpmMode;
        WaveMode _waveMode;
        SetNoteMode _setNoteMode;
        LowestJudgmentConditionMode _lowestJudgmentConditionMode;
        PutCopyNotes _putCopyNotes;
        double _multiplierUnit = 0.01;
        double _lowestLongNoteModify = 100.0;
        double _highestLongNoteModify = 100.0;
        double _putNoteSet = 25.0;
        double _putNoteSetMillis = 100.0;

        public bool IsNoteSaltModeWarning(string date) => Utility.IsLowerDate(Version.Parse(date), 1, 16, 11) && (NoteSaltModeValue == NoteSaltMode.InputSalt || NoteSaltModeValue == NoteSaltMode.MeterSalt || NoteSaltModeValue == NoteSaltMode.HalfInputSalt);

        public bool IsGASWarning => HitPointsModeValue == HitPointsMode.Default || HitPointsModeValue == HitPointsMode.Higher;

        public Component ComponentValue { get; }

        public ModeComponent()
        {
            ComponentValue = new Component(BPM);
        }

        public ModeComponent(Computing valueComputing, JSON.TwilightQuitNet.QuitNetItem quitNetItem)
        {
            ComponentValue = new Component(quitNetItem.bpm);
            ComputingValue = valueComputing;
            AutoModeValue = (AutoMode)quitNetItem.autoMode;
            NoteSaltModeValue = (NoteSaltMode)quitNetItem.noteSaltMode;
            AudioMultiplier = quitNetItem.audioMultiplier;
            FaintNoteModeValue = (FaintNoteMode)quitNetItem.faintNoteMode;
            JudgmentModeValue = (JudgmentMode)quitNetItem.judgmentMode;
            HitPointsModeValue = (HitPointsMode)quitNetItem.hitPointsMode;
            NoteMobilityModeValue = (NoteMobilityMode)quitNetItem.noteMobilityMode;
            LongNoteModeValue = (LongNoteMode)quitNetItem.longNoteMode;
            InputFavorModeValue = (InputFavorMode)quitNetItem.inputFavorMode;
            NoteModifyModeValue = (NoteModifyMode)quitNetItem.noteModifyMode;
            BPMModeValue = (BPMMode)quitNetItem.bpmMode;
            WaveModeValue = (WaveMode)quitNetItem.waveMode;
            SetNoteModeValue = (SetNoteMode)quitNetItem.setNoteMode;
            LowestJudgmentConditionModeValue = (LowestJudgmentConditionMode)quitNetItem.lowestJudgmentConditionMode;
            SentMultiplier = quitNetItem.multiplier;
            MultiplierValue = BPM * AudioMultiplier * SentMultiplier;
            HighestJudgment0 = quitNetItem.highestJudgment0;
            HigherJudgment0 = quitNetItem.higherJudgment0;
            HighJudgment0 = quitNetItem.highJudgment0;
            LowJudgment0 = quitNetItem.lowJudgment0;
            LowerJudgment0 = quitNetItem.lowerJudgment0;
            LowestJudgment0 = quitNetItem.lowestJudgment0;
            HighestJudgment1 = quitNetItem.highestJudgment1;
            HigherJudgment1 = quitNetItem.higherJudgment1;
            HighJudgment1 = quitNetItem.highJudgment1;
            LowJudgment1 = quitNetItem.lowJudgment1;
            LowerJudgment1 = quitNetItem.lowerJudgment1;
            LowestJudgment1 = quitNetItem.lowestJudgment1;
        }

        public PutCopyNotes PutCopyNotesValueV2
        {
            get => _putCopyNotes;

            set
            {
                if (SetProperty(ref _putCopyNotes, value, nameof(PutCopyNotesValueV2)))
                {
                    OnPropertyChanged(nameof(PutCopyNotesText));
                    OnPropertyChanged(nameof(PutCopyNotesAvailable));
                    OnPropertyChanged(nameof(CanBeTwilightCommentContents));
                }
            }
        }

        public bool PutCopyNotesAvailable => PutCopyNotesValueV2 != PutCopyNotes.Default;

        public string PutCopyNotesText => PutCopyNotesValueV2 switch
        {
            PutCopyNotes.Default => LanguageSystem.Instance.DefaultPutCopyNotesText,
            PutCopyNotes.Copy => LanguageSystem.Instance.CopyPutCopyNotesText,
            PutCopyNotes.P1Symmetric => LanguageSystem.Instance.P1SymmetricCopyNotesText,
            PutCopyNotes.P2Symmetric => LanguageSystem.Instance.P2SymmetricCopyNotesText,
            _ => default
        };

        public bool CanBeTwilightComment => JudgmentModeValue != JudgmentMode.Favor &&
            HandlingHitPointsModeValue != HitPointsMode.Favor && HandlingHitPointsModeValue != HitPointsMode.Test &&
            LongNoteModeValue != LongNoteMode.Input &&
            NoteModifyModeValue != NoteModifyMode.LongNote &&
            BPMModeValue == BPMMode.Default &&
            WaveModeValue == WaveMode.Default &&
            SetNoteModeValue == SetNoteMode.Default &&
            PutCopyNotesValueV2 == PutCopyNotes.Default;

        public string CanBeTwilightCommentContents => CanBeTwilightComment ? LanguageSystem.Instance.CanBeTwilightCommentContents : LanguageSystem.Instance.CannotBeTwilightCommentContents;

        public bool IsLowerStand => (Component.AutoableInputCounts[(int)InputMode] > 0 && AutoModeValue == AutoMode.Autoable) ||
            AudioMultiplier < 1.0 ||
            JudgmentModeValue == JudgmentMode.Lower || JudgmentModeValue == JudgmentMode.Lowest ||
            HitPointsModeValue == HitPointsMode.Lower || HitPointsModeValue == HitPointsMode.Lowest ||
            LongNoteModeValue == LongNoteMode.Auto ||
            (InputFavorModeValue != InputFavorMode.Default && (Component.AutoableInputCounts[(int)InputFavorModeValue] < Component.AutoableInputCounts[(int)InputMode] || Component.InputCounts[(int)InputFavorModeValue] < Component.InputCounts[(int)InputMode]));

        /// <summary>
        /// 계산된 스크롤 속도
        /// </summary>
        public double Multiplier => Math.Max(0.0, CanModifyMultiplier ? MultiplierValue / (BPM * AudioMultiplier) : SentMultiplier);

        public string IIDXMultiplierMillisText
        {
            get
            {
                var value = ViewModels.Instance.MainValue.GetHandlingComputer()?.GetIIDXMultiplierMillis(this) ?? 0.0;
                return $"{value:#,##0.##} ms ({6 * value / 10:#,##0} frame IIDX)";
            }
        }

        [JsonIgnore]
        public double BPM => ComputingValue?.BPM ?? Component.StandardBPM;

        [JsonIgnore]
        public Component.InputMode InputMode => ComputingValue?.InputMode ?? default;

        [JsonIgnore]
        public bool IsSalt => ComputingValue?.IsSalt ?? false;

        [JsonIgnore]
        public bool CanModifyMultiplier
        {
            get => _valueCanModifyMultiplier;

            set
            {
                if (SetProperty(ref _valueCanModifyMultiplier, value, nameof(CanModifyMultiplier)))
                {
                    NotifyIIDXMultiplierMillisText();
                }
            }
        }

        [JsonIgnore]
        public bool CanModifyAudioMultiplier
        {
            get => _valueCanModifyAudioMultiplier;

            set => SetProperty(ref _valueCanModifyAudioMultiplier, value, nameof(CanModifyAudioMultiplier));
        }

        public double MultiplierValue
        {
            get => _multiplierValue;

            set
            {
                if (SetProperty(ref _multiplierValue, value, nameof(MultiplierValue)))
                {
                    OnPropertyChanged(nameof(Multiplier));
                    NotifyIIDXMultiplierMillisText();
                }
            }
        }

        [JsonIgnore]
        public Computing ComputingValue
        {
            get => _valueComputing;

            set
            {
                if (SetProperty(ref _valueComputing, value, nameof(Multiplier)))
                {
                    ComponentValue.SetBPM(BPM);
                    NotifyIIDXMultiplierMillisText();
                }
            }
        }

        /// <summary>
        /// 고정 스크롤 속도
        /// </summary>
        [JsonIgnore]
        public double SentMultiplier
        {
            get => _sentMultiplier;

            set
            {
                if (SetProperty(ref _sentMultiplier, value, nameof(Multiplier)))
                {
                    NotifyIIDXMultiplierMillisText();
                }
            }
        }

        public void NotifyIIDXMultiplierMillisText() => OnPropertyChanged(nameof(IIDXMultiplierMillisText));

        public int Salt
        {
            get => _salt;

            set => SetProperty(ref _salt, value, nameof(Salt));
        }

        public AutoMode AutoModeValue
        {
            get => _autoMode;

            set
            {
                if (SetProperty(ref _autoMode, value, nameof(AutoModeValue)))
                {
                    OnPropertyChanged(nameof(AutoModeContents));
                }
            }
        }

        public string AutoModeContents => LanguageSystem.Instance.AutoModeTexts[(int)AutoModeValue];

        public NoteSaltMode NoteSaltModeValue
        {
            get => _noteSaltMode;

            set
            {
                if (SetProperty(ref _noteSaltMode, value, nameof(NoteSaltModeValue)))
                {
                    OnPropertyChanged(nameof(NoteSaltModeContents));
                }
            }
        }

        public string NoteSaltModeContents => LanguageSystem.Instance.NoteSaltModeTexts[(int)NoteSaltModeValue];

        public bool IsAudioMultiplierWarning => AudioMultiplier == 0.98 || AudioMultiplier == 0.99 || AudioMultiplier == 1.01 || AudioMultiplier == 1.02;

        public double AudioMultiplier
        {
            get => _audioMultiplier;

            set
            {
                if (SetProperty(ref _audioMultiplier, value, nameof(AudioMultiplier)))
                {
                    OnPropertyChanged(nameof(Multiplier));
                    NotifyIIDXMultiplierMillisText();
                }
            }
        }

        public FaintNoteMode FaintNoteModeValue
        {
            get => _faintNoteMode;

            set
            {
                if (SetProperty(ref _faintNoteMode, value, nameof(FaintNoteModeValue)))
                {
                    OnPropertyChanged(nameof(FaintNoteModeContents));
                }
            }
        }

        public string FaintNoteModeContents => LanguageSystem.Instance.FaintNoteModeTexts[(int)FaintNoteModeValue];

        public JudgmentMode JudgmentModeValue
        {
            get => _judgmentMode;

            set
            {
                if (SetProperty(ref _judgmentMode, value, nameof(JudgmentModeValue)))
                {
                    OnPropertyChanged(nameof(JudgmentModeContents));
                    OnPropertyChanged(nameof(CanBeTwilightCommentContents));
                }
            }
        }

        public string JudgmentModeContents => LanguageSystem.Instance.JudgmentModeTexts[(int)JudgmentModeValue];

        public HitPointsMode HitPointsModeValue
        {
            get => _hitPointsMode;

            set
            {
                if (SetProperty(ref _hitPointsMode, value, nameof(HitPointsModeValue)))
                {
                    OnPropertyChanged(nameof(HitPointsModeContents));
                    OnPropertyChanged(nameof(CanBeTwilightCommentContents));
                }
                HandlingHitPointsModeValue = value;
            }
        }

        public string HitPointsModeContents => LanguageSystem.Instance.HitPointsModeTexts[(int)HitPointsModeValue];

        [JsonIgnore]
        public HitPointsMode HandlingHitPointsModeValue { get; set; }

        public Color HandlingHitPointsColor => BaseUI.Instance.HitPointsColor[(int)HandlingHitPointsModeValue];

        public bool CanGAS => (HandlingHitPointsModeValue == HitPointsMode.Highest && Configure.Instance.GASLevel >= 1) || (HandlingHitPointsModeValue == HitPointsMode.Higher && Configure.Instance.GASLevel >= 2);

        public NoteMobilityMode NoteMobilityModeValue
        {
            get => _noteMobilityMode;

            set
            {
                if (SetProperty(ref _noteMobilityMode, value, nameof(NoteMobilityModeValue)))
                {
                    OnPropertyChanged(nameof(NoteMobilityModeContents));
                }
            }
        }

        public string NoteMobilityModeContents => LanguageSystem.Instance.NoteMobilityModeTexts[(int)NoteMobilityModeValue];

        public LongNoteMode LongNoteModeValue
        {
            get => _longNoteMode;

            set
            {
                if (SetProperty(ref _longNoteMode, value, nameof(LongNoteModeValue)))
                {
                    OnPropertyChanged(nameof(LongNoteModeContents));
                    OnPropertyChanged(nameof(CanBeTwilightCommentContents));
                }
            }
        }

        public string LongNoteModeContents => LanguageSystem.Instance.LongNoteModeTexts[(int)LongNoteModeValue];

        public InputFavorMode InputFavorModeValue
        {
            get => _inputFavorMode;

            set
            {
                if (SetProperty(ref _inputFavorMode, value, nameof(InputFavorModeValue)))
                {
                    OnPropertyChanged(nameof(InputFavorModeContents));
                }
            }
        }

        public string InputFavorModeContents => LanguageSystem.Instance.InputFavorModeTexts[(int)InputFavorModeValue];

        public NoteModifyMode NoteModifyModeValue
        {
            get => _noteModifyMode;

            set
            {
                if (SetProperty(ref _noteModifyMode, value, nameof(NoteModifyModeValue)))
                {
                    OnPropertyChanged(nameof(NoteModifyModeContents));
                    OnPropertyChanged(nameof(CanBeTwilightCommentContents));
                }
            }
        }

        public string NoteModifyModeContents => LanguageSystem.Instance.NoteModifyModeTexts[(int)NoteModifyModeValue];

        public BPMMode BPMModeValue
        {
            get => _bpmMode;

            set
            {
                if (SetProperty(ref _bpmMode, value, nameof(BPMModeValue)))
                {
                    OnPropertyChanged(nameof(BPMModeContents));
                    OnPropertyChanged(nameof(CanBeTwilightCommentContents));
                }
            }
        }

        public string BPMModeContents => LanguageSystem.Instance.BPMModeTexts[(int)BPMModeValue];

        public WaveMode WaveModeValue
        {
            get => _waveMode;

            set
            {
                if (SetProperty(ref _waveMode, value, nameof(WaveModeValue)))
                {
                    OnPropertyChanged(nameof(WaveModeContents));
                    OnPropertyChanged(nameof(CanBeTwilightCommentContents));
                }
            }
        }

        public string WaveModeContents => LanguageSystem.Instance.WaveModeTexts[(int)WaveModeValue];

        public SetNoteMode SetNoteModeValue
        {
            get => _setNoteMode;

            set
            {
                if (SetProperty(ref _setNoteMode, value, nameof(SetNoteModeValue)))
                {
                    OnPropertyChanged(nameof(SetNoteModeContents));
                    OnPropertyChanged(nameof(CanBeTwilightCommentContents));
                }
            }
        }

        public string SetNoteModeContents => LanguageSystem.Instance.SetNoteModeTexts[(int)SetNoteModeValue];

        public LowestJudgmentConditionMode LowestJudgmentConditionModeValue
        {
            get => _lowestJudgmentConditionMode;

            set
            {
                if (SetProperty(ref _lowestJudgmentConditionMode, value, nameof(LowestJudgmentConditionModeValue)))
                {
                    OnPropertyChanged(nameof(LowestJudgmentConditionModeContents));
                }
            }
        }

        public string LowestJudgmentConditionModeContents => LanguageSystem.Instance.LowestJudgmentConditionModeTexts[(int)LowestJudgmentConditionModeValue];

        public double[][] FavorJudgments { get; set; } = new double[6][]
        {
            new double[] { -1.0, 1.0 },
            new double[] { -1.0, 1.0 },
            new double[] { -1.0, 1.0 },
            new double[] { -1.0, 1.0 },
            new double[] { -1.0, 1.0 },
            new double[] { -1.0, 1.0 }
        };

        [JsonIgnore]
        public double HighestJudgment0
        {
            get => FavorJudgments[(int)Component.Judged.Highest][0];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Highest][0], value, nameof(HighestJudgment0)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Highest][1], Math.Max(HighestJudgment1, HighestJudgment0), nameof(HighestJudgment1));
                }
            }
        }

        [JsonIgnore]
        public double HigherJudgment0
        {
            get => FavorJudgments[(int)Component.Judged.Higher][0];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Higher][0], value, nameof(HigherJudgment0)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Higher][1], Math.Max(HigherJudgment1, HigherJudgment0), nameof(HigherJudgment1));
                }
            }
        }

        [JsonIgnore]
        public double HighJudgment0
        {
            get => FavorJudgments[(int)Component.Judged.High][0];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.High][0], value, nameof(HighJudgment0)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.High][1], Math.Max(HighJudgment1, HighJudgment0), nameof(HighJudgment1));
                }
            }
        }

        [JsonIgnore]
        public double LowJudgment0
        {
            get => FavorJudgments[(int)Component.Judged.Low][0];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Low][0], value, nameof(LowJudgment0)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Low][1], Math.Max(LowJudgment1, LowJudgment0), nameof(LowJudgment1));
                }
            }
        }

        [JsonIgnore]
        public double LowerJudgment0
        {
            get => FavorJudgments[(int)Component.Judged.Lower][0];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Lower][0], value, nameof(LowerJudgment0)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Lower][1], Math.Max(LowerJudgment1, LowerJudgment0), nameof(LowerJudgment1));
                }
            }
        }

        [JsonIgnore]
        public double LowestJudgment0
        {
            get => FavorJudgments[(int)Component.Judged.Lowest][0];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Lowest][0], value, nameof(LowestJudgment0)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Lowest][1], Math.Max(LowestJudgment1, LowestJudgment0), nameof(LowestJudgment1));
                }
            }
        }

        [JsonIgnore]
        public double HighestJudgment1
        {
            get => FavorJudgments[(int)Component.Judged.Highest][1];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Highest][1], value, nameof(HighestJudgment1)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Highest][0], Math.Min(HighestJudgment0, HighestJudgment1), nameof(HighestJudgment0));
                }
            }
        }

        [JsonIgnore]
        public double HigherJudgment1
        {
            get => FavorJudgments[(int)Component.Judged.Higher][1];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Higher][1], value, nameof(HigherJudgment1)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Higher][0], Math.Min(HigherJudgment0, HigherJudgment1), nameof(HigherJudgment0));
                }
            }
        }

        [JsonIgnore]
        public double HighJudgment1
        {
            get => FavorJudgments[(int)Component.Judged.High][1];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.High][1], value, nameof(HighJudgment1)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.High][0], Math.Min(HighJudgment0, HighJudgment1), nameof(HighJudgment0));
                }
            }
        }

        [JsonIgnore]
        public double LowJudgment1
        {
            get => FavorJudgments[(int)Component.Judged.Low][1];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Low][1], value, nameof(LowJudgment1)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Low][0], Math.Min(LowJudgment0, LowJudgment1), nameof(LowJudgment0));
                }
            }
        }

        [JsonIgnore]
        public double LowerJudgment1
        {
            get => FavorJudgments[(int)Component.Judged.Lower][1];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Lower][1], value, nameof(LowerJudgment1)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Lower][0], Math.Min(LowerJudgment0, LowerJudgment1), nameof(LowerJudgment0));
                }
            }
        }

        [JsonIgnore]
        public double LowestJudgment1
        {
            get => FavorJudgments[(int)Component.Judged.Lowest][1];

            set
            {
                if (SetProperty(ref FavorJudgments[(int)Component.Judged.Lowest][1], value, nameof(LowestJudgment1)))
                {
                    SetProperty(ref FavorJudgments[(int)Component.Judged.Lowest][0], Math.Min(LowestJudgment0, LowestJudgment1), nameof(LowestJudgment0));
                }
            }
        }

        public double[][] FavorHitPoints { get; set; } = new double[6][]
        {
            new double[] { 0.0, 0.0 },
            new double[] { 0.0, 0.0 },
            new double[] { 0.0, 0.0 },
            new double[] { 0.0, 0.0 },
            new double[] { 0.0, 0.0 },
            new double[] { 0.0, 0.0 }
        };

        [JsonIgnore]
        public double HighestHitPoints0
        {
            get => FavorHitPoints[(int)Component.Judged.Highest][0];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Highest][0], value, nameof(HighestHitPoints0));
        }

        [JsonIgnore]
        public double HigherHitPoints0
        {
            get => FavorHitPoints[(int)Component.Judged.Higher][0];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Higher][0], value, nameof(HigherHitPoints0));
        }

        [JsonIgnore]
        public double HighHitPoints0
        {
            get => FavorHitPoints[(int)Component.Judged.High][0];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.High][0], value, nameof(HighHitPoints0));
        }

        [JsonIgnore]
        public double LowHitPoints0
        {
            get => FavorHitPoints[(int)Component.Judged.Low][0];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Low][0], value, nameof(LowHitPoints0));
        }

        [JsonIgnore]
        public double LowerHitPoints0
        {
            get => FavorHitPoints[(int)Component.Judged.Lower][0];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Lower][0], value, nameof(LowerHitPoints0));
        }

        [JsonIgnore]
        public double LowestHitPoints0
        {
            get => FavorHitPoints[(int)Component.Judged.Lowest][0];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Lowest][0], value, nameof(LowestHitPoints0));
        }

        [JsonIgnore]
        public double HighestHitPoints1
        {
            get => FavorHitPoints[(int)Component.Judged.Highest][1];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Highest][1], value, nameof(HighestHitPoints1));
        }

        [JsonIgnore]
        public double HigherHitPoints1
        {
            get => FavorHitPoints[(int)Component.Judged.Higher][1];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Higher][1], value, nameof(HigherHitPoints1));
        }

        [JsonIgnore]
        public double HighHitPoints1
        {
            get => FavorHitPoints[(int)Component.Judged.High][1];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.High][1], value, nameof(HighHitPoints1));
        }

        [JsonIgnore]
        public double LowHitPoints1
        {
            get => FavorHitPoints[(int)Component.Judged.Low][1];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Low][1], value, nameof(LowHitPoints1));
        }

        [JsonIgnore]
        public double LowerHitPoints1
        {
            get => FavorHitPoints[(int)Component.Judged.Lower][1];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Lower][1], value, nameof(LowerHitPoints1));
        }

        [JsonIgnore]
        public double LowestHitPoints1
        {
            get => FavorHitPoints[(int)Component.Judged.Lowest][1];

            set => SetProperty(ref FavorHitPoints[(int)Component.Judged.Lowest][1], value, nameof(LowestHitPoints1));
        }

        public void SetLowestAutoLongNoteModify()
        {
            if (Configure.Instance.LowestAutoLongNoteModify)
            {
                ConfigureLowestLongNoteModify = 1000.0 * 240.0 / (BPM * AudioMultiplier) / Configure.Instance.LowestAutoLongNoteModifyValue;
            }
        }

        public void SetHighestAutoLongNoteModify()
        {
            if (Configure.Instance.HighestAutoLongNoteModify)
            {
                ConfigureHighestLongNoteModify = 1000.0 * 240.0 / (BPM * AudioMultiplier) / Configure.Instance.HighestAutoLongNoteModifyValue;
            }
        }

        public void SetAutoPutNoteSetMillis()
        {
            if (Configure.Instance.AutoPutNoteSetMillis)
            {
                PutNoteSetMillis = 1000.0 * 240.0 / (BPM * AudioMultiplier) / Configure.Instance.AutoPutNoteSetMillisValue;
            }
        }

        public double ConfigureLowestLongNoteModify
        {
            get => _lowestLongNoteModify;

            set => SetProperty(ref _lowestLongNoteModify, value, nameof(ConfigureLowestLongNoteModify));
        }

        public double ConfigureHighestLongNoteModify
        {
            get => _highestLongNoteModify;

            set => SetProperty(ref _highestLongNoteModify, value, nameof(ConfigureHighestLongNoteModify));
        }

        public double PutNoteSet
        {
            get => _putNoteSet;

            set => SetProperty(ref _putNoteSet, value, nameof(PutNoteSet));
        }

        public double PutNoteSetMillis
        {
            get => _putNoteSetMillis;

            set => SetProperty(ref _putNoteSetMillis, value, nameof(PutNoteSetMillis));
        }

        public object GetJSON() => new
        {
            salt = Salt,
            autoMode = AutoModeValue,
            noteSaltMode = NoteSaltModeValue,
            audioMultiplier = AudioMultiplier,
            faintNoteMode = FaintNoteModeValue,
            judgmentMode = JudgmentModeValue,
            hitPointsMode = HandlingHitPointsModeValue,
            noteMobilityMode = NoteMobilityModeValue,
            longNoteMode = LongNoteModeValue,
            inputFavorMode = InputFavorModeValue,
            noteModifyMode = NoteModifyModeValue,
            bpmMode = BPMModeValue,
            waveMode = WaveModeValue,
            setNoteMode = SetNoteModeValue,
            lowestJudgmentConditionMode = LowestJudgmentConditionModeValue,
            putCopyNotes = PutCopyNotesValueV2,
            highestJudgment0 = HighestJudgment0,
            higherJudgment0 = HigherJudgment0,
            highJudgment0 = HighJudgment0,
            lowJudgment0 = LowJudgment0,
            lowerJudgment0 = LowerJudgment0,
            lowestJudgment0 = LowestJudgment0,
            highestJudgment1 = HighestJudgment1,
            higherJudgment1 = HigherJudgment1,
            highJudgment1 = HighJudgment1,
            lowJudgment1 = LowJudgment1,
            lowerJudgment1 = LowerJudgment1,
            lowestJudgment1 = LowestJudgment1,
            lowestLongNoteModify = ConfigureLowestLongNoteModify,
            highestLongNoteModify = ConfigureHighestLongNoteModify,
            putNoteSet = PutNoteSet,
            putNoteSetMillis = PutNoteSetMillis,
            highestHitPoints0 = HighestHitPoints0,
            higherHitPoints0 = HigherHitPoints0,
            highHitPoints0 = HighHitPoints0,
            lowHitPoints0 = LowHitPoints0,
            lowerHitPoints0 = LowerHitPoints0,
            lowestHitPoints0 = LowestHitPoints0,
            highestHitPoints1 = HighestHitPoints1,
            higherHitPoints1 = HigherHitPoints1,
            highHitPoints1 = HighHitPoints1,
            lowHitPoints1 = LowHitPoints1,
            lowerHitPoints1 = LowerHitPoints1,
            lowestHitPoints1 = LowestHitPoints1,
        };

        public void InitModeComponent()
        {
            AutoModeValue = AutoMode.Default;
            NoteSaltModeValue = NoteSaltMode.Default;
            SetAudioMultiplier(1.0);
            FaintNoteModeValue = FaintNoteMode.Default;
            JudgmentModeValue = JudgmentMode.Default;
            HitPointsModeValue = HitPointsMode.Highest;
            NoteMobilityModeValue = NoteMobilityMode.Default;
            LongNoteModeValue = LongNoteMode.Default;
            InputFavorModeValue = InputFavorMode.Default;
            NoteModifyModeValue = NoteModifyMode.Default;
            BPMModeValue = BPMMode.Default;
            WaveModeValue = WaveMode.Default;
            SetNoteModeValue = SetNoteMode.Default;
            LowestJudgmentConditionModeValue = LowestJudgmentConditionMode.Default;
            PutCopyNotesValueV2 = PutCopyNotes.Default;
        }

        public ModeComponent Clone()
        {
            var modeComponentValue = new ModeComponent();
            modeComponentValue.CopyAs(this);
            return modeComponentValue;
        }

        public bool IsCompatible(ModeComponent modeComponentValue)
        {
            if (NoteSaltModeValue != modeComponentValue.NoteSaltModeValue)
            {
                return false;
            }
            if (LongNoteModeValue != modeComponentValue.LongNoteModeValue)
            {
                return false;
            }
            if (InputFavorModeValue != modeComponentValue.InputFavorModeValue)
            {
                return false;
            }
            if (NoteModifyModeValue != modeComponentValue.NoteModifyModeValue)
            {
                return false;
            }
            if (BPMModeValue != modeComponentValue.BPMModeValue)
            {
                return false;
            }
            if (WaveModeValue != modeComponentValue.WaveModeValue)
            {
                return false;
            }
            if (SetNoteModeValue != modeComponentValue.SetNoteModeValue)
            {
                return false;
            }
            if (PutCopyNotesValueV2 != modeComponentValue.PutCopyNotesValueV2)
            {
                return false;
            }
            if (NoteModifyModeValue == NoteModifyMode.LongNote)
            {
                if (modeComponentValue.NoteModifyModeValue == NoteModifyMode.LongNote)
                {
                    if (ConfigureLowestLongNoteModify != modeComponentValue.ConfigureLowestLongNoteModify || ConfigureHighestLongNoteModify != modeComponentValue.ConfigureHighestLongNoteModify)
                    {
                        return false;
                    }
                }
            }
            if (SetNoteModeValue != SetNoteMode.Default)
            {
                if (modeComponentValue.SetNoteModeValue != SetNoteMode.Default)
                {
                    if (PutNoteSet != modeComponentValue.PutNoteSet || PutNoteSetMillis != modeComponentValue.PutNoteSetMillis)
                    {
                        return false;
                    }
                }
            }
            if (NoteSaltModeValue != NoteSaltMode.Default && NoteSaltModeValue != NoteSaltMode.Symmetric)
            {
                if (modeComponentValue.NoteSaltModeValue != NoteSaltMode.Default && modeComponentValue.NoteSaltModeValue != NoteSaltMode.Symmetric)
                {
                    if (Salt != modeComponentValue.Salt)
                    {
                        return false;
                    }
                }
            }
            if (NoteMobilityModeValue == NoteMobilityMode._4DHD || NoteMobilityModeValue == NoteMobilityMode._4D)
            {
                if (modeComponentValue.NoteMobilityModeValue == NoteMobilityMode._4DHD || modeComponentValue.NoteMobilityModeValue == NoteMobilityMode._4D)
                {
                    if (Salt != modeComponentValue.Salt)
                    {
                        return false;
                    }
                }
            }
            if (SetNoteModeValue != SetNoteMode.Default)
            {
                if (modeComponentValue.SetNoteModeValue != SetNoteMode.Default)
                {
                    if (Salt != modeComponentValue.Salt)
                    {
                        return false;
                    }
                }
            }
            if (IsSalt)
            {
                if (modeComponentValue.IsSalt)
                {
                    if (Salt != modeComponentValue.Salt)
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void CopyAs(ModeComponent modeComponentValue, Computing valueComputing = null, bool? setSalt = null)
        {
            AutoModeValue = modeComponentValue.AutoModeValue;
            NoteSaltModeValue = modeComponentValue.NoteSaltModeValue;
            AudioMultiplier = modeComponentValue.AudioMultiplier;
            FaintNoteModeValue = modeComponentValue.FaintNoteModeValue;
            JudgmentModeValue = modeComponentValue.JudgmentModeValue;
            var handlingHitPointsModeValue = modeComponentValue.HandlingHitPointsModeValue;
            HitPointsModeValue = modeComponentValue.HitPointsModeValue;
            HandlingHitPointsModeValue = handlingHitPointsModeValue;
            NoteMobilityModeValue = modeComponentValue.NoteMobilityModeValue;
            LongNoteModeValue = modeComponentValue.LongNoteModeValue;
            InputFavorModeValue = modeComponentValue.InputFavorModeValue;
            NoteModifyModeValue = modeComponentValue.NoteModifyModeValue;
            BPMModeValue = modeComponentValue.BPMModeValue;
            WaveModeValue = modeComponentValue.WaveModeValue;
            SetNoteModeValue = modeComponentValue.SetNoteModeValue;
            LowestJudgmentConditionModeValue = modeComponentValue.LowestJudgmentConditionModeValue;
            var favorJudgments = modeComponentValue.FavorJudgments;
            for (var i = favorJudgments.Length - 1; i >= 0; --i)
            {
                for (var j = favorJudgments[i].Length - 1; j >= 0; --j)
                {
                    FavorJudgments[i][j] = favorJudgments[i][j];
                }
            }
            OnPropertyChanged(nameof(HighestJudgment0));
            OnPropertyChanged(nameof(HigherJudgment0));
            OnPropertyChanged(nameof(HighJudgment0));
            OnPropertyChanged(nameof(LowJudgment0));
            OnPropertyChanged(nameof(LowerJudgment0));
            OnPropertyChanged(nameof(LowestJudgment0));
            OnPropertyChanged(nameof(HighestJudgment1));
            OnPropertyChanged(nameof(HigherJudgment1));
            OnPropertyChanged(nameof(HighJudgment1));
            OnPropertyChanged(nameof(LowJudgment1));
            OnPropertyChanged(nameof(LowerJudgment1));
            OnPropertyChanged(nameof(LowestJudgment1));
            var favorHitPoints = modeComponentValue.FavorHitPoints;
            for (var i = favorHitPoints.Length - 1; i >= 0; --i)
            {
                for (var j = favorHitPoints[i].Length - 1; j >= 0; --j)
                {
                    FavorHitPoints[i][j] = favorHitPoints[i][j];
                }
            }
            OnPropertyChanged(nameof(LowestHitPoints0));
            OnPropertyChanged(nameof(LowestHitPoints1));
            OnPropertyChanged(nameof(LowerHitPoints0));
            OnPropertyChanged(nameof(LowerHitPoints1));
            OnPropertyChanged(nameof(LowHitPoints0));
            OnPropertyChanged(nameof(LowHitPoints1));
            OnPropertyChanged(nameof(HighHitPoints0));
            OnPropertyChanged(nameof(HighHitPoints1));
            OnPropertyChanged(nameof(HigherHitPoints0));
            OnPropertyChanged(nameof(HigherHitPoints1));
            OnPropertyChanged(nameof(HighestHitPoints0));
            OnPropertyChanged(nameof(HighestHitPoints1));
            ConfigureLowestLongNoteModify = modeComponentValue.ConfigureLowestLongNoteModify;
            ConfigureHighestLongNoteModify = modeComponentValue.ConfigureHighestLongNoteModify;
            PutNoteSet = modeComponentValue.PutNoteSet;
            PutNoteSetMillis = modeComponentValue.PutNoteSetMillis;
            PutCopyNotesValueV2 = modeComponentValue.PutCopyNotesValueV2;
            CanModifyMultiplier = modeComponentValue.CanModifyMultiplier;
            SentMultiplier = modeComponentValue.SentMultiplier;
            MultiplierValue = modeComponentValue.MultiplierValue;
            CanModifyAudioMultiplier = modeComponentValue.CanModifyAudioMultiplier;
            if (!(setSalt ?? Configure.Instance.SetSalt))
            {
                Salt = modeComponentValue.Salt;
            }
            ComputingValue = valueComputing ?? modeComponentValue.ComputingValue;
        }

        public bool HigherMultiplier()
        {
            var wasModified = SetMultiplier(MultiplierValue + BPM * AudioMultiplier * _multiplierUnit);
            if (wasModified)
            {
                _multiplierUnit += 0.01;
            }
            return wasModified;
        }

        public bool LowerMultiplier()
        {
            var wasModified = SetMultiplier(MultiplierValue - BPM * AudioMultiplier * _multiplierUnit);
            if (wasModified)
            {
                _multiplierUnit += 0.01;
            }
            return wasModified;
        }

        public bool SetMultiplier(double multiplierValue)
        {
            if (CanModifyMultiplier)
            {
                multiplierValue = Math.Max(0.0, multiplierValue);
                if (multiplierValue != MultiplierValue)
                {
                    MultiplierValue = multiplierValue;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public void CopyAsJSON(JSON.ModeComponentData modeComponentData)
        {
            Salt = modeComponentData.salt;
            AutoModeValue = (AutoMode)modeComponentData.autoMode;
            NoteSaltModeValue = (NoteSaltMode)modeComponentData.noteSaltMode;
            AudioMultiplier = Math.Round(modeComponentData.audioMultiplier, 2);
            FaintNoteModeValue = (FaintNoteMode)modeComponentData.faintNoteMode;
            JudgmentModeValue = (JudgmentMode)modeComponentData.judgmentMode;
            HitPointsModeValue = (HitPointsMode)modeComponentData.hitPointsMode;
            NoteMobilityModeValue = (NoteMobilityMode)modeComponentData.noteMobilityMode;
            LongNoteModeValue = (LongNoteMode)modeComponentData.longNoteMode;
            InputFavorModeValue = (InputFavorMode)modeComponentData.inputFavorMode;
            NoteModifyModeValue = (NoteModifyMode)modeComponentData.noteModifyMode;
            BPMModeValue = (BPMMode)modeComponentData.bpmMode;
            WaveModeValue = (WaveMode)modeComponentData.waveMode;
            SetNoteModeValue = (SetNoteMode)modeComponentData.setNoteMode;
            LowestJudgmentConditionModeValue = (LowestJudgmentConditionMode)modeComponentData.lowestJudgmentConditionMode;
            PutCopyNotesValueV2 = (PutCopyNotes)modeComponentData.putCopyNotes;
            HighestJudgment0 = modeComponentData.highestJudgment0;
            HigherJudgment0 = modeComponentData.higherJudgment0;
            HighJudgment0 = modeComponentData.highJudgment0;
            LowJudgment0 = modeComponentData.lowJudgment0;
            LowerJudgment0 = modeComponentData.lowerJudgment0;
            LowestJudgment0 = modeComponentData.lowestJudgment0;
            HighestJudgment1 = modeComponentData.highestJudgment1;
            HigherJudgment1 = modeComponentData.higherJudgment1;
            HighJudgment1 = modeComponentData.highJudgment1;
            LowJudgment1 = modeComponentData.lowJudgment1;
            LowerJudgment1 = modeComponentData.lowerJudgment1;
            LowestJudgment1 = modeComponentData.lowestJudgment1;
            ConfigureLowestLongNoteModify = modeComponentData.lowestLongNoteModify;
            ConfigureHighestLongNoteModify = modeComponentData.highestLongNoteModify;
            PutNoteSet = modeComponentData.putNoteSet;
            PutNoteSetMillis = modeComponentData.putNoteSetMillis;
            HighestHitPoints0 = modeComponentData.highestHitPoints0;
            HigherHitPoints0 = modeComponentData.higherHitPoints0;
            HighHitPoints0 = modeComponentData.highHitPoints0;
            LowHitPoints0 = modeComponentData.lowHitPoints0;
            LowerHitPoints0 = modeComponentData.lowerHitPoints0;
            LowestHitPoints0 = modeComponentData.lowestHitPoints0;
            HighestHitPoints1 = modeComponentData.highestHitPoints1;
            HigherHitPoints1 = modeComponentData.higherHitPoints1;
            HighHitPoints1 = modeComponentData.highHitPoints1;
            LowHitPoints1 = modeComponentData.lowHitPoints1;
            LowerHitPoints1 = modeComponentData.lowerHitPoints1;
            LowestHitPoints1 = modeComponentData.lowestHitPoints1;
        }

        public void InitMultiplierUnit() => _multiplierUnit = 0.1;

        public bool HigherAudioMultiplier()
        {
            return SetAudioMultiplier(Math.Round(AudioMultiplier + 0.01, 2));
        }

        public bool LowerAudioMultiplier()
        {
            return SetAudioMultiplier(Math.Round(AudioMultiplier - 0.01, 2));
        }

        public bool SetAudioMultiplier(double audioMultiplier)
        {
            if (CanModifyAudioMultiplier)
            {
                audioMultiplier = Math.Clamp(audioMultiplier, 0.5, 2.0);
                if (audioMultiplier != AudioMultiplier)
                {
                    AudioMultiplier = audioMultiplier;
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}