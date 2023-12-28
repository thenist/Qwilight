using CommunityToolkit.Mvvm.Messaging;
using FMOD;
using Microsoft.Win32;
using Qwilight.MSG;
using Qwilight.NoteFile;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Globalization;
using System.IO;
using System.Runtime.InteropServices;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Windows;
using System.Windows.Media;
using Windows.Gaming.Input;
using Windows.System;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;

namespace Qwilight
{
    public sealed partial class Configure : Model
    {
        enum ReflexMode
        {
            eOff,
            eLowLatency,
            eLowLatencyWithBoost,
        }

        [LibraryImport("NVIDIA")]
        private static partial void SetNVLLConfigure(ReflexMode mode, uint frameLimitUs);

        public enum TutorialID
        {
            NetQuitMode,
            F1Assist,
            ModifyAutoMode,
            EnterAutoComputeConfigure
        }

        public enum NVLLMode
        {
            Not, Standard, Unlimited
        }

        public enum InputAudioVariety
        {
            DJMAX, IIDX
        }

        public enum AudioVisualizerMode
        {
            AudioVisualizerMode0, AudioVisualizerMode1, AudioVisualizerMode2
        }

        public enum WantBanned
        {
            Total, NotBanned
        }

        public enum DefaultSpinningMode
        {
            Unpause, Configure, Undo, Stop
        }

        public enum DefaultAudioVariety
        {
            Not, UI, Favor
        }

        public static readonly Configure Instance = QwilightComponent.GetBuiltInData<Configure>(nameof(Configure));

        static readonly JsonSerializerOptions _defaultJSONConfigure = Utility.GetJSONConfigure(defaultJSONConfigure =>
        {
            defaultJSONConfigure.Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping;
            defaultJSONConfigure.IgnoreReadOnlyProperties = true;
        });
        static readonly string _fileName = Path.Combine(QwilightComponent.QwilightEntryPath, "Configure.json");
        static readonly string _faultFileName = Path.ChangeExtension(_fileName, ".json.$");
        static readonly string _tmp0FileName = Path.ChangeExtension(_fileName, ".json.tmp.0");
        static readonly string _tmp1FileName = Path.ChangeExtension(_fileName, ".json.tmp.1");

        public Configure()
        {
            _aesCipher = SHA256.HashData(Encoding.UTF8.GetBytes(Environment.MachineName));
            SystemEvents.DisplaySettingsChanged += (sender, e) => OnSetAutoNVLLFramerate();
        }

        public void NotifyTutorial(TutorialID tutorialID)
        {
            if (PassedTutorialIDs.Add(tutorialID))
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, typeof(LanguageSystem).GetProperties().Single(property => property.Name == $"TutorialID{(int)tutorialID}").GetValue(LanguageSystem.Instance) as string);
            }
        }

        public void Load()
        {
            Utility.WipeFile(_tmp0FileName);
            Utility.MoveFile(_tmp1FileName, _fileName);
            try
            {
                if (File.Exists(_fileName))
                {
                    var textConfigure = Utility.GetJSON<Configure>(File.ReadAllText(_fileName, Encoding.UTF8), _defaultJSONConfigure);
                    foreach (var value in typeof(Configure).GetProperties().Where(value => value.GetCustomAttributes(typeof(JsonIgnoreAttribute), false).Length == 0 && value.CanWrite))
                    {
                        value.SetValue(this, value.GetValue(textConfigure));
                    }
                    Validate(false);
                }
                else
                {
                    Validate(true);
                }
            }
            catch (Exception e)
            {
                ConfigureFault = $"Failed to Validate Configure ({e.Message})";
                Validate(true);
                Utility.MoveFile(_fileName, _faultFileName);
            }
            finally
            {
                _isLoaded = true;
                OnSetQwilightFill();
                OnSetMeterNoteColor();
                OnSetLimiterColor();
                OnSetAudioVisualizerMainColor();
                OnSetAudioVisualizerInputColor();
                OnSetInputNoteCountViewColor();
                OnSetAutoableInputNoteCountViewColor();
                var hwModeCollection = new List<HwMode>();
                var rawHwMode = new DEVMODEW();
                var i = 0U;
                while (PInvoke.EnumDisplaySettings(null, (ENUM_DISPLAY_SETTINGS_MODE)i++, ref rawHwMode))
                {
                    if (rawHwMode.Anonymous1.Anonymous2.dmDisplayFixedOutput == 0)
                    {
                        hwModeCollection.Add(new(rawHwMode.dmPelsWidth, rawHwMode.dmPelsHeight, rawHwMode.dmDisplayFrequency));
                    }
                }
                if (hwModeCollection.Contains(HwModeV2Value))
                {
                    OnSetHwModeV2Value();
                }
                else
                {
                    PInvoke.EnumDisplaySettings(null, ENUM_DISPLAY_SETTINGS_MODE.ENUM_CURRENT_SETTINGS, ref rawHwMode);
                    HwModeV2Value = new HwMode(rawHwMode.dmPelsWidth, rawHwMode.dmPelsHeight, rawHwMode.dmDisplayFrequency);
                }
                OnSetAutoNVLLFramerate();
                OnSetIsXwindow();
            }
        }

        readonly object _setSaveCSX = new();
        readonly byte[] _aesCipher;
        readonly byte[] _aesIV = new byte[16];
        readonly double[] _vibrations = new double[4];
        readonly double[] _audioVolumes = new double[4];
        readonly double[] _equalizers = new double[5];
        bool _detailedConfigure;
        bool _isLoaded;
        double _veilDrawingHeight;
        bool _totalLimiterVariety;
        bool _autoCompute;
        bool _windowedMode;
        bool _isXwindow;
        bool _bw;
        bool _ls;
        bool _aura;
        bool _valueK70;
        bool _mediaInput;
        bool _loadedMedia;
        bool _media;
        bool _handleInputAudio;
        bool _banalAudio;
        bool _banalMedia;
        bool _banalFailedMedia;
        bool _waveIn;
        bool _wave;
        bool _audioInput;
        bool _valueUIPipelineLimiter;
        bool _valueUIPipelineMainDrawingPaint;
        bool _valueUIPipelineJudgmentMain;
        bool _valueUIPipelineJudgmentCount;
        bool _valueUIPipelineJudgmentMeter;
        bool _valueUIPipelineJudgmentVisualizer;
        bool _valueUIPipelineJudgmentPaint;
        bool _valueUIPipelineHitNotePaint;
        bool _valueUIPipelineBPM;
        bool _valueUIPipelineNet;
        bool _valueUIPipelineHunter;
        bool _valueUICommentNote;
        bool _valueUIPipelineMainJudgmentMeter;
        bool _valueUIPipelineJudgmentInputVisualizer;
        bool _lowHitPointsFaintUI;
        bool _audioVisualizer;
        bool _lostPointAudio;
        bool _isMediaFill;
        bool _isQwilightFill;
        bool _saltAuto;
        bool _isFailMode;
        WantBanned _wantBanned;
        bool _wantLevelSystem;
        bool _wantLevelTextValue;
        bool _wantBPM;
        bool _wantAverageInputCount;
        bool _wantHighestInputCount;
        ControllerSystem.InputAPI _valueControllerInputAPI;
        DefaultControllerSystem.InputAPI _defaultControllerInputAPI;
        bool _autoHighlight;
        string _wantLevelName;
        bool _autoGetQwilight;
        bool _equalizer;
        bool _tube;
        bool _valueSFX;
        bool _flange;
        bool _averager;
        bool _audioMultiplierAtone;
        DefaultEntryItem _lastDefaultEntryItem;
        bool _autoSignIn;
        int _lowestWantLevelTextValue;
        int _highestWantLevelContensValue;
        int _lowestWantBPM;
        int _highestWantBPM;
        int _lowestWantAverageInputCount;
        int _highestWantAverageInputCount;
        int _lowestWantHighestInputCount;
        int _highestWantHighestInputCount;
        double _audioWait;
        double _banalAudioWait;
        double _mediaWait;
        ControllerSystem.VibrationMode _vibrationMode;
        UIItem _valueUIItem;
        UIItem _valueBaseUIItemValue;
        bool _alwaysNotP2Position;
        HunterVariety _hunterVariety;
        bool _netCommentFollow;
        bool _wantHellBPM;
        int _noteFormatID;
        int _commentViewTabPosition;
        int _hofViewTabPosition;
        int _hofViewTotalTabPosition;
        int _hofViewAtTabPosition;
        int _hofViewAbilityTabPosition;
        bool _autoLabelledInputFavorMillis;
        bool _autoLowestLongNoteModify;
        bool _highestAutoLongNoteModify;
        bool _autoPutNoteSetMillis;
        double _autoLabelledInputFavorMillisValue;
        double _autoLowestLongNoteModifyValue;
        double _autoHighestLongNoteModifyValue;
        double _autoPutNoteSetMillisValue;
        string _language;
        bool _setHwMode;
        HwMode _hwModeValue;
        ControllerSystem.DInputControllerVariety _dInputControllerVariety;
        int _windowLength;
        int _windowHeight;
        bool _vesa;
        bool _dataCount3;
        NVLLMode _valueNVLLMode;
        double _valueNVLLFramerate;
        bool _autoNVLLFramerate;
        bool _favorMediaInput;
        double _mainAreaFaint;
        uint _audioDataLength;
        bool _autoNoteWait;
        bool _stopLastEqualAudio;
        bool _autoJudgmentMeterMillis;
        double _judgmentMeterMillis;
        AutoJudgmentMeterMillisItem _autoJudgmentMeterMillisItem;
        bool _handleMeter;
        bool _flowValues;
        bool _ubuntuNetItemTarget;
        DefaultAudioVariety _defaultAudioVariety;
        int _valueGASLevel;

        public long LazyGCV2 { get; set; }

        public int GASLevel
        {
            get => _valueGASLevel;

            set
            {
                if (SetProperty(ref _valueGASLevel, value, nameof(GASLevel)) && _isLoaded)
                {
                    foreach (var modeComponentItem in ViewModels.Instance.ModifyModeComponentValue.ModifyModeComponentItems[ModifyModeComponentViewModel.HitPointsModeVariety])
                    {
                        modeComponentItem.NotifyIsVConfigure();
                    }
                }
            }
        }

        public bool TVAssistConfigure { get; set; }

        public bool UbuntuNetItemTarget
        {
            get => _ubuntuNetItemTarget;

            set
            {
                if (SetProperty(ref _ubuntuNetItemTarget, value, nameof(UbuntuNetItemTarget)) & _isLoaded)
                {
                    ViewModels.Instance.MainValue.LoadTwilightCommentItemCollection();
                }
            }
        }

        public bool HandleMeter
        {
            get => _handleMeter;

            set
            {
                if (SetProperty(ref _handleMeter, value))
                {
                    OnPropertyChanged(nameof(HandleMeterText));
                    OnPropertyChanged(nameof(HandleMeterPaint));
                }
            }
        }

        public string HandleMeterText => HandleMeter ? LanguageSystem.Instance.HandleMeterText : LanguageSystem.Instance.NotHandleMeterText;

        public Brush HandleMeterPaint => Paints.PointPaints[HandleMeter ? 1 : 0];

        public bool FlowValues
        {
            get => _flowValues;

            set
            {
                if (SetProperty(ref _flowValues, value))
                {
                    OnPropertyChanged(nameof(FlowValuesText));
                    OnPropertyChanged(nameof(FlowValuesPaint));
                }
            }
        }

        public string FlowValuesText => FlowValues ? LanguageSystem.Instance.FlowValuesText : LanguageSystem.Instance.NotFlowValuesText;

        public Brush FlowValuesPaint => Paints.PointPaints[FlowValues ? 1 : 0];

        public Dictionary<string, string> LevelTargetMap { get; set; }

        public Dictionary<string, AudioConfigure> AudioConfigureValues { get; set; }

        public bool AllowTwilightComment { get; set; }

        public bool IsQwilightFill
        {
            get => _isQwilightFill;

            set
            {
                if (SetProperty(ref _isQwilightFill, value))
                {
                    OnPropertyChanged(nameof(IsQwilightFillText));
                    OnPropertyChanged(nameof(IsQwilightFillContents));
                    OnPropertyChanged(nameof(QwilightFillMode));
                    OnSetQwilightFill();
                }
            }
        }

        void OnSetQwilightFill()
        {
            if (_isLoaded)
            {
                StrongReferenceMessenger.Default.Send<SetD2DViewArea>();
            }
        }

        public string IsQwilightFillText => IsQwilightFill ? LanguageSystem.Instance.QwilightFillText : LanguageSystem.Instance.NotQwilightFillText;

        public string IsQwilightFillContents => IsQwilightFill ? LanguageSystem.Instance.QwilightFillContents : LanguageSystem.Instance.NotQwilightFillContents;

        public Stretch QwilightFillMode => IsQwilightFill ? Stretch.Fill : Stretch.Uniform;

        public double JudgmentMeterMillis
        {
            get => _judgmentMeterMillis;

            set => SetProperty(ref _judgmentMeterMillis, value, nameof(JudgmentMeterMillis));
        }

        public bool AutoJudgmentMeterMillis
        {
            get => _autoJudgmentMeterMillis;

            set => SetProperty(ref _autoJudgmentMeterMillis, value, nameof(AutoJudgmentMeterMillis));
        }

        public AutoJudgmentMeterMillisItem AutoJudgmentMeterMillisItemValue
        {
            get => _autoJudgmentMeterMillisItem;

            set
            {
                if (SetProperty(ref _autoJudgmentMeterMillisItem, value) && _isLoaded)
                {
                    ViewModels.Instance.MainValue.OnJudgmentMeterMillisModified();
                }
            }
        }

        public bool DetailedConfigure
        {
            get => _detailedConfigure;

            set
            {
                if (SetProperty(ref _detailedConfigure, value, nameof(DetailedConfigure)))
                {
                    OnPropertyChanged(nameof(IsDInputXyzSensitivityVisible));
                    OnPropertyChanged(nameof(IsDInputIIDXSensitivityVisible));
                    OnPropertyChanged(nameof(IsBanalAudioVisible));
                    if (_isLoaded)
                    {
                        ViewModels.Instance.ConfigureValue.NotifyCanGetDefaultText();
                    }
                }
            }
        }

        public bool FavorMediaInput
        {
            get => _favorMediaInput;

            set => SetProperty(ref _favorMediaInput, value, nameof(FavorMediaInput));
        }

        public double MediaInputPosition0 { get; set; }

        public double MediaInputPosition1 { get; set; }

        public double MediaInputLength { get; set; }

        public double MediaInputHeight { get; set; }

        public double MediaInputFaint { get; set; }

        public void InitMediaInputArea()
        {
            MediaInputPosition0 = 0.0;
            MediaInputPosition1 = 0.0;
            MediaInputLength = 0.0;
            MediaInputHeight = 0.0;
        }

        public bool AutoNoteWait
        {
            get => _autoNoteWait;

            set => SetProperty(ref _autoNoteWait, value, nameof(AutoNoteWait));
        }

        public bool StopLastEqualAudio
        {
            get => _stopLastEqualAudio;

            set
            {
                if (SetProperty(ref _stopLastEqualAudio, value))
                {
                    OnPropertyChanged(nameof(StopLastEqualAudioText));
                    OnPropertyChanged(nameof(StopLastEqualAudioPaint));
                }
            }
        }

        public string StopLastEqualAudioText => StopLastEqualAudio ? LanguageSystem.Instance.StopLastEqualAudioText : LanguageSystem.Instance.NotStopLastEqualAudioText;

        public Brush StopLastEqualAudioPaint => Paints.PointPaints[StopLastEqualAudio ? 1 : 0];

        public bool VESAV2
        {
            get => _vesa;

            set
            {
                if (SetProperty(ref _vesa, value))
                {
                    OnPropertyChanged(nameof(VESAPaint));
                    OnPropertyChanged(nameof(VESAText));
                    OnSetAutoNVLLFramerate();
                }
            }
        }

        public Brush VESAPaint => Paints.PointPaints[VESAV2 ? 1 : 0];

        public string VESAText => VESAV2 ? LanguageSystem.Instance.VESAText : LanguageSystem.Instance.NotVESAText;

        public bool DataCount3
        {
            get => _dataCount3;

            set
            {
                if (SetProperty(ref _dataCount3, value))
                {
                    OnPropertyChanged(nameof(DataCount3Paint));
                    OnPropertyChanged(nameof(DataCount3Text));
                }
            }
        }

        public Brush DataCount3Paint => Paints.PointPaints[DataCount3 ? 1 : 0];

        public string DataCount3Text => DataCount3 ? LanguageSystem.Instance.DataCount3Text : LanguageSystem.Instance.NotDataCount3Text;

        public int DataCount => DataCount3 ? 3 : 2;

        public NVLLMode NVLLModeValue
        {
            get => _valueNVLLMode;

            set
            {
                if (SetProperty(ref _valueNVLLMode, value, nameof(NVLLModeValue)))
                {
                    OnPropertyChanged(nameof(NVLLModePaint));
                    OnPropertyChanged(nameof(NVLLModeText));
                    SetNVLLConfigureImpl();
                }
            }
        }

        public bool IsNVLL => NVLLModeValue != NVLLMode.Not;

        public Brush NVLLModePaint => Paints.PointPaints[IsNVLL ? 1 : 0];

        public string NVLLModeText => NVLLModeValue switch
        {
            NVLLMode.Not => LanguageSystem.Instance.NVLLModeNotText,
            NVLLMode.Standard => LanguageSystem.Instance.NVLLModeOKText,
            NVLLMode.Unlimited => LanguageSystem.Instance.NVLLModeUnlimitedText,
            _ => default,
        };

        public double NVLLFramerate
        {
            get => _valueNVLLFramerate;

            set
            {
                if (SetProperty(ref _valueNVLLFramerate, value, nameof(NVLLFramerate)))
                {
                    SetNVLLConfigureImpl();
                    OnSetAutoNVLLFramerate();
                }
            }
        }

        public void SetNVLLConfigureImpl()
        {
            if (_isLoaded)
            {
                SetNVLLConfigure((ReflexMode)NVLLModeValue, NVLLFramerate > 0 ? (uint)(1000 * 1000 / NVLLFramerate) : 0);
            }
        }

        public bool AutoNVLLFramerate
        {
            get => _autoNVLLFramerate;

            set
            {
                if (SetProperty(ref _autoNVLLFramerate, value, nameof(AutoNVLLFramerate)))
                {
                    OnSetAutoNVLLFramerate();
                }
            }
        }

        void OnSetAutoNVLLFramerate()
        {
            if (_isLoaded && AutoNVLLFramerate)
            {
                if (VESAV2)
                {
                    NVLLFramerate = 0.0;
                }
                else
                {
                    var rawHwMode = new DEVMODEW();
                    PInvoke.EnumDisplaySettings(null, ENUM_DISPLAY_SETTINGS_MODE.ENUM_CURRENT_SETTINGS, ref rawHwMode);
                    NVLLFramerate = rawHwMode.dmDisplayFrequency;
                }
            }
        }

        public string DefaultDrawingFilePath { get; set; }

        public string VeilDrawingFilePath { get; set; }

        public double VeilDrawingHeight
        {
            get => _veilDrawingHeight;

            set
            {
                if (SetProperty(ref _veilDrawingHeight, value, nameof(VeilDrawingHeightContents)) && _isLoaded)
                {
                    ViewModels.Instance.MainValue.ModeComponentValue.NotifyIIDXMultiplierMillisText();
                }
            }
        }

        public string VeilDrawingHeightContents => VeilDrawingHeight.ToString(LanguageSystem.Instance.PointLevelContents);

        public int NoteFormatID
        {
            get => _noteFormatID;

            set => SetProperty(ref _noteFormatID, value, nameof(NoteFormatID));
        }

        public Dictionary<string, UIConfigure> UIConfigureValuesV2 { get; set; }

        [JsonIgnore]
        public UIConfigure UIConfigureValue => UIConfigureValuesV2[UIItemValue.Title];

        public Dictionary<string, BaseUIConfigure> BaseUIConfigureValues { get; set; }

        [JsonIgnore]
        public BaseUIConfigure BaseUIConfigureValue => BaseUIConfigureValues[BaseUIItemValue.Title];

        [JsonIgnore]
        public string ConfigureFault { get; set; }

        public bool AutoLabelledInputFavorMillis
        {
            get => _autoLabelledInputFavorMillis;

            set
            {
                if (SetProperty(ref _autoLabelledInputFavorMillis, value, nameof(AutoLabelledInputFavorMillis)) && _isLoaded)
                {
                    ViewModels.Instance.MainValue.ModeComponentValue.SetAutoLabelledInputFavorMillis();
                }
            }
        }

        public bool AutoLowestLongNoteModify
        {
            get => _autoLowestLongNoteModify;

            set
            {
                if (SetProperty(ref _autoLowestLongNoteModify, value, nameof(AutoLowestLongNoteModify)) && _isLoaded)
                {
                    ViewModels.Instance.MainValue.ModeComponentValue.SetAutoLowestLongNoteModify();
                }
            }
        }

        public double AutoLabelledInputFavorMillisValue
        {
            get => _autoLabelledInputFavorMillisValue;

            set
            {
                if (SetProperty(ref _autoLabelledInputFavorMillisValue, value, nameof(AutoLabelledInputFavorMillisValue)) && _isLoaded)
                {
                    ViewModels.Instance.MainValue.ModeComponentValue.SetAutoLabelledInputFavorMillis();
                }
            }
        }

        public bool HighestAutoLongNoteModify
        {
            get => _highestAutoLongNoteModify;

            set
            {
                if (SetProperty(ref _highestAutoLongNoteModify, value, nameof(HighestAutoLongNoteModify)) && _isLoaded)
                {
                    ViewModels.Instance.MainValue.ModeComponentValue.SetAutoHighestLongNoteModify();
                }
            }
        }

        public double AutoLowestLongNoteModifyValue
        {
            get => _autoLowestLongNoteModifyValue;

            set
            {
                if (SetProperty(ref _autoLowestLongNoteModifyValue, value, nameof(AutoLowestLongNoteModifyValue)) && _isLoaded)
                {
                    ViewModels.Instance.MainValue.ModeComponentValue.SetAutoLowestLongNoteModify();
                }
            }
        }

        public double AutoHighestLongNoteModifyValue
        {
            get => _autoHighestLongNoteModifyValue;

            set
            {
                if (SetProperty(ref _autoHighestLongNoteModifyValue, value, nameof(AutoHighestLongNoteModifyValue)) && _isLoaded)
                {
                    ViewModels.Instance.MainValue.ModeComponentValue.SetAutoHighestLongNoteModify();
                }
            }
        }

        public bool AutoPutNoteSetMillis
        {
            get => _autoPutNoteSetMillis;

            set => SetProperty(ref _autoPutNoteSetMillis, value, nameof(AutoPutNoteSetMillis));
        }

        public double AutoPutNoteSetMillisValue
        {
            get => _autoPutNoteSetMillisValue;

            set => SetProperty(ref _autoPutNoteSetMillisValue, value, nameof(AutoPutNoteSetMillisValue));
        }

        public string Language
        {
            get => _language;

            set
            {
                if (SetProperty(ref _language, value) && _isLoaded)
                {
                    LanguageSystem.Instance.Init(value);
                    TwilightSystem.Instance.SendParallel(Event.Types.EventID.SetLanguage, value);
                    ViewModels.Instance.MainValue.NotifyModel();
                }
            }
        }

        public int FavorHunterStand { get; set; }

        public Version Date { get; set; }

        public long DefaultNoteDate { get; set; }

        public long DefaultUIDate { get; set; }

        public bool AutoGetDefaultNote { get; set; }

        public bool AutoGetDefaultUI { get; set; }

        public byte[] Cipher { get; set; }

        public ModeComponent ModeComponentValue { get; set; }

        public ModeComponentBundle[] ModeComponentBundles { get; set; }

        public double FastInputMillis { get; set; }

        public List<FavorJudgment> FavorJudgments { get; set; }

        public List<FavorHitPoints> FavorHitPoints { get; set; }

        public OUTPUTTYPE AudioVariety { get; set; }

        public int LastASIOAudioValueID { get; set; }

        public int LastWASAPIAudioValueID { get; set; }

        public List<DefaultEntryItem> DefaultEntryItems { get; set; }

        public HashSet<TutorialID> PassedTutorialIDs { get; set; }

        public bool IsLoaded { get; set; }

        public Dictionary<string, int> LastEntryItemPositions { get; set; }

        public InputBundle<DefaultInput> DefaultInputBundlesV6 { get; set; }

        public string SetPostItemInputText => string.Format(LanguageSystem.Instance.SetPostItemInputText, DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.PostItem0], DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.PostItem1]);

        public void NotifySetPostItemInputText()
        {
            OnPropertyChanged(nameof(SetPostItemInputText));
        }

        public InputBundle<HwDInput> DInputBundlesV4 { get; set; }

        public InputBundle<HwXInput> XInputBundlesV4 { get; set; }

        public InputBundle<WGI> WGIBundlesV3 { get; set; }

        public InputBundle<MIDI> MIDIBundlesV4 { get; set; }

        public double MIDIPBCSensitivity { get; set; }

        public int DInputXyzSensitivityV2 { get; set; }

        public bool IsDInputXyzSensitivityVisible => DetailedConfigure && DInputControllerVarietyValue == ControllerSystem.DInputControllerVariety.BMS;

        public double DInputIIDXSensitivity { get; set; }

        public bool IsDInputIIDXSensitivityVisible => DetailedConfigure && DInputControllerVarietyValue == ControllerSystem.DInputControllerVariety.IIDX;

        public bool SetHwMode
        {
            get => _setHwMode;

            set
            {
                if (SetProperty(ref _setHwMode, value, nameof(SetHwMode)) && _isLoaded)
                {
                    Utility.ModifyHwMode(HwModeV2Value);
                }
            }
        }

        public HwMode HwModeV2Value
        {
            get => _hwModeValue;

            set
            {
                if (SetProperty(ref _hwModeValue, value))
                {
                    OnSetHwModeV2Value();
                }
            }
        }

        public void OnSetHwModeV2Value()
        {
            if (_isLoaded)
            {
                Utility.ModifyHwMode(HwModeV2Value);
            }
        }

        public ControllerSystem.DInputControllerVariety DInputControllerVarietyValue
        {
            get => _dInputControllerVariety;

            set
            {
                if (SetProperty(ref _dInputControllerVariety, value))
                {
                    OnPropertyChanged(nameof(IsDInputXyzSensitivityVisible));
                    OnPropertyChanged(nameof(IsDInputIIDXSensitivityVisible));
                }
            }
        }

        public bool SetSalt { get; set; }

        public int LoopUnit { get; set; }

        public Component.InputMapping InputMappingValue { get; set; }

        public AudioVisualizerMode AudioVisualizerModeValue { get; set; }

        public ViewItem HandleFailedAudio { get; set; }

        public ViewItem ViewFailedDrawing { get; set; }

        public ViewItem ViewLowestJudgment { get; set; }

        public int HandleFailedAudioCount { get; set; }

        public double FailedDrawingMillis { get; set; }

        public bool[] InputWantInputMode { get; set; }

        public bool[] InputWantNoteVariety { get; set; }

        public bool IsTotalWantNoteVariety => InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMS] && InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.BMSON] && InputWantNoteVariety[(int)BaseNoteFile.NoteVariety.EventNote];

        public bool[] InputWantHandled { get; set; }

        public bool[] InputWantLevel { get; set; }

        public Brush InputWantWindowPaint => WantHellBPM ||
            !IsTotalWantNoteVariety ||
            InputWantHandled.Any(inputWantHandled => !inputWantHandled) ||
            InputWantInputMode.Skip(1).Any(inputWantInputMode => !inputWantInputMode) ||
            InputWantLevel.Any(inputWantLevel => !inputWantLevel) ||
            WantLevelTextValue ||
            WantBPM ||
            WantHighestInputCount ? Paints.Paint1 : Paints.Paint4;

        public void NotifyInputWantWindowPaint() => OnPropertyChanged(nameof(InputWantWindowPaint));

        public InputAudioVariety InputAudioVarietyValue { get; set; }

        public DefaultSpinningMode DefaultSpinningModeValue { get; set; }

        public AutoEnterSite AutoEnterDefaultSite { get; set; }

        public AutoEnterSite AutoEnterNotifySite { get; set; }

        public AutoEnterSite AutoEnterPlatformSite { get; set; }

        public double AudioInputValue { get; set; }

        public double Vibration0
        {
            get => _vibrations[0];

            set => SetProperty(ref _vibrations[0], value, nameof(Vibration0));
        }

        public double Vibration1
        {
            get => _vibrations[1];

            set => SetProperty(ref _vibrations[1], value, nameof(Vibration1));
        }

        public double Vibration2
        {
            get => _vibrations[2];

            set => SetProperty(ref _vibrations[2], value, nameof(Vibration2));
        }

        public double Vibration3
        {
            get => _vibrations[3];

            set => SetProperty(ref _vibrations[3], value, nameof(Vibration3));
        }

        public double TotalAudioVolume
        {
            get => _audioVolumes[AudioSystem.TotalAudio];

            set => SetProperty(ref _audioVolumes[AudioSystem.TotalAudio], value, nameof(TotalAudioVolume));
        }

        public double MainAudioVolume
        {
            get => _audioVolumes[AudioSystem.MainAudio];

            set => SetProperty(ref _audioVolumes[AudioSystem.MainAudio], value, nameof(MainAudioVolume));
        }

        public double InputAudioVolume
        {
            get => _audioVolumes[AudioSystem.InputAudio];

            set => SetProperty(ref _audioVolumes[AudioSystem.InputAudio], value, nameof(InputAudioVolume));
        }

        public double SEAudioVolume
        {
            get => _audioVolumes[AudioSystem.SEAudio];

            set => SetProperty(ref _audioVolumes[AudioSystem.SEAudio], value, nameof(SEAudioVolume));
        }

        public double WaveFadeVolume { get; set; }

        public double MainAreaFaint
        {
            get => _mainAreaFaint;

            set => SetProperty(ref _mainAreaFaint, value, nameof(MainAreaFaint));
        }

        public int CompilingBin { get; set; }

        public int LoadingBin { get; set; }

        public int UIBin { get; set; }

        public int AudioVisualizerCount { get; set; }

        public double JudgmentVisualizerMillis { get; set; }

        public bool AutoCompute
        {
            get => _autoCompute;

            set
            {
                if (SetProperty(ref _autoCompute, value, nameof(AutoCompute)))
                {
                    OnPropertyChanged(nameof(AutoComputePaint));
                    OnPropertyChanged(nameof(AutoComputeText));
                    OnPropertyChanged(nameof(AutoComputeContents));
                }
            }
        }

        public Brush AutoComputePaint => Paints.PointPaints[AutoCompute ? 1 : 0];

        public string AutoComputeText => AutoCompute ? LanguageSystem.Instance.AutoComputeText : LanguageSystem.Instance.NotAutoComputeText;

        public string AutoComputeContents => AutoCompute ? LanguageSystem.Instance.AutoComputeContents : LanguageSystem.Instance.NotAutoComputeContents;

        public bool WindowedMode
        {
            get => _windowedMode;

            set
            {
                if (SetProperty(ref _windowedMode, value, nameof(WindowedMode)))
                {
                    OnPropertyChanged(nameof(WindowedModeText));
                    OnPropertyChanged(nameof(WindowedModeContents));
                    OnPropertyChanged(nameof(WindowEllipse));
                }
            }
        }

        public string WindowedModeText => WindowedMode ? LanguageSystem.Instance.WindowedModeText : LanguageSystem.Instance.NotWindowedModeText;

        public string WindowedModeContents => WindowedMode ? LanguageSystem.Instance.WindowedModeContents : LanguageSystem.Instance.NotWindowedModeContents;

        public double WindowEllipse => WindowedMode && Environment.OSVersion.Version.Build >= 22000 ? Levels.WindowEllipse : 0.0;

        public string AvatarID { get; set; }

        public string BanalAudioFilePath { get; set; }

        public DefaultAudioFilePathItem[] DefaultAudioFilePathItems { get; set; }

        public DefaultAudioVariety DefaultAudioVarietyValue
        {
            get => _defaultAudioVariety;

            set
            {
                if (SetProperty(ref _defaultAudioVariety, value, nameof(DefaultAudioVarietyValue)))
                {
                    OnPropertyChanged(nameof(DefaultAudioVarietyText));
                    OnPropertyChanged(nameof(DefaultAudioVarietyPaint));
                }
            }
        }

        public string DefaultAudioVarietyText => DefaultAudioVarietyValue switch
        {
            DefaultAudioVariety.Not => LanguageSystem.Instance.NotDefaultAudioVarietyText,
            DefaultAudioVariety.UI => LanguageSystem.Instance.UIDefaultAudioVarietyText,
            DefaultAudioVariety.Favor => LanguageSystem.Instance.FavorDefaultAudioVarietyText,
            _ => default
        };

        public Brush DefaultAudioVarietyPaint => Paints.PointPaints[DefaultAudioVarietyValue != DefaultAudioVariety.Not ? 1 : 0];

        public string BanalMediaFilePath { get; set; }

        public string BanalFailedMediaFilePath { get; set; }

        public string BMSEditorFilePath { get; set; }

        public string BMSONEditorFilePath { get; set; }

        public BPMVariety BPMVarietyValue { get; set; }

        public FontFamily[] FontFamilyValues { get; set; }

        public void SetFontFamily()
        {
            FontFamilyValue = new(string.Join(',', FontFamilyValues.SkipLast(1).Select(fontFamily => fontFamily?.ToString() ?? string.Empty)));
            OnPropertyChanged(nameof(FontFamilyValue));
            OnPropertyChanged(nameof(FontFamilyValues));
            FontFace = new(FontFamilyValue, FontStyles.Normal, FontWeights.Normal, FontStretches.Normal);
            DrawingSystem.Instance.SetFontFamily();
            BaseUI.Instance.SetFontFamily();
            UI.Instance.SetFontFamily();
        }

        public int CommentViewTabPosition
        {
            get => _commentViewTabPosition;

            set
            {
                if (SetProperty(ref _commentViewTabPosition, value, nameof(CommentViewTabPosition)) && _isLoaded)
                {
                    var mainViewModel = ViewModels.Instance.MainValue;
                    mainViewModel.Computer?.InitNetComments();
                    mainViewModel.LoadCommentItemCollection();
                }
            }
        }

        public int HOFViewTabPosition
        {
            get => _hofViewTabPosition;

            set
            {
                if (SetProperty(ref _hofViewTabPosition, value, nameof(HOFViewTabPosition)) && _isLoaded)
                {
                    _ = ViewModels.Instance.MainValue.CallHOFAPI();
                }
            }
        }

        public int HOFViewTotalTabPosition
        {
            get => _hofViewTotalTabPosition;

            set
            {
                if (SetProperty(ref _hofViewTotalTabPosition, value, nameof(HOFViewTotalTabPosition)) && _isLoaded)
                {
                    _ = ViewModels.Instance.MainValue.CallHOFAPI();
                }
            }
        }

        public int HOFViewAtTabPosition
        {
            get => _hofViewAtTabPosition;

            set
            {
                if (SetProperty(ref _hofViewAtTabPosition, value, nameof(HOFViewAtTabPosition)) && _isLoaded)
                {
                    _ = ViewModels.Instance.MainValue.CallHOFAPI();
                }
            }
        }

        public int HOFViewAbilityTabPosition
        {
            get => _hofViewAbilityTabPosition;

            set
            {
                if (SetProperty(ref _hofViewAbilityTabPosition, value, nameof(HOFViewAbilityTabPosition)) && _isLoaded)
                {
                    _ = ViewModels.Instance.MainValue.CallHOFAPI();
                }
            }
        }

        [JsonIgnore]
        public FontFamily FontFamilyValue { get; set; } = new();

        [JsonIgnore]
        public Typeface FontFace { get; set; }

        public FitMode FitModeValue { get; set; }

        public bool Tube
        {
            get => _tube;

            set
            {
                if (SetProperty(ref _tube, value, nameof(Tube)))
                {
                    OnPropertyChanged(nameof(TubePaint));
                    OnPropertyChanged(nameof(TubeText));
                    if (_isLoaded)
                    {
                        AudioSystem.Instance.SetTube(value);
                    }
                }
            }
        }

        public Brush TubePaint => Paints.PointPaints[Tube ? 1 : 0];

        public string TubeText => Tube ? LanguageSystem.Instance.TubeText : LanguageSystem.Instance.NotTubeText;

        public bool AutoGetQwilight
        {
            get => !QwilightComponent.IsValve && _autoGetQwilight;

            set => _autoGetQwilight = value;
        }

        public bool SFX
        {
            get => _valueSFX;

            set
            {
                if (SetProperty(ref _valueSFX, value, nameof(SFX)))
                {
                    OnPropertyChanged(nameof(SFXPaint));
                    OnPropertyChanged(nameof(SFXText));
                    if (_isLoaded)
                    {
                        AudioSystem.Instance.SetSFX(value);
                    }
                }
            }
        }

        public Brush SFXPaint => Paints.PointPaints[SFX ? 1 : 0];

        public string SFXText => SFX ? LanguageSystem.Instance.SFXText : LanguageSystem.Instance.NotSFXText;

        public bool Flange
        {
            get => _flange;

            set
            {
                if (SetProperty(ref _flange, value, nameof(Flange)))
                {
                    OnPropertyChanged(nameof(FlangePaint));
                    OnPropertyChanged(nameof(FlangeText));
                    if (_isLoaded)
                    {
                        AudioSystem.Instance.SetFlange(value);
                    }
                }
            }
        }

        public Brush FlangePaint => Paints.PointPaints[Flange ? 1 : 0];

        public string FlangeText => Flange ? LanguageSystem.Instance.FlangeText : LanguageSystem.Instance.NotFlangeText;

        public bool Averager
        {
            get => _averager;

            set
            {
                if (SetProperty(ref _averager, value, nameof(Averager)))
                {
                    OnPropertyChanged(nameof(AveragerPaint));
                    OnPropertyChanged(nameof(AveragerText));
                    if (_isLoaded)
                    {
                        AudioSystem.Instance.SetAverager(value);
                    }
                }
            }
        }

        public Brush AveragerPaint => Paints.PointPaints[Averager ? 1 : 0];

        public string AveragerText => Averager ? LanguageSystem.Instance.AveragerText : LanguageSystem.Instance.NotAveragerText;

        public bool AudioMultiplierAtone
        {
            get => _audioMultiplierAtone;

            set
            {
                if (SetProperty(ref _audioMultiplierAtone, value, nameof(AudioMultiplierAtone)) && _isLoaded)
                {
                    AudioSystem.Instance.SetAudioMultiplierAtone(value, ViewModels.Instance.MainValue.ModeComponentValue.AudioMultiplier);
                }
            }
        }

        public double EqualizerHz0 { get; set; }

        public double EqualizerHz1 { get; set; }

        public double EqualizerHz2 { get; set; }

        public double EqualizerHz3 { get; set; }

        public double EqualizerHz4 { get; set; }

        public double Equalizer0
        {
            get => _equalizers[0];

            set => SetProperty(ref _equalizers[0], value, nameof(Equalizer0));
        }

        public double Equalizer1
        {
            get => _equalizers[1];

            set => SetProperty(ref _equalizers[1], value, nameof(Equalizer1));
        }

        public double Equalizer2
        {
            get => _equalizers[2];

            set => SetProperty(ref _equalizers[2], value, nameof(Equalizer2));
        }

        public double Equalizer3
        {
            get => _equalizers[3];

            set => SetProperty(ref _equalizers[3], value, nameof(Equalizer3));
        }

        public double Equalizer4
        {
            get => _equalizers[4];

            set => SetProperty(ref _equalizers[4], value, nameof(Equalizer4));
        }

        public ControllerSystem.VibrationMode VibrationModeValue
        {
            get => _vibrationMode;

            set
            {
                if (SetProperty(ref _vibrationMode, value))
                {
                    OnPropertyChanged(nameof(VibrationModeText));
                    OnPropertyChanged(nameof(IsVibrationMode));
                }
            }
        }

        public bool IsVibrationMode => VibrationModeValue != ControllerSystem.VibrationMode.Not;

        public string VibrationModeText => VibrationModeValue switch
        {
            ControllerSystem.VibrationMode.Not => LanguageSystem.Instance.NotVibrationModeText,
            ControllerSystem.VibrationMode.Input => LanguageSystem.Instance.InputVibrationModeText,
            ControllerSystem.VibrationMode.Failed => LanguageSystem.Instance.FailedVibrationModeText,
            _ => default,
        };

        public string GetCipher()
        {
            try
            {
                using var aes = Aes.Create();
                aes.KeySize = 256;
                return Encoding.UTF8.GetString(aes.CreateDecryptor(_aesCipher, _aesIV).TransformFinalBlock(Cipher, 0, Cipher.Length));
            }
            catch
            {
                return string.Empty;
            }
        }

        public void SetCipher(string valueCipher)
        {
            using var aes = Aes.Create();
            aes.KeySize = 256;
            var data = Encoding.UTF8.GetBytes(valueCipher);
            Cipher = aes.CreateEncryptor(_aesCipher, _aesIV).TransformFinalBlock(data, 0, data.Length);
        }

        public ControllerSystem.InputAPI ControllerInputAPI
        {
            get => _valueControllerInputAPI;

            set
            {
                if (SetProperty(ref _valueControllerInputAPI, value))
                {
                    OnPropertyChanged(nameof(ControllerInputAPIText));
                    OnPropertyChanged(nameof(ControllerInputAPIContents));
                    OnPropertyChanged(nameof(IsControllerInputAPIDInput));
                    OnPropertyChanged(nameof(IsControllerInputAPIWGI));
                }
            }
        }

        public string ControllerInputAPIText => ControllerInputAPI switch
        {
            ControllerSystem.InputAPI.DInput => LanguageSystem.Instance.DInputText,
            ControllerSystem.InputAPI.XInput => LanguageSystem.Instance.XInputText,
            ControllerSystem.InputAPI.WGI => LanguageSystem.Instance.WGIText,
            _ => null
        };

        public string ControllerInputAPIContents => ControllerInputAPI switch
        {
            ControllerSystem.InputAPI.DInput => LanguageSystem.Instance.DInputContents,
            ControllerSystem.InputAPI.XInput => LanguageSystem.Instance.XInputContents,
            ControllerSystem.InputAPI.WGI => LanguageSystem.Instance.WGIContents,
            _ => null
        };

        public bool IsControllerInputAPIDInput => ControllerInputAPI == ControllerSystem.InputAPI.DInput;

        public bool IsControllerInputAPIWGI => ControllerInputAPI == ControllerSystem.InputAPI.WGI;

        public DefaultControllerSystem.InputAPI DefaultControllerInputAPI
        {
            get => _defaultControllerInputAPI;

            set
            {
                if (SetProperty(ref _defaultControllerInputAPI, value))
                {
                    OnPropertyChanged(nameof(DefaultControllerInputAPIText));
                    OnPropertyChanged(nameof(DefaultControllerInputAPIContents));
                }
            }
        }

        public string DefaultControllerInputAPIText => DefaultControllerInputAPI switch
        {
            DefaultControllerSystem.InputAPI.DefaultInput => LanguageSystem.Instance.DefaultControllerDefaultInputText,
            DefaultControllerSystem.InputAPI.DInput => LanguageSystem.Instance.DefaultControllerDInputText,
            _ => null
        };

        public string DefaultControllerInputAPIContents => DefaultControllerInputAPI switch
        {
            DefaultControllerSystem.InputAPI.DefaultInput => LanguageSystem.Instance.DefaultControllerDefaultInputContents,
            DefaultControllerSystem.InputAPI.DInput => LanguageSystem.Instance.DefaultControllerDInputContents,
            _ => null
        };

        public WantBanned WantBannedValue
        {
            get => _wantBanned;

            set => SetProperty(ref _wantBanned, value, nameof(WantBannedText));
        }

        public string WantBannedText => WantBannedValue switch
        {
            WantBanned.Total => LanguageSystem.Instance.WantTotalBannedText,
            WantBanned.NotBanned => LanguageSystem.Instance.WantNotBannedText,
            _ => default,
        };

        public bool AutoHighlight
        {
            get => _autoHighlight;

            set
            {
                if (SetProperty(ref _autoHighlight, value))
                {
                    OnPropertyChanged(nameof(AutoHighlightText));
                    OnPropertyChanged(nameof(AutoHighlightContents));
                }
            }
        }

        public string AutoHighlightText => AutoHighlight ? LanguageSystem.Instance.AutoHighlightText : LanguageSystem.Instance.AutoLevyingText;

        public string AutoHighlightContents => AutoHighlight ? LanguageSystem.Instance.AutoHighlightContents : LanguageSystem.Instance.AutoLevyingContents;

        public bool IsXwindow
        {
            get => _isXwindow;

            set
            {
                if (SetProperty(ref _isXwindow, value))
                {
                    OnPropertyChanged(nameof(IsXwindowPaint));
                    OnPropertyChanged(nameof(IsXwindowText));
                    OnSetIsXwindow();
                }
            }
        }

        public Brush IsXwindowPaint => Paints.PointPaints[IsXwindow ? 1 : 0];

        public string IsXwindowText => IsXwindow ? LanguageSystem.Instance.XwindowText : LanguageSystem.Instance.NotXwindowText;

        void OnSetIsXwindow()
        {
            if (_isLoaded)
            {
                if (IsXwindow)
                {
                    XwindowSystem.Instance.HandleSystem();
                }
                else
                {
                    XwindowSystem.Instance.Stop();
                }
            }
        }

        public bool BW
        {
            get => _bw;

            set
            {
                if (SetProperty(ref _bw, value))
                {
                    OnPropertyChanged(nameof(BWPaint));
                    OnPropertyChanged(nameof(BWText));
                }
            }
        }

        public Brush BWPaint => Paints.PointPaints[BW ? 1 : 0];

        public string BWText => BW ? LanguageSystem.Instance.BWText : LanguageSystem.Instance.NotBWText;

        public bool LS
        {
            get => _ls;

            set
            {
                if (SetProperty(ref _ls, value))
                {
                    OnPropertyChanged(nameof(LSPaint));
                    OnPropertyChanged(nameof(LSText));
                }
            }
        }

        public Brush LSPaint => Paints.PointPaints[LS ? 1 : 0];

        public string LSText => LS ? LanguageSystem.Instance.LSText : LanguageSystem.Instance.NotLSText;

        public bool Aura
        {
            get => _aura;

            set
            {
                if (SetProperty(ref _aura, value))
                {
                    OnPropertyChanged(nameof(AuraPaint));
                    OnPropertyChanged(nameof(AuraText));
                }
            }
        }

        public Brush AuraPaint => Paints.PointPaints[Aura ? 1 : 0];

        public string AuraText => Aura ? LanguageSystem.Instance.AuraText : LanguageSystem.Instance.NotAuraText;

        public bool K70
        {
            get => _valueK70;

            set
            {
                if (SetProperty(ref _valueK70, value))
                {
                    OnPropertyChanged(nameof(K70Paint));
                    OnPropertyChanged(nameof(K70Text));
                }
            }
        }

        public Brush K70Paint => Paints.PointPaints[K70 ? 1 : 0];

        public string K70Text => K70 ? LanguageSystem.Instance.K70Text : LanguageSystem.Instance.NotK70Text;

        public bool MediaInput
        {
            get => _mediaInput;

            set => SetProperty(ref _mediaInput, value, nameof(MediaInput));
        }

        public bool LoadedMedia
        {
            get => _loadedMedia;

            set => SetProperty(ref _loadedMedia, value, nameof(LoadedMedia));
        }

        public bool Media
        {
            get => _media;

            set
            {
                if (SetProperty(ref _media, value))
                {
                    OnPropertyChanged(nameof(MediaPaint));
                    OnPropertyChanged(nameof(MediaText));
                    if (_isLoaded)
                    {
                        var handlingComputer = ViewModels.Instance.MainValue.GetHandlingComputer();
                        if (handlingComputer != null)
                        {
                            handlingComputer.SetWait();
                            MediaSystem.Instance.HandleDefaultIfAvailable(handlingComputer);
                            MediaSystem.Instance.HandleIfAvailable(handlingComputer);
                        }
                    }
                }
            }
        }

        public Brush MediaPaint => Paints.PointPaints[Media ? 1 : 0];

        public string MediaText => Media ? LanguageSystem.Instance.MediaText : LanguageSystem.Instance.NotMediaText;

        public bool HandleInputAudio
        {
            get => _handleInputAudio;

            set
            {
                if (SetProperty(ref _handleInputAudio, value))
                {
                    OnPropertyChanged(nameof(HandleInputAudioPaint));
                    OnPropertyChanged(nameof(HandleInputAudioText));
                    OnPropertyChanged(nameof(IsBanalAudioVisible));
                    if (_isLoaded)
                    {
                        var audioValueName = AudioSystem.Instance.AudioValue?.Name;
                        if (!string.IsNullOrEmpty(audioValueName))
                        {
                            AudioConfigureValues[audioValueName].HandleInputAudio = value;
                        }
                    }
                }
            }
        }

        public Brush HandleInputAudioPaint => Paints.PointPaints[HandleInputAudio ? 1 : 0];

        public string HandleInputAudioText => HandleInputAudio ? LanguageSystem.Instance.HandleInputAudioText : LanguageSystem.Instance.NotHandleInputAudioText;

        public bool BanalAudio
        {
            get => _banalAudio;

            set
            {
                if (SetProperty(ref _banalAudio, value))
                {
                    OnPropertyChanged(nameof(BanalAudioPaint));
                    OnPropertyChanged(nameof(BanalAudioText));
                }
            }
        }

        public Brush BanalAudioPaint => Paints.PointPaints[BanalAudio ? 1 : 0];

        public string BanalAudioText => BanalAudio ? LanguageSystem.Instance.BanalAudioText : LanguageSystem.Instance.NotBanalAudioText;

        public bool IsBanalAudioVisible => DetailedConfigure && HandleInputAudio;

        public bool BanalMedia
        {
            get => _banalMedia;

            set
            {
                if (SetProperty(ref _banalMedia, value, nameof(BanalMedia)))
                {
                    OnPropertyChanged(nameof(BanalMediaPaint));
                    OnPropertyChanged(nameof(BanalMediaText));
                }
            }
        }

        public Brush BanalMediaPaint => Paints.PointPaints[BanalMedia ? 1 : 0];

        public string BanalMediaText => BanalMedia ? LanguageSystem.Instance.BanalMediaText : LanguageSystem.Instance.NotBanalMediaText;

        public bool AlwaysBanalMedia { get; set; }

        public bool BanalFailedMedia
        {
            get => _banalFailedMedia;

            set
            {
                if (SetProperty(ref _banalFailedMedia, value, nameof(BanalFailedMedia)))
                {
                    OnPropertyChanged(nameof(BanalFailedMediaPaint));
                    OnPropertyChanged(nameof(BanalFailedMediaText));
                }
            }
        }

        public Brush BanalFailedMediaPaint => Paints.PointPaints[BanalFailedMedia ? 1 : 0];

        public string BanalFailedMediaText => BanalFailedMedia ? LanguageSystem.Instance.BanalFailedMediaText : LanguageSystem.Instance.NotBanalFailedMediaText;

        public bool AlwaysBanalFailedMedia { get; set; }

        public bool WaveIn
        {
            get => _waveIn;

            set => SetProperty(ref _waveIn, value, nameof(WaveIn));
        }

        public bool Wave
        {
            get => _wave;

            set => SetProperty(ref _wave, value, nameof(Wave));
        }

        public bool AudioInput
        {
            get => _audioInput;

            set
            {
                if (SetProperty(ref _audioInput, value, nameof(AudioInput)) && _isLoaded)
                {
                    ViewModels.Instance.HandleSiteViewModels(siteViewModel => siteViewModel.NotifyIsSendingAudioInput());
                }
            }
        }

        public bool UIPipelineLimiter
        {
            get => _valueUIPipelineLimiter;

            set
            {
                if (SetProperty(ref _valueUIPipelineLimiter, value, nameof(UIPipelineLimiter)))
                {
                    OnPropertyChanged(nameof(UIPipelineLimiterPaint));
                }
            }
        }

        public Brush UIPipelineLimiterPaint => Paints.PointPaints[UIPipelineLimiter ? 1 : 0];

        public bool UIPipelineMainDrawingPaint
        {
            get => _valueUIPipelineMainDrawingPaint;

            set => SetProperty(ref _valueUIPipelineMainDrawingPaint, value, nameof(UIPipelineMainDrawingPaintPaint));
        }

        public Brush UIPipelineMainDrawingPaintPaint => Paints.PointPaints[UIPipelineMainDrawingPaint ? 1 : 0];

        public bool UIPipelineJudgmentMain
        {
            get => _valueUIPipelineJudgmentMain;

            set => SetProperty(ref _valueUIPipelineJudgmentMain, value, nameof(UIPipelineJudgmentMainPaint));
        }

        public Brush UIPipelineJudgmentMainPaint => Paints.PointPaints[UIPipelineJudgmentMain ? 1 : 0];

        public bool UIPipelineJudgmentCount
        {
            get => _valueUIPipelineJudgmentCount;

            set => SetProperty(ref _valueUIPipelineJudgmentCount, value, nameof(UIPipelineJudgmentCountPaint));
        }

        public Brush UIPipelineJudgmentCountPaint => Paints.PointPaints[UIPipelineJudgmentCount ? 1 : 0];

        public bool UIPipelineJudgmentMeter
        {
            get => _valueUIPipelineJudgmentMeter;

            set
            {
                if (SetProperty(ref _valueUIPipelineJudgmentMeter, value, nameof(UIPipelineJudgmentMeter)))
                {
                    OnPropertyChanged(nameof(UIPipelineJudgmentMeterPaint));
                }
            }
        }

        public Brush UIPipelineJudgmentMeterPaint => Paints.PointPaints[UIPipelineJudgmentMeter ? 1 : 0];

        public bool UIPipelineNet
        {
            get => _valueUIPipelineNet;

            set
            {
                if (SetProperty(ref _valueUIPipelineNet, value, nameof(UIPipelineNet)))
                {
                    OnPropertyChanged(nameof(UIPipelineNetPaint));
                }
            }
        }

        public Brush UIPipelineNetPaint => Paints.PointPaints[UIPipelineNet ? 1 : 0];

        public bool UIPipelineHunter
        {
            get => _valueUIPipelineHunter;

            set
            {
                if (SetProperty(ref _valueUIPipelineHunter, value, nameof(UIPipelineHunter)))
                {
                    OnPropertyChanged(nameof(UIPipelineHunterPaint));
                }
            }
        }

        public Brush UIPipelineHunterPaint => Paints.PointPaints[UIPipelineHunter ? 1 : 0];

        public bool UIPipelineJudgmentVisualizer
        {
            get => _valueUIPipelineJudgmentVisualizer;

            set
            {
                if (SetProperty(ref _valueUIPipelineJudgmentVisualizer, value, nameof(UIPipelineJudgmentVisualizer)))
                {
                    OnPropertyChanged(nameof(UIPipelineJudgmentVisualizerPaint));
                }
            }
        }

        public Brush UIPipelineJudgmentVisualizerPaint => Paints.PointPaints[UIPipelineJudgmentVisualizer ? 1 : 0];

        public bool UIPipelineJudgmentPaint
        {
            get => _valueUIPipelineJudgmentPaint;

            set => SetProperty(ref _valueUIPipelineJudgmentPaint, value, nameof(UIPipelineJudgmentPaint));
        }

        public Brush UIPipelineJudgmentPaintPaint => Paints.PointPaints[UIPipelineJudgmentPaint ? 1 : 0];

        public bool UIPipelineHitNotePaint
        {
            get => _valueUIPipelineHitNotePaint;

            set => SetProperty(ref _valueUIPipelineHitNotePaint, value, nameof(UIPipelineHitNotePaintPaint));
        }

        public Brush UIPipelineHitNotePaintPaint => Paints.PointPaints[UIPipelineHitNotePaint ? 1 : 0];

        public bool UIPipelineBPM
        {
            get => _valueUIPipelineBPM;

            set
            {
                if (SetProperty(ref _valueUIPipelineBPM, value, nameof(UIPipelineBPM)))
                {
                    OnPropertyChanged(nameof(UIPipelineBPMPaint));
                }
            }
        }

        public Brush UIPipelineBPMPaint => Paints.PointPaints[UIPipelineBPM ? 1 : 0];

        public bool UICommentNote
        {
            get => _valueUICommentNote;

            set => SetProperty(ref _valueUICommentNote, value, nameof(UICommentNotePaint));
        }

        public Brush UICommentNotePaint => Paints.PointPaints[UICommentNote ? 1 : 0];

        public bool UIPipelineMainJudgmentMeter
        {
            get => _valueUIPipelineMainJudgmentMeter;

            set => SetProperty(ref _valueUIPipelineMainJudgmentMeter, value, nameof(UIPipelineMainJudgmentMeterPaint));
        }

        public Brush UIPipelineMainJudgmentMeterPaint => Paints.PointPaints[UIPipelineMainJudgmentMeter ? 1 : 0];

        public bool UIPipelineJudgmentInputVisualizer
        {
            get => _valueUIPipelineJudgmentInputVisualizer;

            set
            {
                if (SetProperty(ref _valueUIPipelineJudgmentInputVisualizer, value, nameof(UIPipelineJudgmentInputVisualizer)))
                {
                    OnPropertyChanged(nameof(UIPipelineJudgmentInputVisualizerPaint));
                }
            }
        }

        public Brush UIPipelineJudgmentInputVisualizerPaint => Paints.PointPaints[UIPipelineJudgmentInputVisualizer ? 1 : 0];

        public bool LowHitPointsFaintUI
        {
            get => _lowHitPointsFaintUI;

            set
            {
                if (SetProperty(ref _lowHitPointsFaintUI, value))
                {
                    OnPropertyChanged(nameof(LowHitPointsFaintUIPaint));
                    OnPropertyChanged(nameof(LowHitPointsFaintUIText));
                }
            }
        }

        public Brush LowHitPointsFaintUIPaint => LowHitPointsFaintUI ? Paints.Paint1 : Paints.Paint4;

        public string LowHitPointsFaintUIText => LowHitPointsFaintUI ? LanguageSystem.Instance.LowHitPointsFaintUIText : LanguageSystem.Instance.NotLowHitPointsFaintUIText;

        public bool AudioVisualizer
        {
            get => _audioVisualizer;

            set
            {
                if (SetProperty(ref _audioVisualizer, value, nameof(AudioVisualizer)) && _isLoaded)
                {
                    AudioSystem.Instance.SetAudioVisualizer(value);
                }
            }
        }

        public bool Equalizer
        {
            get => _equalizer;

            set
            {
                if (SetProperty(ref _equalizer, value, nameof(Equalizer)))
                {
                    OnPropertyChanged(nameof(EqualizerPaint));
                    if (_isLoaded)
                    {
                        AudioSystem.Instance.SetEqualizer(value);
                    }
                }
            }
        }

        public Brush EqualizerPaint => Paints.PointPaints[Equalizer ? 1 : 0];

        public bool TotalLimiterVariety
        {
            get => _totalLimiterVariety;

            set => SetProperty(ref _totalLimiterVariety, value, nameof(TotalLimiterVariety));
        }

        public bool AutoableLimiterVariety { get; set; }

        public bool CenterLimiterVariety { get; set; }

        public bool Limiter57Variety { get; set; }

        public bool IsMediaFill
        {
            get => _isMediaFill;

            set
            {
                if (SetProperty(ref _isMediaFill, value))
                {
                    OnPropertyChanged(nameof(IsMediaFillText));
                    OnPropertyChanged(nameof(IsMediaFillContents));
                }
            }
        }

        public string IsMediaFillText => IsMediaFill ? LanguageSystem.Instance.MediaFillText : LanguageSystem.Instance.NotMediaFillText;

        public string IsMediaFillContents => IsMediaFill ? LanguageSystem.Instance.MediaFillContents : LanguageSystem.Instance.NotMediaFillContents;

        public bool LostPointAudio
        {
            get => _lostPointAudio;

            set
            {
                if (SetProperty(ref _lostPointAudio, value))
                {
                    OnPropertyChanged(nameof(LostPointAudioPaint));
                    OnPropertyChanged(nameof(LostPointAudioText));
                }
            }
        }

        public Brush LostPointAudioPaint => Paints.PointPaints[LostPointAudio ? 1 : 0];

        public string LostPointAudioText => LostPointAudio ? LanguageSystem.Instance.LostPointAudioText : LanguageSystem.Instance.NotLostPointAudioText;

        public Dictionary<string, string> LastInputWants { get; set; }

        [JsonIgnore]
        public string InputWant
        {
            get => LastInputWants.GetValueOrDefault(LastDefaultEntryItem?.DefaultEntryPath ?? string.Empty, string.Empty);

            set => LastInputWants[LastDefaultEntryItem?.DefaultEntryPath ?? string.Empty] = value;
        }

        public bool SaltAuto
        {
            get => _saltAuto;

            set => SetProperty(ref _saltAuto, value, nameof(SaltAuto));
        }

        public bool IsFailMode
        {
            get => _isFailMode;

            set
            {
                if (SetProperty(ref _isFailMode, value))
                {
                    OnPropertyChanged(nameof(IsFailModeText));
                    OnPropertyChanged(nameof(IsFailModeContents));
                }
            }
        }

        public string IsFailModeText => IsFailMode ? LanguageSystem.Instance.IsFailModeText : LanguageSystem.Instance.NotIsFailModeText;

        public string IsFailModeContents => IsFailMode ? LanguageSystem.Instance.IsFailModeContents : LanguageSystem.Instance.NotIsFailModeContents;

        public bool WantLevelSystem
        {
            get => _wantLevelSystem;

            set
            {
                if (SetProperty(ref _wantLevelSystem, value, nameof(WantLevelSystem)))
                {
                    OnPropertyChanged(nameof(WantLevelNameText));
                }
            }
        }

        public string WantLevelName
        {
            get => _wantLevelName;

            set
            {
                if (SetProperty(ref _wantLevelName, value, nameof(WantLevelName)))
                {
                    OnPropertyChanged(nameof(WantLevelNameText));
                }
            }
        }

        public string WantLevelNameText => !WantLevelSystem || string.IsNullOrEmpty(WantLevelName) ? "🔖" : WantLevelName;

        public string[] WantLevelIDs { get; set; }

        public bool AutoSignIn
        {
            get => _autoSignIn;

            set => SetProperty(ref _autoSignIn, value, nameof(AutoSignIn));
        }

        public uint AudioDataLength
        {
            get => _audioDataLength;

            set => SetProperty(ref _audioDataLength, value, nameof(AudioDataLength));
        }

        public int LowestWantLevelTextValue
        {
            get => _lowestWantLevelTextValue;

            set => SetProperty(ref _lowestWantLevelTextValue, value, nameof(LowestWantLevelTextValue));
        }

        public int HighestWantLevelTextValue
        {
            get => _highestWantLevelContensValue;

            set => SetProperty(ref _highestWantLevelContensValue, value, nameof(HighestWantLevelTextValue));
        }

        public bool WantLevelTextValue
        {
            get => _wantLevelTextValue;

            set => SetProperty(ref _wantLevelTextValue, value, nameof(WantLevelTextValue));
        }

        public int LowestWantBPM
        {
            get => _lowestWantBPM;

            set => SetProperty(ref _lowestWantBPM, value, nameof(LowestWantBPM));
        }

        public int HighestWantBPM
        {
            get => _highestWantBPM;

            set => SetProperty(ref _highestWantBPM, value, nameof(HighestWantBPM));
        }

        public bool WantBPM
        {
            get => _wantBPM;

            set => SetProperty(ref _wantBPM, value, nameof(WantBPM));
        }

        public int LowestWantAverageInputCount
        {
            get => _lowestWantAverageInputCount;

            set => SetProperty(ref _lowestWantAverageInputCount, value, nameof(LowestWantAverageInputCount));
        }

        public int HighestWantAverageInputCount
        {
            get => _highestWantAverageInputCount;

            set => SetProperty(ref _highestWantAverageInputCount, value, nameof(HighestWantAverageInputCount));
        }
        public bool WantAverageInputCount
        {
            get => _wantAverageInputCount;

            set => SetProperty(ref _wantAverageInputCount, value, nameof(WantAverageInputCount));
        }

        public int LowestWantHighestInputCount
        {
            get => _lowestWantHighestInputCount;

            set => SetProperty(ref _lowestWantHighestInputCount, value, nameof(LowestWantHighestInputCount));
        }

        public int HighestWantHighestInputCount
        {
            get => _highestWantHighestInputCount;

            set => SetProperty(ref _highestWantHighestInputCount, value, nameof(HighestWantHighestInputCount));
        }

        public bool WantHighestInputCount
        {
            get => _wantHighestInputCount;

            set => SetProperty(ref _wantHighestInputCount, value, nameof(WantHighestInputCount));
        }

        public int WindowPosition0V2 { get; set; }

        public int WindowPosition1V2 { get; set; }

        public int WindowLengthV2
        {
            get => _windowLength;

            set => SetProperty(ref _windowLength, value, nameof(WindowLengthV2));
        }

        public int WindowHeightV2
        {
            get => _windowHeight;

            set => SetProperty(ref _windowHeight, value, nameof(WindowHeightV2));
        }

        public double AudioWait
        {
            get => _audioWait;

            set => SetProperty(ref _audioWait, value, nameof(AudioWait));
        }

        public double BanalAudioWait
        {
            get => _banalAudioWait;

            set
            {
                if (SetProperty(ref _banalAudioWait, value, nameof(BanalAudioWait)) && _isLoaded)
                {
                    var audioValueName = AudioSystem.Instance.AudioValue?.Name;
                    if (audioValueName != null)
                    {
                        AudioConfigureValues[audioValueName].AudioWait = value;
                    }
                }
            }
        }

        public double MediaWait
        {
            get => _mediaWait;

            set => SetProperty(ref _mediaWait, value, nameof(MediaWait));
        }

        public double BanalMediaWait { get; set; }

        public DefaultEntryItem LastDefaultEntryItem
        {
            get => _lastDefaultEntryItem;

            set
            {
                if (SetProperty(ref _lastDefaultEntryItem, value, nameof(LastDefaultEntryItem)))
                {
                    OnPropertyChanged(nameof(InputWant));
                }
            }
        }

        public UIItem UIItemValue
        {
            get => _valueUIItem;

            set
            {
                if (SetProperty(ref _valueUIItem, value, nameof(UIItemValue)))
                {
                    OnPropertyChanged(nameof(UIConfigureValue));
                }
            }
        }

        public UIItem BaseUIItemValue
        {
            get => _valueBaseUIItemValue;

            set => SetProperty(ref _valueBaseUIItemValue, value, nameof(BaseUIItemValue));
        }

        public bool AlwaysNotP2Position
        {
            get => _alwaysNotP2Position;

            set
            {
                if (SetProperty(ref _alwaysNotP2Position, value))
                {
                    OnPropertyChanged(nameof(AlwaysNotP2PositionText));
                    OnPropertyChanged(nameof(AlwaysNotP2PositionContents));
                }
            }
        }

        public string AlwaysNotP2PositionText => AlwaysNotP2Position ? LanguageSystem.Instance.AlwaysNotP2PositionText : LanguageSystem.Instance.AlwaysP2PositionText;

        public string AlwaysNotP2PositionContents => AlwaysNotP2Position ? LanguageSystem.Instance.AlwaysNotP2PositionContents : LanguageSystem.Instance.AlwaysP2PositionContents;

        public bool GroupEntry { get; set; }

        public HunterVariety HunterVarietyV2Value
        {
            get => _hunterVariety;

            set => SetProperty(ref _hunterVariety, value, nameof(IsFavorHunterVariety));
        }

        public bool IsFavorHunterVariety => HunterVarietyV2Value.Mode == HunterVariety.HunterVarietyFavor;

        public bool NetCommentFollow
        {
            get => _netCommentFollow;

            set
            {
                if (SetProperty(ref _netCommentFollow, value))
                {
                    OnPropertyChanged(nameof(NetCommentFollowText));
                    OnPropertyChanged(nameof(NetCommentFollowContents));
                }
            }
        }

        public string NetCommentFollowText => NetCommentFollow ? LanguageSystem.Instance.NetCommentFollowText : LanguageSystem.Instance.NetCommentNotFollowText;

        public string NetCommentFollowContents => NetCommentFollow ? LanguageSystem.Instance.NetCommentFollowContents : LanguageSystem.Instance.NetCommentNotFollowContents;

        public bool WantHellBPM
        {
            get => _wantHellBPM;

            set => SetProperty(ref _wantHellBPM, value, nameof(WantHellBPMText));
        }

        public string WantHellBPMText => WantHellBPM ? LanguageSystem.Instance.HellBPMText : LanguageSystem.Instance.NotHellBPMText;

        public int NetItemCount { get; set; }

        public void Save(bool isParallel)
        {
            if (isParallel)
            {
                Task.Run(SaveImpl);
            }
            else
            {
                SaveImpl();
            }

            void SaveImpl()
            {
                lock (_setSaveCSX)
                {
                    ModeComponentValue = ViewModels.Instance.MainValue.ModeComponentValue;

                    Utility.CopyFile(_fileName, _tmp0FileName);
                    Utility.MoveFile(_tmp0FileName, _tmp1FileName);
                    File.WriteAllText(_fileName, Utility.SetJSON(this, _defaultJSONConfigure), Encoding.UTF8);
                    Utility.WipeFile(_tmp1FileName);
                }
            }
        }

        public void Validate(bool isInit)
        {
            if (isInit || LoadingBin == default)
            {
                LoadingBin = QwilightComponent.CPUCount;
            }
            if (isInit || CompilingBin == default)
            {
                CompilingBin = QwilightComponent.CPUCount;
            }
            if (isInit || AvatarID == default)
            {
                AvatarID = string.Empty;
            }
            if (isInit || Cipher == default)
            {
                Cipher = Array.Empty<byte>();
            }
            if (isInit)
            {
                ControllerInputAPI = ControllerSystem.InputAPI.DInput;
                AutoSignIn = false;
                IsFailMode = true;
                LowHitPointsFaintUI = true;
                IsLoaded = false;
                AutoHighlight = true;
                InputMappingValue = Component.InputMapping.Mapping1;
                IsMediaFill = true;
                AudioWait = 0.0;
                SaltAuto = false;
                Equalizer = false;
                InitEqualizers(1);
                Vibration0 = 1.0;
                Vibration1 = 1.0;
                Vibration2 = 1.0;
                Vibration3 = 1.0;
                VibrationModeValue = ControllerSystem.VibrationMode.Input;
                NoteFormatID = -1;
                AlwaysNotP2Position = false;
                IsXwindow = false;
            }
            if (isInit || AudioVariety == default)
            {
                AudioVariety = OUTPUTTYPE.WASAPI;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 5, 8))
            {
                DefaultEntryItems = new();
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 8, 7))
            {
                UIBin = QwilightComponent.CPUCount;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 8, 12))
            {
                AudioVisualizerCount = 80;
                AudioVisualizerModeValue = AudioVisualizerMode.AudioVisualizerMode2;
                JudgmentVisualizerMillis = 1000.0;
                UIPipelineJudgmentCount = true;
                UIPipelineJudgmentMeter = true;
                UIPipelineJudgmentVisualizer = true;
                UIPipelineJudgmentPaint = true;
                UIPipelineHitNotePaint = true;
                UIPipelineBPM = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 9, 0))
            {
                UIPipelineMainDrawingPaint = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 10, 0))
            {
                HandleInputAudio = true;
                AudioInput = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 10, 2))
            {
                WaveIn = true;
                Wave = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 10, 9))
            {
                MediaWait = 0.0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 10, 14))
            {
                DefaultEntryItems.Add(new()
                {
                    DefaultEntryVarietyValue = DefaultEntryItem.DefaultEntryVariety.Favorite,
                    DefaultEntryPath = Guid.NewGuid().ToString(),
                    FavoriteEntryName = "Favorites ⭐"
                });
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 10, 19))
            {
                CommentViewTabPosition = 0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 10, 20))
            {
                MainAudioVolume = 1.0;
                InputAudioVolume = 1.0;
                TotalAudioVolume = 1.0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 10, 23))
            {
                WantBPM = false;
                LowestWantBPM = 60;
                HighestWantBPM = 240;
                WantHighestInputCount = false;
                LowestWantHighestInputCount = 8;
                HighestWantHighestInputCount = 16;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 10, 30))
            {
                AudioMultiplierAtone = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 10, 32))
            {
                AutoCompute = true;
                UIPipelineLimiter = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 11, 1))
            {
                WindowedMode = true;
            }
            if (Utility.IsLowerDate(Date, 1, 11, 4))
            {
                UIItemValue = new()
                {
                    UIEntry = "Default",
                    YamlName = "Default"
                };
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 11, 5))
            {
                NetItemCount = 8;
                UIPipelineNet = true;
                UIPipelineHunter = true;
                NetCommentFollow = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 11, 6))
            {
                TotalLimiterVariety = true;
                AutoableLimiterVariety = false;
                CenterLimiterVariety = false;
            }
            if (Utility.IsLowerDate(Date, 1, 11, 9))
            {
                BaseUIItemValue = new()
                {
                    UIEntry = "@Default",
                    YamlName = "@Default"
                };
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 11, 9))
            {
                WantLevelTextValue = false;
                LowestWantLevelTextValue = 1;
                HighestWantLevelTextValue = 12;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 11, 36))
            {
                GroupEntry = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 12, 0))
            {
                MediaInput = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 25))
            {
                InitColors(1);
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 32))
            {
                InitEqualizers(2);
                SFX = false;
                Flange = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 33))
            {
                AutoPutNoteSetMillisValue = 16.0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 38))
            {
                Limiter57Variety = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 51))
            {
                HandleFailedAudio = new()
                {
                    Data = ViewItem.Always
                };
                ViewFailedDrawing = new()
                {
                    Data = ViewItem.Always
                };
                ViewLowestJudgment = new()
                {
                    Data = ViewItem.Always
                };
                HandleFailedAudioCount = 5;
                FailedDrawingMillis = 500.0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 54))
            {
                AudioVisualizer = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 61))
            {
                BPMVarietyValue = new()
                {
                    Data = BPMVariety.AudioMultiplier
                };
                HunterVarietyV2Value = new()
                {
                    Mode = HunterVariety.HunterVarietyHigher
                };
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 65))
            {
                LoopUnit = 1000;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 94))
            {
                AutoEnterNotifySite = new()
                {
                    Data = AutoEnterSite.AutoEnter
                };
                AutoEnterDefaultSite = new()
                {
                    Data = AutoEnterSite.AutoEnter
                };
                AutoEnterPlatformSite = new()
                {
                    Data = AutoEnterSite.AutoEnter
                };
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 101))
            {
                Aura = false;
                K70 = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 110))
            {
                MIDIPBCSensitivity = 100;
                BW = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 128))
            {
                WindowPosition0V2 = 0;
                WindowPosition1V2 = 0;
                WindowLengthV2 = 1280;
                WindowHeightV2 = 720;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 133))
            {
                SEAudioVolume = 1.0;
                AudioInputValue = 0.0;
                WaveFadeVolume = 50.0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 13, 138))
            {
                VESAV2 = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 0))
            {
                FavorMediaInput = false;
                InitMediaInputArea();
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 5))
            {
                DInputXyzSensitivityV2 = 100;
                LostPointAudio = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 6))
            {
                InputAudioVarietyValue = InputAudioVariety.IIDX;
                FavorJudgments = new()
                {
                    new()
                    {
                        Name = "LR2 EZ",
                        Value = new double[6][]
                        {
                            new double[] { -21.0, 21.0 },
                            new double[] { -40.5, 40.5 },
                            new double[] { -60.0, 60.0 },
                            new double[] { -120.0, 120.0 },
                            new double[] { -200.0, 200.0 },
                            new double[] { -1000.0, 200.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "LR2 NM",
                        Value = new double[6][]
                        {
                            new double[] { -18.0, 18.0 },
                            new double[] { -29.0, 29.0 },
                            new double[] { -40.0, 40.0 },
                            new double[] { -100.0, 100.0 },
                            new double[] { -200.0, 200.0 },
                            new double[] { -1000.0, 200.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "LR2 HD",
                        Value = new double[6][]
                        {
                            new double[] { -15.0, 15.0 },
                            new double[] { -22.5, 22.5 },
                            new double[] { -30.0, 30.0 },
                            new double[] { -60.0, 60.0 },
                            new double[] { -200.0, 200.0 },
                            new double[] { -1000.0, 200.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "LR2 VHD",
                        Value = new double[6][]
                        {
                            new double[] { -8.0, 8.0 },
                            new double[] { -16.0, 16.0 },
                            new double[] { -24.0, 24.0 },
                            new double[] { -40.0, 40.0 },
                            new double[] { -200.0, 200.0 },
                            new double[] { -1000.0, 200.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "BEATORAJA EZ",
                        Value = new double[6][]
                        {
                            new double[] { -20.0, 20.0 },
                            new double[] { -40.0, 40.0 },
                            new double[] { -60.0, 60.0 },
                            new double[] { -150.0, 150.0 },
                            new double[] { -220.0, 280.0 },
                            new double[] { -500.0, 280.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "BEATORAJA NM",
                        Value =  new double[6][]
                        {
                            new double[] { -15.0, 15.0 },
                            new double[] { -30.0, 30.0 },
                            new double[] { -45.0, 45.0 },
                            new double[] { -112.5, 112.5 },
                            new double[] { -275.0, 350.0 },
                            new double[] { -500.0, 350.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "BEATORAJA HD",
                        Value = new double[6][]
                        {
                            new double[] { -10.0, 10.0 },
                            new double[] { -20.0, 20.0 },
                            new double[] { -30.0, 30.0 },
                            new double[] { -75.0, 75.0 },
                            new double[] { -330.0, 420.0 },
                            new double[] { -500.0, 420.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "BEATORAJA VHD",
                        Value = new double[6][]
                        {
                            new double[] { -5.0, 5.0 },
                            new double[] { -10.0, 10.0 },
                            new double[] { -15.0, 15.0 },
                            new double[] { -37.5, 37.5 },
                            new double[] { -385.0, 490.0 },
                            new double[] { -500.0, 490.0 }
                        },
                        IsDefault = true
                    }
                };
                FavorHitPoints = new()
                {
                    new()
                    {
                        Name = "LR2 EZ",
                        Value = new double[6][]
                        {
                            new double[] { 120.0, 0.0 },
                            new double[] { 120.0, 0.0 },
                            new double[] { 120.0, 0.0 },
                            new double[] { 60.0, 0.0 },
                            new double[] { 0.0, -4.8 },
                            new double[] { 0.0, -3.2 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "LR2 NM",
                        Value = new double[6][]
                        {
                            new double[] { 100.0, 0.0 },
                            new double[] { 100.0, 0.0 },
                            new double[] { 100.0, 0.0 },
                            new double[] { 50.0, 0.0 },
                            new double[] { 0.0, -6.0 },
                            new double[] { 0.0, -4.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "LR2 HD",
                        Value = new double[6][]
                        {
                            new double[] { 0.0, 0.1 },
                            new double[] { 0.0, 0.1 },
                            new double[] { 0.0, 0.1 },
                            new double[] { 0.0, 0.05 },
                            new double[] { 0.0, -10.0 },
                            new double[] { 0.0, -6.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "BEATORAJA EZ",
                        Value = new double[6][]
                        {
                            new double[] { 100.0, 0.0 },
                            new double[] { 100.0, 0.0 },
                            new double[] { 100.0, 0.0 },
                            new double[] { 50.0, 0.0 },
                            new double[] { 0.0, -1.0 },
                            new double[] { 0.0, -2.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "BEATORAJA NM",
                        Value = new double[6][]
                        {
                            new double[] { 100.0, 0.0 },
                            new double[] { 100.0, 0.0 },
                            new double[] { 100.0, 0.0 },
                            new double[] { 50.0, 0.0 },
                            new double[] { 0.0, -1.0 },
                            new double[] { 0.0, -3.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "BEATORAJA HD",
                        Value = new double[6][]
                        {
                            new double[] { 100.0, 0.0 },
                            new double[] { 100.0, 0.0 },
                            new double[] { 100.0, 0.0 },
                            new double[] { 50.0, 0.0 },
                            new double[] { 0.0, -2.0 },
                            new double[] { 0.0, -6.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "BEATORAJA VHD",
                        Value = new double[6][]
                        {
                            new double[] { 0.0, 0.15 },
                            new double[] { 0.0, 0.135 },
                            new double[] { 0.0, 0.12 },
                            new double[] { 0, 0.03 },
                            new double[] { 0.0, -5.0 },
                            new double[] { 0.0, -10.0 }
                        },
                        IsDefault = true
                    },
                    new()
                    {
                        Name = "BEATORAJA UHD",
                        Value = new double[6][]
                        {
                            new double[] { 0.0, 0.15 },
                            new double[] { 0.0, 0.105 },
                            new double[] { 0.0, 0.06 },
                            new double[] { 0, 0.0 },
                            new double[] { 0.0, -10.0 },
                            new double[] { 0.0, -15.0 }
                        },
                        IsDefault = true
                    }
                };
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 7))
            {
                WantLevelName = null;
                WantLevelIDs = Array.Empty<string>();
                InputWantLevel = new bool[6];
                Array.Fill(InputWantLevel, true);
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 13))
            {
                AudioDataLength = 256;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 20))
            {
                DInputControllerVarietyValue = ControllerSystem.DInputControllerVariety.BMS;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 26))
            {
                SetSalt = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 32))
            {
                DInputBundlesV4 = new();
                DInputBundlesV4.SetInputs();
                DInputBundlesV4.SetStandardInputs();
                XInputBundlesV4 = new();
                XInputBundlesV4.SetInputs();
                XInputBundlesV4.Inputs[(int)Component.InputMode._4][1][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadLeft };
                XInputBundlesV4.Inputs[(int)Component.InputMode._4][2][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadUp };
                XInputBundlesV4.Inputs[(int)Component.InputMode._4][3][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.Y };
                XInputBundlesV4.Inputs[(int)Component.InputMode._4][4][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.B };
                XInputBundlesV4.Inputs[(int)Component.InputMode._5][1][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadLeft };
                XInputBundlesV4.Inputs[(int)Component.InputMode._5][2][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadUp };
                XInputBundlesV4.Inputs[(int)Component.InputMode._5][3][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadRight };
                XInputBundlesV4.Inputs[(int)Component.InputMode._5][3][1].Data = new() { Buttons = Vortice.XInput.GamepadButtons.X };
                XInputBundlesV4.Inputs[(int)Component.InputMode._5][4][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.Y };
                XInputBundlesV4.Inputs[(int)Component.InputMode._5][5][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.B };
                XInputBundlesV4.Inputs[(int)Component.InputMode._6][1][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadLeft };
                XInputBundlesV4.Inputs[(int)Component.InputMode._6][2][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadUp };
                XInputBundlesV4.Inputs[(int)Component.InputMode._6][3][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadRight };
                XInputBundlesV4.Inputs[(int)Component.InputMode._6][4][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.X };
                XInputBundlesV4.Inputs[(int)Component.InputMode._6][5][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.Y };
                XInputBundlesV4.Inputs[(int)Component.InputMode._6][6][0].Data = new() { Buttons = Vortice.XInput.GamepadButtons.B };
                XInputBundlesV4.SetStandardInputs();
                XInputBundlesV4.StandardInputs[InputStandardControllerViewModel.LowerEntry].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadUp };
                XInputBundlesV4.StandardInputs[InputStandardControllerViewModel.HigherEntry].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadDown };
                XInputBundlesV4.StandardInputs[InputStandardControllerViewModel.LowerNoteFile].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadLeft };
                XInputBundlesV4.StandardInputs[InputStandardControllerViewModel.HigherNoteFile].Data = new() { Buttons = Vortice.XInput.GamepadButtons.DPadRight };
                XInputBundlesV4.StandardInputs[InputStandardControllerViewModel.LevyNoteFile].Data = new() { Buttons = Vortice.XInput.GamepadButtons.Start };
                XInputBundlesV4.StandardInputs[InputStandardControllerViewModel.Wait].Data = new() { Buttons = Vortice.XInput.GamepadButtons.Back };
                WGIBundlesV3 = new();
                WGIBundlesV3.SetInputs();
                WGIBundlesV3.Inputs[(int)Component.InputMode._4][1][0].Data = new() { Buttons = GamepadButtons.DPadLeft };
                WGIBundlesV3.Inputs[(int)Component.InputMode._4][2][0].Data = new() { Buttons = GamepadButtons.DPadUp };
                WGIBundlesV3.Inputs[(int)Component.InputMode._4][3][0].Data = new() { Buttons = GamepadButtons.Y };
                WGIBundlesV3.Inputs[(int)Component.InputMode._4][4][0].Data = new() { Buttons = GamepadButtons.B };
                WGIBundlesV3.Inputs[(int)Component.InputMode._5][1][0].Data = new() { Buttons = GamepadButtons.DPadLeft };
                WGIBundlesV3.Inputs[(int)Component.InputMode._5][2][0].Data = new() { Buttons = GamepadButtons.DPadUp };
                WGIBundlesV3.Inputs[(int)Component.InputMode._5][3][0].Data = new() { Buttons = GamepadButtons.DPadRight };
                WGIBundlesV3.Inputs[(int)Component.InputMode._5][3][1].Data = new() { Buttons = GamepadButtons.X };
                WGIBundlesV3.Inputs[(int)Component.InputMode._5][4][0].Data = new() { Buttons = GamepadButtons.Y };
                WGIBundlesV3.Inputs[(int)Component.InputMode._5][5][0].Data = new() { Buttons = GamepadButtons.B };
                WGIBundlesV3.Inputs[(int)Component.InputMode._6][1][0].Data = new() { Buttons = GamepadButtons.DPadLeft };
                WGIBundlesV3.Inputs[(int)Component.InputMode._6][2][0].Data = new() { Buttons = GamepadButtons.DPadUp };
                WGIBundlesV3.Inputs[(int)Component.InputMode._6][3][0].Data = new() { Buttons = GamepadButtons.DPadRight };
                WGIBundlesV3.Inputs[(int)Component.InputMode._6][4][0].Data = new() { Buttons = GamepadButtons.X };
                WGIBundlesV3.Inputs[(int)Component.InputMode._6][5][0].Data = new() { Buttons = GamepadButtons.Y };
                WGIBundlesV3.Inputs[(int)Component.InputMode._6][6][0].Data = new() { Buttons = GamepadButtons.B };
                WGIBundlesV3.SetStandardInputs();
                WGIBundlesV3.StandardInputs[InputStandardControllerViewModel.LowerEntry].Data = new() { Buttons = GamepadButtons.DPadUp };
                WGIBundlesV3.StandardInputs[InputStandardControllerViewModel.HigherEntry].Data = new() { Buttons = GamepadButtons.DPadDown };
                WGIBundlesV3.StandardInputs[InputStandardControllerViewModel.LowerNoteFile].Data = new() { Buttons = GamepadButtons.DPadLeft };
                WGIBundlesV3.StandardInputs[InputStandardControllerViewModel.HigherNoteFile].Data = new() { Buttons = GamepadButtons.DPadRight };
                WGIBundlesV3.StandardInputs[InputStandardControllerViewModel.LevyNoteFile].Data = new() { Buttons = GamepadButtons.View };
                WGIBundlesV3.StandardInputs[InputStandardControllerViewModel.Wait].Data = new() { Buttons = GamepadButtons.Menu };
                MIDIBundlesV4 = new();
                MIDIBundlesV4.SetInputs();
                MIDIBundlesV4.SetStandardInputs();
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 47))
            {
                AudioConfigureValues = new();
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 49))
            {
                UICommentNote = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 63))
            {
                AutoNoteWait = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 64))
            {
                StopLastEqualAudio = true;
                LevelTargetMap = new();
                Language = Utility.GetLanguage(CultureInfo.CurrentUICulture.LCID);
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 66))
            {
                AutoGetQwilight = !QwilightComponent.IsVS;
                NVLLModeValue = NVLLMode.Not;
                NVLLFramerate = 0U;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 67))
            {
                DefaultInputBundlesV6 = new();
                DefaultInputBundlesV6.SetInputs();
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._4][1][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._4][2][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._4][3][0].Data = VirtualKey.J;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._4][4][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5][1][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5][2][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5][3][0].Data = VirtualKey.Space;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5][4][0].Data = VirtualKey.J;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5][5][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._6][1][0].Data = VirtualKey.S;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._6][2][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._6][3][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._6][4][0].Data = VirtualKey.J;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._6][5][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._6][6][0].Data = VirtualKey.L;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7][1][0].Data = VirtualKey.S;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7][2][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7][3][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7][4][0].Data = VirtualKey.Space;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7][5][0].Data = VirtualKey.J;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7][6][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7][7][0].Data = VirtualKey.L;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._8][1][0].Data = VirtualKey.A;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._8][2][0].Data = VirtualKey.S;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._8][3][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._8][4][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._8][5][0].Data = VirtualKey.J;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._8][6][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._8][7][0].Data = VirtualKey.L;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._8][8][0].Data = (VirtualKey)186;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][1][0].Data = VirtualKey.A;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][2][0].Data = VirtualKey.S;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][3][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][4][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][5][0].Data = VirtualKey.Space;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][6][0].Data = VirtualKey.J;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][7][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][8][0].Data = VirtualKey.L;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][9][0].Data = (VirtualKey)186;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][1][1].Data = VirtualKey.NumberPad7;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][2][1].Data = VirtualKey.NumberPad8;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][3][1].Data = VirtualKey.NumberPad9;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][4][1].Data = VirtualKey.NumberPad4;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][5][1].Data = VirtualKey.NumberPad5;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][6][1].Data = VirtualKey.NumberPad6;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][7][1].Data = VirtualKey.NumberPad1;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][8][1].Data = VirtualKey.NumberPad2;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._9][9][1].Data = VirtualKey.NumberPad3;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5_1][1][0].Data = VirtualKey.LeftShift;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5_1][1][1].Data = VirtualKey.RightShift;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5_1][2][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5_1][3][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5_1][4][0].Data = VirtualKey.Space;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5_1][5][0].Data = VirtualKey.J;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._5_1][6][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7_1][1][0].Data = VirtualKey.LeftShift;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7_1][1][1].Data = VirtualKey.RightShift;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7_1][2][0].Data = VirtualKey.S;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7_1][3][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7_1][4][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7_1][5][0].Data = VirtualKey.Space;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7_1][6][0].Data = VirtualKey.J;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7_1][7][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._7_1][8][0].Data = VirtualKey.L;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][1][0].Data = VirtualKey.LeftShift;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][2][0].Data = VirtualKey.Z;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][3][0].Data = VirtualKey.S;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][4][0].Data = VirtualKey.X;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][5][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][6][0].Data = VirtualKey.C;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][7][0].Data = (VirtualKey)188;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][8][0].Data = VirtualKey.L;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][9][0].Data = (VirtualKey)190;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][10][0].Data = (VirtualKey)186;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][11][0].Data = (VirtualKey)191;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10_2][12][0].Data = VirtualKey.RightShift;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][1][0].Data = VirtualKey.LeftShift;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][2][0].Data = VirtualKey.Z;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][3][0].Data = VirtualKey.S;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][4][0].Data = VirtualKey.X;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][5][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][6][0].Data = VirtualKey.C;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][7][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][8][0].Data = VirtualKey.V;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][9][0].Data = VirtualKey.M;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][10][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][11][0].Data = (VirtualKey)188;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][12][0].Data = VirtualKey.L;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][13][0].Data = (VirtualKey)190;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][14][0].Data = (VirtualKey)186;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][15][0].Data = (VirtualKey)191;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._14_2][16][0].Data = VirtualKey.RightShift;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][1][0].Data = VirtualKey.A;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][2][0].Data = VirtualKey.S;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][3][0].Data = VirtualKey.D;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][4][0].Data = VirtualKey.F;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][5][0].Data = VirtualKey.G;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][6][0].Data = VirtualKey.H;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][7][0].Data = VirtualKey.J;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][8][0].Data = VirtualKey.K;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][9][0].Data = VirtualKey.L;
                DefaultInputBundlesV6.Inputs[(int)Component.InputMode._10][10][0].Data = (VirtualKey)186;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 68))
            {
                WantLevelSystem = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 69))
            {
                DataCount3 = false;
                DetailedConfigure = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 72))
            {
                UIPipelineJudgmentMain = true;
                UIPipelineMainJudgmentMeter = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 73))
            {
                AutoJudgmentMeterMillis = true;
                AutoJudgmentMeterMillisItemValue = new()
                {
                    Judged = Component.Judged.Highest
                };
                JudgmentMeterMillis = 0.0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 75))
            {
                AllowTwilightComment = !QwilightComponent.IsVS;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 78))
            {
                IsQwilightFill = false;
                DefaultControllerInputAPI = DefaultControllerSystem.InputAPI.DefaultInput;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 96))
            {
                InputWantNoteVariety = new bool[7];
                Array.Fill(InputWantNoteVariety, true);
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 97))
            {
                LastDefaultEntryItem = DefaultEntryItem.Total;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 101))
            {
                LastEntryItemPositions = new();
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 103))
            {
                PassedTutorialIDs = new();
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 104))
            {
                Tube = false;
                FitModeValue = new()
                {
                    Mode = FitMode.Title
                };
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 105))
            {
                AutoLowestLongNoteModify = false;
                HighestAutoLongNoteModify = false;
                AutoLowestLongNoteModifyValue = 16.0;
                AutoHighestLongNoteModifyValue = 16.0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 107))
            {
                LastWASAPIAudioValueID = 0;
                LastASIOAudioValueID = 0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 109))
            {
                ModeComponentBundles = new ModeComponentBundle[]
                {
                    new(),
                    new(),
                    new()
                    {
                        Name = "LR2 EZ",
                        Value = new()
                        {
                            JudgmentModeValue = ModeComponent.JudgmentMode.Favor,
                            HitPointsModeValue = ModeComponent.HitPointsMode.Favor,
                            FavorJudgments = new double[6][]
                            {
                                new double[] { -21.0, 21.0 },
                                new double[] { -40.5, 40.5 },
                                new double[] { -60.0, 60.0 },
                                new double[] { -120.0, 120.0 },
                                new double[] { -200.0, 200.0 },
                                new double[] { -1000.0, 200.0 }
                            },
                            FavorHitPoints = new double[6][]
                            {
                                new double[] { 120.0, 0.0 },
                                new double[] { 120.0, 0.0 },
                                new double[] { 120.0, 0.0 },
                                new double[] { 60.0, 0.0 },
                                new double[] { 0.0, -4.8 },
                                new double[] { 0.0, -3.2 }
                            }
                        }
                    },
                    new()
                    {
                        Name = "LR2 NM",
                        Value = new()
                        {
                            JudgmentModeValue = ModeComponent.JudgmentMode.Favor,
                            HitPointsModeValue = ModeComponent.HitPointsMode.Favor,
                            FavorJudgments = new double[6][]
                            {
                                new double[] { -18.0, 18.0 },
                                new double[] { -29.0, 29.0 },
                                new double[] { -40.0, 40.0 },
                                new double[] { -100.0, 100.0 },
                                new double[] { -200.0, 200.0 },
                                new double[] { -1000.0, 200.0 }
                            },
                            FavorHitPoints = new double[6][]
                            {
                                new double[] { 100.0, 0.0 },
                                new double[] { 100.0, 0.0 },
                                new double[] { 100.0, 0.0 },
                                new double[] { 50.0, 0.0 },
                                new double[] { 0.0, -6.0 },
                                new double[] { 0.0, -4.0 }
                            }
                        }
                    },
                    new()
                    {
                        Name = "LR2 HD",
                        Value = new()
                        {
                            JudgmentModeValue = ModeComponent.JudgmentMode.Favor,
                            HitPointsModeValue = ModeComponent.HitPointsMode.Favor,
                            FavorJudgments = new double[6][]
                            {
                                new double[] { -15.0, 15.0 },
                                new double[] { -22.5, 22.5 },
                                new double[] { -30.0, 30.0 },
                                new double[] { -60.0, 60.0 },
                                new double[] { -200.0, 200.0 },
                                new double[] { -1000.0, 200.0 }
                            },
                            FavorHitPoints = new double[6][]
                            {
                                new double[] { 0.0, 0.1 },
                                new double[] { 0.0, 0.1 },
                                new double[] { 0.0, 0.1 },
                                new double[] { 0.0, 0.05 },
                                new double[] { 0.0, -10.0 },
                                new double[] { 0.0, -6.0 }
                            }
                        }
                    },
                    new()
                    {
                        Name = "BEATORAJA EZ",
                        Value = new()
                        {
                            JudgmentModeValue = ModeComponent.JudgmentMode.Favor,
                            HitPointsModeValue = ModeComponent.HitPointsMode.Favor,
                            FavorJudgments = new double[6][]
                            {
                                new double[] { -20.0, 20.0 },
                                new double[] { -40.0, 40.0 },
                                new double[] { -60.0, 60.0 },
                                new double[] { -150.0, 150.0 },
                                new double[] { -220.0, 280.0 },
                                new double[] { -500.0, 280.0 }
                            },
                            FavorHitPoints =  new double[6][]
                            {
                                new double[] { 100.0, 0.0 },
                                new double[] { 100.0, 0.0 },
                                new double[] { 100.0, 0.0 },
                                new double[] { 50.0, 0.0 },
                                new double[] { 0.0, -1.0 },
                                new double[] { 0.0, -2.0 }
                            }
                        }
                    },
                    new()
                    {
                        Name = "BEATORAJA NM",
                        Value = new()
                        {
                            JudgmentModeValue = ModeComponent.JudgmentMode.Favor,
                            HitPointsModeValue = ModeComponent.HitPointsMode.Favor,
                            FavorJudgments =  new double[6][]
                            {
                                new double[] { -15.0, 15.0 },
                                new double[] { -30.0, 30.0 },
                                new double[] { -45.0, 45.0 },
                                new double[] { -112.5, 112.5 },
                                new double[] { -275.0, 350.0 },
                                new double[] { -500.0, 350.0 }
                            },
                            FavorHitPoints =  new double[6][]
                            {
                                new double[] { 100.0, 0.0 },
                                new double[] { 100.0, 0.0 },
                                new double[] { 100.0, 0.0 },
                                new double[] { 50.0, 0.0 },
                                new double[] { 0.0, -1.0 },
                                new double[] { 0.0, -3.0 }
                            }
                        }
                    },
                    new()
                    {
                        Name = "BEATORAJA HD",
                        Value = new()
                        {
                            JudgmentModeValue = ModeComponent.JudgmentMode.Favor,
                            HitPointsModeValue = ModeComponent.HitPointsMode.Favor,
                            FavorJudgments = new double[6][]
                            {
                                new double[] { -10.0, 10.0 },
                                new double[] { -20.0, 20.0 },
                                new double[] { -30.0, 30.0 },
                                new double[] { -75.0, 75.0 },
                                new double[] { -330.0, 420.0 },
                                new double[] { -500.0, 420.0 }
                            },
                            FavorHitPoints = new double[6][]
                            {
                                new double[] { 100.0, 0.0 },
                                new double[] { 100.0, 0.0 },
                                new double[] { 100.0, 0.0 },
                                new double[] { 50.0, 0.0 },
                                new double[] { 0.0, -2.0 },
                                new double[] { 0.0, -6.0 }
                            }
                        }
                    },
                };
                FastInputMillis = 0.0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 112))
            {
                HandleMeter = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 115))
            {
                SetHwMode = false;
                LS = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 116))
            {
                BaseUIConfigureValues = new()
                {
                    { BaseUIItemValue.Title, new BaseUIConfigure() }
                };
                UIConfigureValuesV2 = new()
                {
                    { UIItemValue.Title, new UIConfigure() }
                };
                DInputIIDXSensitivity = 1000.0 / 7;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 117))
            {
                WantBannedValue = WantBanned.NotBanned;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 119))
            {
                var rawHwMode = new DEVMODEW();
                PInvoke.EnumDisplaySettings(null, ENUM_DISPLAY_SETTINGS_MODE.ENUM_CURRENT_SETTINGS, ref rawHwMode);
                HwModeV2Value = new(rawHwMode.dmPelsWidth, rawHwMode.dmPelsHeight, rawHwMode.dmDisplayFrequency);
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 120))
            {
                TVAssistConfigure = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 14, 126))
            {
                Averager = false;
                UbuntuNetItemTarget = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 15, 3))
            {
                ModeComponentValue = new();
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 15, 5))
            {
                DefaultAudioVarietyValue = DefaultAudioVariety.UI;
                DefaultDrawingFilePath = string.Empty;
                VeilDrawingFilePath = string.Empty;
                VeilDrawingHeight = 0.0;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 15, 6))
            {
                DefaultInputBundlesV6.SetStandardInputs();
                DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.LowerMultiplier].Data = VirtualKey.F3;
                DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.HigherMultiplier].Data = VirtualKey.F4;
                DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.ModifyAutoMode].Data = VirtualKey.F1;
                DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.HandleUndo].Data = VirtualKey.Tab;
                DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.MediaMode].Data = VirtualKey.F2;
                DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.LowerAudioMultiplier].Data = (VirtualKey)189;
                DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.HigherAudioMultiplier].Data = (VirtualKey)187;
                DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.PostItem0].Data = VirtualKey.V;
                DefaultInputBundlesV6.StandardInputs[InputStandardViewModel.PostItem1].Data = VirtualKey.N;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 0))
            {
                AutoNVLLFramerate = false;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 3))
            {
                MainAreaFaint = 1.0;
                MediaInputFaint = 1.0;
                UIPipelineJudgmentInputVisualizer = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 5))
            {
                InitColors(2);
                BanalFailedMedia = false;
                AlwaysBanalFailedMedia = false;
                BanalFailedMediaFilePath = string.Empty;
                BanalAudioWait = 0.0;
                BanalAudio = false;
                BanalMedia = false;
                AlwaysBanalMedia = false;
                BanalMediaFilePath = string.Empty;
                BanalAudioFilePath = Path.Combine(QwilightComponent.UIEntryPath, "Default.wav");
                BanalMediaWait = 0.0;
                AutoGetDefaultNote = true;
                AutoGetDefaultUI = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 6))
            {
                LoadedMedia = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 8))
            {
                LastInputWants = new();
                InputWantInputMode = new bool[17];
                Array.Fill(InputWantInputMode, true);
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 9))
            {
                FontFamilyValues = new FontFamily[4];
                Array.Fill(FontFamilyValues, QwilightComponent.GetBuiltInData<FontFamily>("DefaultFontFamily"));
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 10))
            {
                GASLevel = 2;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 12))
            {
                DefaultAudioFilePathItems = Array.Empty<DefaultAudioFilePathItem>();
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 13))
            {
                FlowValues = true;
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 14))
            {
                InputWantHandled = new bool[8];
                Array.Fill(InputWantHandled, true);
            }
            if (isInit || Utility.IsLowerDate(Date, 1, 16, 16))
            {
                LazyGCV2 = 0L;
            }
            if (!UIConfigureValuesV2.ContainsKey(UIItemValue.Title))
            {
                UIConfigureValuesV2[UIItemValue.Title] = new();
            }
            if (!BaseUIConfigureValues.ContainsKey(BaseUIItemValue.Title))
            {
                BaseUIConfigureValues[BaseUIItemValue.Title] = new();
            }
            SetFontFamily();
            Date = QwilightComponent.Date;
            if (isInit)
            {
                NotifyModel();
            }
        }

        public void InitEqualizers(int level)
        {
            if ((level & 1) == 1)
            {
                Equalizer0 = 0F;
                Equalizer1 = 0F;
                Equalizer2 = 0F;
                Equalizer3 = 0F;
                Equalizer4 = 0F;
            }
            if ((level & 2) == 2)
            {
                EqualizerHz0 = 90F;
                EqualizerHz1 = 500F;
                EqualizerHz2 = 3000F;
                EqualizerHz3 = 5000F;
                EqualizerHz4 = 8000F;
            }
        }

        public double GetAudioVisualizerModifier(double audioVisualizerHeight, double value) => AudioVisualizerModeValue switch
        {
            AudioVisualizerMode.AudioVisualizerMode0 => 0.0,
            AudioVisualizerMode.AudioVisualizerMode1 => (audioVisualizerHeight - value) / 2,
            AudioVisualizerMode.AudioVisualizerMode2 => audioVisualizerHeight - value,
            _ => default
        };

        public override void NotifyModel()
        {
            base.NotifyModel();
            UIConfigureValue.NotifyModel();
            BaseUIConfigureValue.NotifyModel();
        }
    }
}