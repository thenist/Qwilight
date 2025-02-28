﻿using CommunityToolkit.Mvvm.Input;
using CommunityToolkit.Mvvm.Messaging;
using FMOD;
using Ionic.Zip;
using Qwilight.MSG;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using System.IO;
using System.Windows.Threading;
using Windows.ApplicationModel.DataTransfer;
using Windows.Win32;
using Windows.Win32.Graphics.Gdi;
using Windows.Win32.UI.WindowsAndMessaging;
using YamlDotNet.RepresentationModel;
using ImageSource = System.Windows.Media.ImageSource;

namespace Qwilight.ViewModel
{
    public sealed partial class ConfigureViewModel : BaseViewModel
    {
        readonly UIItem[] _targets = new UIItem[2];
        readonly bool[] _audioVisualizerModeInputs = new bool[3];
        readonly bool[] _inputMapInputs = new bool[4];
        readonly bool[] _dInputControllerVarietyInputs = new bool[2];
        readonly bool[] _audioVarietyInputs = new bool[2];
        readonly bool[] _valueGPUModeInputs = new bool[2];
        readonly bool[] _loopUnitInputs = new bool[4];
        readonly bool[] _inputAudioVarietyInputs = new bool[2];
        readonly DispatcherTimer _detailedHandler;
        DispatcherTimer _fadingComputingHandler;
        GPUConfigure.GPUMode _defaultGPUMode;
        bool _defaultWMPointer;
        uint _defaultAudioDataLength;
        int _detailedAudioInputValue;
        string _detailedAudioItemCount;
        string _detailedAudioHandlerItemCount;
        string _detailedHandlingAudioCount;
        float _detailedAudioUnitStatus;
        string _detailedMediaItemCount;
        string _detailedMediaHandlerItemCount;
        string _detailedNoteDrawingCount;
        string _detailedDrawingItemCount;
        string _mainControllerPowerStatus;
        long _detailedSet;
        int _tabPosition;
        int _tabPositioinComputing;
        int _tabPositionUI;
        HunterVariety _defaultHunterVariety;
        bool _defaultNetCommentFollow;

        public override double TargetLength => 0.9;

        public UIItem BaseUIItemValue
        {
            get => _targets[0];

            set => SetProperty(ref _targets[0], value, nameof(BaseUIItemValue));
        }

        public UIItem UIItemValue
        {
            get => _targets[1];

            set => SetProperty(ref _targets[1], value, nameof(UIItemValue));
        }

        public bool AudioVisualizerMode0
        {
            get => _audioVisualizerModeInputs[0];

            set
            {
                if (SetProperty(ref _audioVisualizerModeInputs[0], value, nameof(AudioVisualizerMode0)) && value)
                {
                    Configure.Instance.AudioVisualizerModeValue = Configure.AudioVisualizerMode.AudioVisualizerMode0;
                }
            }
        }

        public bool AudioVisualizerMode1
        {
            get => _audioVisualizerModeInputs[1];

            set
            {
                if (SetProperty(ref _audioVisualizerModeInputs[1], value, nameof(AudioVisualizerMode1)) && value)
                {
                    Configure.Instance.AudioVisualizerModeValue = Configure.AudioVisualizerMode.AudioVisualizerMode1;
                }
            }
        }

        public bool AudioVisualizerMode2
        {
            get => _audioVisualizerModeInputs[2];

            set
            {
                if (SetProperty(ref _audioVisualizerModeInputs[2], value, nameof(AudioVisualizerMode2)) && value)
                {
                    Configure.Instance.AudioVisualizerModeValue = Configure.AudioVisualizerMode.AudioVisualizerMode2;
                }
            }
        }

        public bool InputMapping0
        {
            get => _inputMapInputs[0];

            set
            {
                if (SetProperty(ref _inputMapInputs[0], value, nameof(InputMapping0)) && value)
                {
                    Configure.Instance.InputMappingValue = Component.InputMapping.Mapping0;
                }
            }
        }

        public bool InputMapping1
        {
            get => _inputMapInputs[1];

            set
            {
                if (SetProperty(ref _inputMapInputs[1], value, nameof(InputMapping1)) && value)
                {
                    Configure.Instance.InputMappingValue = Component.InputMapping.Mapping1;
                }
            }
        }

        public bool InputMapping2
        {
            get => _inputMapInputs[2];

            set
            {
                if (SetProperty(ref _inputMapInputs[2], value, nameof(InputMapping2)) && value)
                {
                    Configure.Instance.InputMappingValue = Component.InputMapping.Mapping2;
                }
            }
        }

        public bool InputMapping3
        {
            get => _inputMapInputs[3];

            set
            {
                if (SetProperty(ref _inputMapInputs[3], value, nameof(InputMapping3)) && value)
                {
                    Configure.Instance.InputMappingValue = Component.InputMapping.Mapping3;
                }
            }
        }

        public int TabPosition
        {
            get => _tabPosition;

            set => SetProperty(ref _tabPosition, value, nameof(TabPosition));
        }

        public int TabPositionComputing
        {
            get => _tabPositioinComputing;

            set => SetProperty(ref _tabPositioinComputing, value, nameof(TabPositionComputing));
        }

        public int TabPositionUI
        {
            get => _tabPositionUI;

            set
            {
                if (SetProperty(ref _tabPositionUI, value, nameof(TabPositionUI)))
                {
                    NotifyCanSaveAsBundle();
                    OnSetTabPositionUI();
                }
            }
        }

        void OnSetTabPositionUI()
        {
            switch (TabPositionUI)
            {
                case 0:
                    UIHandler.Instance.HandleParallel(() =>
                    {
                        BaseUI.Instance.LoadUIFiles();
                        BaseUIItemValue = BaseUI.Instance.UIItems.Contains(Configure.Instance.BaseUIItemValue) ? Configure.Instance.BaseUIItemValue : null;
                    });
                    break;
                case 1:
                    UIHandler.Instance.HandleParallel(() =>
                    {
                        UI.Instance.LoadUIFiles();
                        UIItemValue = UI.Instance.UIItems.Contains(Configure.Instance.UIItemValue) ? Configure.Instance.UIItemValue : null;
                    });
                    break;
            }
        }

        public string[] Titles { get; } = new string[2];

        public string[] Comments { get; } = new string[2];

        public ImageSource[] Drawings { get; } = new ImageSource[2];

        public void OnPointBaseUI() => SetUI(0);

        public void OnPointUI() => SetUI(1);

        public void OnBaseUIConfigure()
        {
            if (!BaseUI.Instance.LoadedConfigures.SequenceEqual(Configure.Instance.BaseUIConfigureValue.UIConfigures))
            {
                Utility.SetBaseUIItem(Configure.Instance.BaseUIItemValue, Configure.Instance.BaseUIItemValue);
            }
        }

        public void OnUIConfigure()
        {
            if (!UI.Instance.LoadedConfigures.SequenceEqual(Configure.Instance.UIConfigureValue.UIConfiguresV2))
            {
                Utility.SetUIItem(Configure.Instance.UIItemValue, Configure.Instance.UIItemValue);
            }
        }

        public void OnWindowArea() => StrongReferenceMessenger.Default.Send<SetWindowArea>();

        public async Task OnDefaultDrawing()
        {
            if (File.Exists(Configure.Instance.DefaultDrawingFilePath))
            {
                Configure.Instance.DefaultDrawingFilePath = string.Empty;
                DrawingSystem.Instance.LoadDefaultDrawing();
            }
            else
            {
                var filePath = await StrongReferenceMessenger.Default.Send(new ViewFileWindow
                {
                    Filters = QwilightComponent.DrawingFileFormats
                });
                if (!string.IsNullOrEmpty(filePath))
                {
                    Configure.Instance.DefaultDrawingFilePath = filePath;
                    DrawingSystem.Instance.LoadDefaultDrawing();
                }
            }
        }
        public void OnSetBaseUI()
        {
            if (!ViewModels.Instance.MainValue.IsUILoading && BaseUIItemValue != null && BaseUIItemValue != Configure.Instance.BaseUIItemValue)
            {
                Utility.SetBaseUIItem(null, BaseUIItemValue);
            }
        }

        public void OnSetUI()
        {
            if (!ViewModels.Instance.MainValue.IsUILoading && UIItemValue != null && UIItemValue != Configure.Instance.UIItemValue)
            {
                Utility.SetUIItem(null, UIItemValue);
            }
        }

        public async Task OnVeilDrawing()
        {
            if (File.Exists(Configure.Instance.VeilDrawingFilePath))
            {
                Configure.Instance.VeilDrawingFilePath = string.Empty;
                DrawingSystem.Instance.LoadVeilDrawing();
            }
            else
            {
                var filePath = await StrongReferenceMessenger.Default.Send(new ViewFileWindow
                {
                    Filters = QwilightComponent.DrawingFileFormats
                });
                if (!string.IsNullOrEmpty(filePath))
                {
                    Configure.Instance.VeilDrawingFilePath = filePath;
                    DrawingSystem.Instance.LoadVeilDrawing();
                }
            }
        }

        public void OnComputingPointed()
        {
            if (ViewModels.Instance.MainValue.IsNoteFileMode)
            {
                Configure.Instance.NotifyTutorial(Configure.TutorialID.EnterAutoComputeConfigure);
            }
            else
            {
                _fadingComputingHandler?.Stop();
                _fadingComputingHandler = new(QwilightComponent.StandardFrametime, DispatcherPriority.Send, (sender, e) =>
                {
                    Faint = Math.Max(0.125, Faint + Utility.GetMove(0.125, Faint, 1000.0 / 60));

                    if (Faint == 0.125)
                    {
                        (sender as DispatcherTimer).Stop();
                    }
                }, UIHandler.Instance.Handler);
            }
        }

        public void OnComputingNotPointed()
        {
            _fadingComputingHandler?.Stop();
            _fadingComputingHandler = new(QwilightComponent.StandardFrametime, DispatcherPriority.Send, (sender, e) =>
            {
                Faint = Math.Min(Faint + Utility.GetMove(1.0, Faint, 1000.0 / 60), 1.0);

                if (Faint == 1.0)
                {
                    (sender as DispatcherTimer).Stop();
                }
            }, UIHandler.Instance.Handler);
        }

        public void OnComputingModified()
        {
            ViewModels.Instance.MainValue.Computer?.SetUIMap();
        }

        [RelayCommand]
        void OnEnterAutoComputeUIConfigure()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            var autoComputer = mainViewModel.AutoComputer;
            if (autoComputer?.IsHandling == true)
            {
                autoComputer.Unpause();
                mainViewModel.EnterAutoComputingMode();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.EnterAutoComputeUIConfigure);
            }
            else if (Configure.Instance.AutoCompute)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAutoComputeFault);
            }
            else
            {
                TabPosition = 4;
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAutoComputeConfigureFault);
            }
        }

        [RelayCommand]
        void OnEnterAutoComputeMediaInputConfigure()
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            if (mainViewModel.GetHandlingComputer()?.IsHandling == true)
            {
                Close();
                Configure.Instance.InitMediaInputArea();
                DrawingSystem.Instance.InitMediaInputArea();
                mainViewModel.EnterAutoComputingMode();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.EnterAutoComputeMediaInputConfigure);
            }
            else if (Configure.Instance.AutoCompute)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAutoComputeFault);
            }
            else
            {
                TabPosition = 4;
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.NotAutoComputeConfigureFault);
            }
        }

        [RelayCommand]
        static void OnCopySalt()
        {
            var dataBundle = new DataPackage();
            dataBundle.SetText(ViewModels.Instance.MainValue.ModeComponentValue.Salt.ToString());
            Clipboard.SetContent(dataBundle);
        }

        [RelayCommand]
        static void OnSetAudioMultiplier(double e) => ViewModels.Instance.MainValue.ModeComponentValue.AudioMultiplier = e;

        [RelayCommand]
        static void OnIsMediaFill() => Configure.Instance.IsMediaFill = !Configure.Instance.IsMediaFill;

        [RelayCommand]
        static void OnViewQwilightEntry() => Utility.OpenAs(QwilightComponent.QwilightEntryPath);

        [RelayCommand]
        static void OnViewUIEntry() => Utility.OpenAs(QwilightComponent.UIEntryPath);

        [RelayCommand]
        static void OnOpenAs(string e) => Utility.OpenAs(e);

        [RelayCommand]
        static void OnFlowVeilDrawing() => Configure.Instance.FlowVeilDrawing = !Configure.Instance.FlowVeilDrawing;

        [RelayCommand]
        static void OnLowHitPointsFaintUI() => Configure.Instance.LowHitPointsFaintUI = !Configure.Instance.LowHitPointsFaintUI;

        [RelayCommand]
        static void OnAlwaysNotP2Position() => Configure.Instance.AlwaysNotP2Position = !Configure.Instance.AlwaysNotP2Position;

        [RelayCommand]
        static void OnNetCommentFollow() => Configure.Instance.NetCommentFollow = !Configure.Instance.NetCommentFollow;

        [RelayCommand]
        static void OnNetItemTarget() => Configure.Instance.UbuntuNetItemTarget = !Configure.Instance.UbuntuNetItemTarget;

        [RelayCommand]
        static void OnHandleMeter() => Configure.Instance.HandleMeter = !Configure.Instance.HandleMeter;

        [RelayCommand]
        static void OnFlowValues() => Configure.Instance.FlowValues = !Configure.Instance.FlowValues;

        [RelayCommand]
        static void OnPutCopyNotes() => ViewModels.Instance.MainValue.ModeComponentValue.PutCopyNotesValueV2 = (ModeComponent.PutCopyNotes)(((int)ViewModels.Instance.MainValue.ModeComponentValue.PutCopyNotesValueV2 + 1) % 4);

        [RelayCommand]
        static void OnInitModeComponent() => ViewModels.Instance.MainValue.InitModeComponent();

        [RelayCommand]
        static void OnModifyModeComponent(int? e)
        {
            if (e.HasValue)
            {
                var toModifyModeComponentViewModel = ViewModels.Instance.ModifyModeComponentValue;
                toModifyModeComponentViewModel.ModeComponentVariety = e.Value;
                toModifyModeComponentViewModel.Open();
            }
        }

        [RelayCommand]
        static void OnSaveModeComponent(int? e)
        {
            if (e.HasValue)
            {
                var modeComponentBundle = Configure.Instance.ModeComponentBundles[e.Value];
                var inputTextViewModel = ViewModels.Instance.InputTextValue;
                inputTextViewModel.Text = LanguageSystem.Instance.SaveModeComponentContents;
                inputTextViewModel.Input = modeComponentBundle.Name;
                inputTextViewModel.HandleOK = new Action<string>(text =>
                {
                    modeComponentBundle.Name = text;
                    modeComponentBundle.Value.CopyAs(ViewModels.Instance.MainValue.ModeComponentValue);
                });
                inputTextViewModel.Open();
            }
        }

        [RelayCommand]
        static void OnStopLastEqualAudio() => Configure.Instance.StopLastEqualAudio = !Configure.Instance.StopLastEqualAudio;

        [RelayCommand]
        static async Task OnGetMIDI() => await MIDISystem.Instance.GetMIDIs().ConfigureAwait(false);

        [RelayCommand]
        static void OnFontFamily(int? e)
        {
            if (e.HasValue)
            {
                ViewModels.Instance.FontFamilyValue.FontPosition = e.Value;
                ViewModels.Instance.FontFamilyValue.Open();
            }
        }

        [RelayCommand]
        static void OnLoopWaveIn() => AudioInputSystem.Instance.LoopWaveIn = !AudioInputSystem.Instance.LoopWaveIn;

        [RelayCommand]
        static void OnIsXwindow() => Configure.Instance.IsXwindow = !Configure.Instance.IsXwindow;

        [RelayCommand]
        static void OnWMPointer() => Configure.Instance.WMPointer = !Configure.Instance.WMPointer;

        [RelayCommand]
        static void OnDefaultControllerInputAPI()
        {
            Configure.Instance.DefaultControllerInputAPI = (DefaultControllerSystem.InputAPI)((int)(Configure.Instance.DefaultControllerInputAPI + 1) % 2);
            DefaultControllerSystem.Instance.HandleSystem(StrongReferenceMessenger.Default.Send<GetWindowHandle>().Response);
        }

        [RelayCommand]
        static void OnControllerInputAPI() => Configure.Instance.ControllerInputAPI = (ControllerSystem.InputAPI)((int)(Configure.Instance.ControllerInputAPI + 1) % 3);

        [RelayCommand]
        static void OnLostPointAudio() => Configure.Instance.LostPointAudio = !Configure.Instance.LostPointAudio;

        [RelayCommand]
        static void OnInitCompiled()
        {
            if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.InitCompiledNotify,
                Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
            }) == MESSAGEBOX_RESULT.IDYES)
            {
                FastDB.Instance.Clear();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.InitCompiledOK);
            }
        }

        [RelayCommand]
        static void OnInitFavoriteEntry()
        {
            if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.InitFavoriteEntryNotify,
                Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
            }) == MESSAGEBOX_RESULT.IDYES)
            {
                ViewModels.Instance.MainValue.WipeFavoriteEntry();
                Configure.Instance.DefaultEntryItems.RemoveAll(defaultEntryItem => defaultEntryItem.DefaultEntryVarietyValue == DefaultEntryItem.DefaultEntryVariety.Favorite);
                DB.Instance.WipeFavoriteEntry();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.InitFavoriteEntryOK);
            }
        }

        [RelayCommand]
        static void OnInitWait()
        {
            if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.InitWaitNotify,
                Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
            }) == MESSAGEBOX_RESULT.IDYES)
            {
                Configure.Instance.AudioWait = 0.0;
                Configure.Instance.BanalAudioWait = 0.0;
                Configure.Instance.MediaWait = 0.0;
                Configure.Instance.BanalMediaWait = 0.0;
                Configure.Instance.NotifyModel();
                DB.Instance.InitWait();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.InitWaitOK);
            }
        }

        [RelayCommand]
        static void OnInitColor()
        {
            if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.InitColorNotify,
                Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
            }) == MESSAGEBOX_RESULT.IDYES)
            {
                Configure.Instance.InitColors(int.MaxValue);
                AvatarTitleSystem.Instance.WipeAvatarTitles();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.InitColorOK);
            }
        }

        [RelayCommand]
        static void OnInitMedia()
        {
            if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.InitMediaNotify,
                Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
            }) == MESSAGEBOX_RESULT.IDYES)
            {
                Configure.Instance.Media = true;
                DB.Instance.InitMedia();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.InitMediaOK);
            }
        }

        [RelayCommand]
        static void OnInitComment()
        {
            if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.InitCommentNotify,
                Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
            }) == MESSAGEBOX_RESULT.IDYES)
            {
                ViewModels.Instance.MainValue.DefaultCommentItemCollection.Clear();
                foreach (var commentFilePath in Utility.GetFiles(QwilightComponent.CommentEntryPath))
                {
                    Utility.WipeFile(commentFilePath);
                }
                DB.Instance.WipeComment();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.InitCommentOK);
            }
        }

        [RelayCommand]
        void OnInitTotal()
        {
            if (StrongReferenceMessenger.Default.Send(new ViewAllowWindow
            {
                Text = LanguageSystem.Instance.InitTotalNotify,
                Data = MESSAGEBOX_STYLE.MB_YESNO | MESSAGEBOX_STYLE.MB_ICONQUESTION | MESSAGEBOX_STYLE.MB_DEFBUTTON1
            }) == MESSAGEBOX_RESULT.IDYES)
            {
                Configure.Instance.Validate(true);
                ViewModels.Instance.MainValue.SetDefaultEntryItems();
                Init();
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.InitTotalOK);
            }
        }

        [RelayCommand]
        static void OnMode(int? e)
        {
            if (e.HasValue)
            {
                var inputVIewModel = ViewModels.Instance.InputValue;
                inputVIewModel.ControllerModeValue = InputViewModel.ControllerMode.DefaultInput;
                inputVIewModel.InputMode = (Component.InputMode)e.Value;
                inputVIewModel.Open();
            }
            else
            {
                ViewModels.Instance.InputStandardValue.Open();
            }
        }

        [RelayCommand]
        static void OnControllerMode(int? e)
        {
            if (e.HasValue)
            {
                var inputVIewModel = ViewModels.Instance.InputValue;
                switch (Configure.Instance.ControllerInputAPI)
                {
                    case ControllerSystem.InputAPI.DInput:
                        inputVIewModel.ControllerModeValue = InputViewModel.ControllerMode.DInput;
                        break;
                    case ControllerSystem.InputAPI.XInput:
                        inputVIewModel.ControllerModeValue = InputViewModel.ControllerMode.XInput;
                        break;
                    case ControllerSystem.InputAPI.WGI:
                        inputVIewModel.ControllerModeValue = InputViewModel.ControllerMode.WGI;
                        break;
                }
                inputVIewModel.InputMode = (Component.InputMode)e.Value;
                inputVIewModel.Open();
            }
            else
            {
                var inputStandardControllerViewModel = ViewModels.Instance.InputStandardControllerValue;
                switch (Configure.Instance.ControllerInputAPI)
                {
                    case ControllerSystem.InputAPI.DInput:
                        inputStandardControllerViewModel.ControllerModeValue = InputStandardControllerViewModel.ControllerMode.DInput;
                        break;
                    case ControllerSystem.InputAPI.XInput:
                        inputStandardControllerViewModel.ControllerModeValue = InputStandardControllerViewModel.ControllerMode.XInput;
                        break;
                    case ControllerSystem.InputAPI.WGI:
                        inputStandardControllerViewModel.ControllerModeValue = InputStandardControllerViewModel.ControllerMode.WGI;
                        break;
                }
                inputStandardControllerViewModel.Open();
            }
        }

        [RelayCommand]
        static void OnMIDIMode(int? e)
        {
            if (e.HasValue)
            {
                var inputVIewModel = ViewModels.Instance.InputValue;
                inputVIewModel.ControllerModeValue = InputViewModel.ControllerMode.MIDI;
                inputVIewModel.InputMode = (Component.InputMode)e.Value;
                inputVIewModel.Open();
            }
            else
            {
                var inputStandardControllerViewModel = ViewModels.Instance.InputStandardControllerValue;
                inputStandardControllerViewModel.ControllerModeValue = InputStandardControllerViewModel.ControllerMode.MIDI;
                inputStandardControllerViewModel.Open();
            }
        }

        [RelayCommand]
        static void OnAudioInputConfigure()
        {
            if (Configure.Instance.AudioInput)
            {
                AudioInputSystem.Instance.GetWaveInValues();
                AudioInputSystem.Instance.GetWaveValues();
            }
            else
            {
                AudioInputSystem.Instance.CloseWaveIn();
                AudioInputSystem.Instance.CloseWave();
            }
        }

        [RelayCommand]
        static void OnWaveIn()
        {
            if (Configure.Instance.WaveIn)
            {
                AudioInputSystem.Instance.GetWaveInValues();
            }
            else
            {
                AudioInputSystem.Instance.CloseWaveIn();
            }
        }

        [RelayCommand]
        static void OnWave()
        {
            if (Configure.Instance.Wave)
            {
                AudioInputSystem.Instance.GetWaveValues();
            }
            else
            {
                AudioInputSystem.Instance.CloseWave();
            }
        }

        [RelayCommand]
        static void OnGetWaveInValues() => AudioInputSystem.Instance.GetWaveInValues();

        [RelayCommand]
        static void OnGetWaveValues() => AudioInputSystem.Instance.GetWaveValues();

        [RelayCommand]
        static void OnMedia() => Configure.Instance.Media = !Configure.Instance.Media;

        [RelayCommand]
        static void OnHandleInputAudio() => Configure.Instance.HandleInputAudio = !Configure.Instance.HandleInputAudio;

        [RelayCommand]
        static void OnLevelingInputAudio() => Configure.Instance.LevelingInputAudio = !Configure.Instance.LevelingInputAudio;

        [RelayCommand]
        static void OnBanalAudio() => Configure.Instance.BanalAudio = !Configure.Instance.BanalAudio;

        [RelayCommand]
        static void OnBanalMedia() => Configure.Instance.BanalMedia = !Configure.Instance.BanalMedia;

        [RelayCommand]
        static void OnBanalFailedMedia() => Configure.Instance.BanalFailedMedia = !Configure.Instance.BanalFailedMedia;

        [RelayCommand]
        static void OnBW() => BWSystem.Instance.Toggle();

        [RelayCommand]
        static void OnLS() => LSSystem.Instance.Toggle();

        [RelayCommand]
        static void OnAura() => AuraSystem.Instance.Toggle();

        [RelayCommand]
        static void OnK70() => K70System.Instance.Toggle();

        [RelayCommand]
        static void OnGS() => GSSystem.Instance.Toggle();

        [RelayCommand]
        static void OnWooting() => WootingSystem.Instance.Toggle();

        [RelayCommand]
        static void OnMSI() => MSISystem.Instance.Toggle();

        [RelayCommand]
        static void OnCM() => CMSystem.Instance.Toggle();

        [RelayCommand]
        static async Task OnLoadBanalAudio()
        {
            var filePath = await StrongReferenceMessenger.Default.Send(new ViewFileWindow
            {
                Filters = QwilightComponent.AudioFileFormats
            });
            if (!string.IsNullOrEmpty(filePath))
            {
                Configure.Instance.BanalAudioFilePath = filePath;
                Configure.Instance.BanalAudio = true;
                AudioSystem.Instance.LoadBanalAudio();
            }
        }

        [RelayCommand]
        static void OnLoadDefaultAudio()
        {
            ViewModels.Instance.ModifyDefaultAudioFilePathValue.Open();
        }

        [RelayCommand]
        static void OnDefaultAudioVariety() => Configure.Instance.DefaultAudioVarietyValue = (Configure.DefaultAudioVariety)((int)(Configure.Instance.DefaultAudioVarietyValue + 1) % 3);

        [RelayCommand]
        static async Task OnLoadBanalMedia()
        {
            var filePath = await StrongReferenceMessenger.Default.Send(new ViewFileWindow
            {
                Filters = QwilightComponent.DrawingFileFormats.Concat(QwilightComponent.MediaFileFormats)
            });
            if (!string.IsNullOrEmpty(filePath))
            {
                Configure.Instance.BanalMediaFilePath = filePath;
                Configure.Instance.BanalMedia = true;
                ViewModels.Instance.MainValue.HandleAutoComputer();
            }
        }

        [RelayCommand]
        static async Task OnDefaultNote() => await TwilightSystem.Instance.GetDefaultNoteDate(0, false).ConfigureAwait(false);

        [RelayCommand]
        static async Task OnDefaultUI() => await TwilightSystem.Instance.GetDefaultUIDate(0, false).ConfigureAwait(false);

        [RelayCommand]
        static async Task OnLoadBanalFailedMedia()
        {
            var filePath = await StrongReferenceMessenger.Default.Send(new ViewFileWindow
            {
                Filters = QwilightComponent.DrawingFileFormats.Concat(QwilightComponent.MediaFileFormats)
            });
            if (!string.IsNullOrEmpty(filePath))
            {
                Configure.Instance.BanalFailedMediaFilePath = filePath;
                Configure.Instance.BanalFailedMedia = true;
                ViewModels.Instance.MainValue.HandleAutoComputer();
            }
        }

        [RelayCommand]
        static async Task OnMediaInputConfigure()
        {
            if (Configure.Instance.MediaInput)
            {
                await MediaInputSystem.Instance.GetMediaInputItems().ConfigureAwait(false);
            }
            else
            {
                MediaInputSystem.Instance.CloseMediaInput();
            }
        }

        [RelayCommand]
        static async Task OnGetMedia() => await MediaInputSystem.Instance.GetMediaInputItems().ConfigureAwait(false);

        [RelayCommand]
        static async Task OnGetQwilight() => await ViewModels.Instance.MainValue.GetQwilight(false).ConfigureAwait(false);

        [RelayCommand]
        static void OnAutoHighlight() => Configure.Instance.AutoHighlight = !Configure.Instance.AutoHighlight;

        [RelayCommand]
        static void OnAverager() => Configure.Instance.Averager = !Configure.Instance.Averager;

        [RelayCommand]
        static void OnEqualizer() => ViewModels.Instance.EqualizerValue.Open();

        [RelayCommand]
        static void OnTube() => Configure.Instance.Tube = !Configure.Instance.Tube;

        [RelayCommand]
        static void OnSFX() => Configure.Instance.SFX = !Configure.Instance.SFX;

        [RelayCommand]
        static void OnFlange() => Configure.Instance.Flange = !Configure.Instance.Flange;

        [RelayCommand]
        static void OnVibrationMode() => Configure.Instance.VibrationModeValue = (ControllerSystem.VibrationMode)((int)(Configure.Instance.VibrationModeValue + 1) % 3);

        [RelayCommand]
        static void OnAutoCompute() => Configure.Instance.AutoCompute = !Configure.Instance.AutoCompute;

        [RelayCommand]
        static void OnWindowedMode()
        {
            Configure.Instance.WindowedMode = !Configure.Instance.WindowedMode;
            StrongReferenceMessenger.Default.Send(new SetWindowedMode());
        }

        [RelayCommand]
        static void OnVESA() => Configure.Instance.VESAV2 = !Configure.Instance.VESAV2;

        [RelayCommand]
        static void OnDataCount3() => Configure.Instance.DataCount3 = !Configure.Instance.DataCount3;

        [RelayCommand]
        static void OnNVLLMode() => Configure.Instance.NVLLModeValue = (Configure.NVLLMode)(((int)Configure.Instance.NVLLModeValue + 1) % 3);

        [RelayCommand]
        static void OnTelnet() => TelnetSystem.Instance.Toggle();

        [RelayCommand]
        void OnSaveAsBundle()
        {
            switch (TabPositionUI)
            {
                case 0:
                    if (BaseUIItemValue != null)
                    {
                        TwilightSystem.Instance.SendParallel(Event.Types.EventID.SaveAsBundle, new
                        {
                            bundleVariety = BundleItem.BundleVariety.UI,
                            bundleName = BaseUIItemValue.YamlName,
                            bundleEntryPath = Path.Combine(BaseUIItemValue.UIEntry, BaseUIItemValue.YamlName)
                        });
                    }
                    break;
                case 1:
                    if (UIItemValue != null)
                    {
                        TwilightSystem.Instance.SendParallel(Event.Types.EventID.SaveAsBundle, new
                        {
                            bundleVariety = BundleItem.BundleVariety.UI,
                            bundleName = UIItemValue.YamlName,
                            bundleEntryPath = Path.Combine(UIItemValue.UIEntry, UIItemValue.YamlName)
                        });
                    }
                    break;
            }
        }

        void SetUI(int i)
        {
            var target = i == 0 ? BaseUIItemValue : UIItemValue;
            Titles[i] = target?.Title ?? string.Empty;
            Comments[i] = string.Empty;
            Drawings[i] = null;
            if (target != null)
            {
                try
                {
                    string zipName;

                    var ys = new YamlStream();
                    using (var sr = File.OpenText(target.GetYamlFilePath()))
                    {
                        ys.Load(sr);
                        var valueNode = ys.Documents[0].RootNode;
                        var formatNode = valueNode[new YamlScalarNode("format")];
                        Titles[i] = Utility.GetText(formatNode, $"title-{Utility.GetLCID(Configure.Instance.Language)}") ?? Utility.GetText(formatNode, "title") ?? target.Title;
                        Comments[i] = Utility.GetText(formatNode, $"comment-{Utility.GetLCID(Configure.Instance.Language)}") ?? Utility.GetText(formatNode, "comment");
                        zipName = $"{(i == 0 ? '@' : string.Empty)}{Utility.GetText(formatNode, "zip")}";
                    }

                    var zipFilePath = Path.Combine(QwilightComponent.UIEntryPath, target.UIEntry, Path.ChangeExtension(zipName, "zip"));
                    if (File.Exists(zipFilePath))
                    {
                        var drawingFileName = $"_{target.YamlName.Substring(i == 0 ? 1 : 0)}.";
                        using var zipFile = new ZipFile(zipFilePath);
                        var drawingZipEntry = zipFile.FirstOrDefault(zipEntry => zipEntry.FileName.IsFrontCaselsss(drawingFileName));
                        if (drawingZipEntry != null)
                        {
                            try
                            {
                                using var rms = PoolSystem.Instance.GetDataFlow((int)drawingZipEntry.UncompressedSize);
                                drawingZipEntry.Extract(rms);
                                Drawings[i] = DrawingSystem.Instance.LoadDefault(rms, null);
                            }
                            catch
                            {
                            }
                        }
                    }
                }
                catch
                {
                }
            }

            OnPropertyChanged(nameof(Titles));
            OnPropertyChanged(nameof(Comments));
            OnPropertyChanged(nameof(Drawings));
        }

        public ObservableCollection<HwMode> HwModeCollection { get; } = new();

        public string DateText { get; } = $"v{QwilightComponent.DateText} ({QwilightComponent.HashText.Substring(0, 8)})";

        public bool IsValve => QwilightComponent.IsValve;

        public bool DInputBMSControllerMode
        {
            get => _dInputControllerVarietyInputs[0];

            set
            {
                if (SetProperty(ref _dInputControllerVarietyInputs[0], value, nameof(DInputBMSControllerMode)) && value)
                {
                    Configure.Instance.DInputControllerVarietyValue = ControllerSystem.DInputControllerVariety.BMS;
                }
            }
        }

        public bool DInputIIDXControllerMode
        {
            get => _dInputControllerVarietyInputs[1];

            set
            {
                if (SetProperty(ref _dInputControllerVarietyInputs[1], value, nameof(DInputIIDXControllerMode)) && value)
                {
                    Configure.Instance.DInputControllerVarietyValue = ControllerSystem.DInputControllerVariety.IIDX;
                }
            }
        }

        public bool CanGetDefaultText => TwilightSystem.Instance.IsEstablished && Configure.Instance.DetailedConfigure;

        public void NotifyCanGetDefaultText() => OnPropertyChanged(nameof(CanGetDefaultText));

        public bool CanSaveAsBundle => TwilightSystem.Instance.IsLoggedIn && TabPositionUI != 2;

        public void NotifyCanSaveAsBundle() => OnPropertyChanged(nameof(CanSaveAsBundle));

        public double CPUCount => QwilightComponent.CPUCount;

        public string MainControllerPowerStatus
        {
            get => _mainControllerPowerStatus;

            set => SetProperty(ref _mainControllerPowerStatus, value, nameof(MainControllerPowerStatus));
        }

        public bool DefaultInputAudioVariety
        {
            get => _inputAudioVarietyInputs[0];

            set
            {
                if (SetProperty(ref _inputAudioVarietyInputs[0], value, nameof(DefaultInputAudioVariety)) && value)
                {
                    Configure.Instance.InputAudioVarietyValue = Configure.InputAudioVariety.DJMAX;
                }
            }
        }

        public bool IIDXInputAudioVariety
        {
            get => _inputAudioVarietyInputs[1];

            set
            {
                if (SetProperty(ref _inputAudioVarietyInputs[1], value, nameof(IIDXInputAudioVariety)) && value)
                {
                    Configure.Instance.InputAudioVarietyValue = Configure.InputAudioVariety.IIDX;
                }
            }
        }

        public bool AudioVarietyWASAPI
        {
            get => _audioVarietyInputs[0];

            set
            {
                if (SetProperty(ref _audioVarietyInputs[0], value, nameof(AudioVarietyWASAPI)) && value)
                {
                    AudioSystem.Instance.SetAudioVariety(Configure.Instance.AudioVariety = OUTPUTTYPE.WASAPI);
                }
            }
        }

        public bool AudioVarietyASIO
        {
            get => _audioVarietyInputs[1];

            set
            {
                if (SetProperty(ref _audioVarietyInputs[1], value, nameof(AudioVarietyASIO)) && value)
                {
                    AudioSystem.Instance.SetAudioVariety(Configure.Instance.AudioVariety = OUTPUTTYPE.ASIO);
                }
            }
        }

        public bool DefaultGPUMode
        {
            get => _valueGPUModeInputs[0];

            set
            {
                if (SetProperty(ref _valueGPUModeInputs[0], value, nameof(DefaultGPUMode)) && value)
                {
                    GPUConfigure.Instance.GPUModeValue = GPUConfigure.GPUMode.Default;
                }
            }
        }

        public bool NVIDIAGPUMode
        {
            get => _valueGPUModeInputs[1];

            set
            {
                if (SetProperty(ref _valueGPUModeInputs[1], value, nameof(NVIDIAGPUMode)) && value)
                {
                    GPUConfigure.Instance.GPUModeValue = GPUConfigure.GPUMode.NVIDIA;
                }
            }
        }

        public bool LoopUnit1000
        {
            get => _loopUnitInputs[0];

            set
            {
                if (SetProperty(ref _loopUnitInputs[0], value, nameof(LoopUnit1000)) && value)
                {
                    Configure.Instance.LoopUnit = 1000;
                }
            }
        }

        public bool LoopUnit2000
        {
            get => _loopUnitInputs[1];

            set
            {
                if (SetProperty(ref _loopUnitInputs[1], value, nameof(LoopUnit2000)) && value)
                {
                    Configure.Instance.LoopUnit = 2000;
                }
            }
        }

        public bool LoopUnit4000
        {
            get => _loopUnitInputs[2];

            set
            {
                if (SetProperty(ref _loopUnitInputs[2], value, nameof(LoopUnit4000)) && value)
                {
                    Configure.Instance.LoopUnit = 4000;
                }
            }
        }

        public bool LoopUnit8000
        {
            get => _loopUnitInputs[3];

            set
            {
                if (SetProperty(ref _loopUnitInputs[3], value, nameof(LoopUnit8000)) && value)
                {
                    Configure.Instance.LoopUnit = 8000;
                }
            }
        }

        public int DetailedAudioInputValue
        {
            get => _detailedAudioInputValue;

            set => SetProperty(ref _detailedAudioInputValue, value, nameof(DetailedAudioInputValue));
        }

        public long DetailedSet
        {
            get => _detailedSet;

            set => SetProperty(ref _detailedSet, value, nameof(DetailedSet));
        }

        public string DetailedAudioItemCount
        {
            get => _detailedAudioItemCount;

            set => SetProperty(ref _detailedAudioItemCount, value, nameof(DetailedAudioItemCount));
        }

        public string DetailedAudioHandlerItemCount
        {
            get => _detailedAudioHandlerItemCount;

            set => SetProperty(ref _detailedAudioHandlerItemCount, value, nameof(DetailedAudioHandlerItemCount));
        }

        public string DetailedHandlingAudioCount
        {
            get => _detailedHandlingAudioCount;

            set => SetProperty(ref _detailedHandlingAudioCount, value, nameof(DetailedHandlingAudioCount));
        }

        public float DetailedAudioUnitStatus
        {
            get => _detailedAudioUnitStatus;

            set => SetProperty(ref _detailedAudioUnitStatus, value, nameof(DetailedAudioUnitStatus));
        }

        public string DetailedMediaItemCount
        {
            get => _detailedMediaItemCount;

            set => SetProperty(ref _detailedMediaItemCount, value, nameof(DetailedMediaItemCount));
        }

        public string DetailedMediaHandlerItemCount
        {
            get => _detailedMediaHandlerItemCount;

            set => SetProperty(ref _detailedMediaHandlerItemCount, value, nameof(DetailedMediaHandlerItemCount));
        }

        public string DetailedDrawingItemCount
        {
            get => _detailedDrawingItemCount;

            set => SetProperty(ref _detailedDrawingItemCount, value, nameof(DetailedDrawingItemCount));
        }

        public string DetailedNoteDrawingCount
        {
            get => _detailedNoteDrawingCount;

            set => SetProperty(ref _detailedNoteDrawingCount, value, nameof(DetailedNoteDrawingCount));
        }

        public void OnTotalVolume(float audioVolume)
        {
            if (IsLoaded)
            {
                AudioSystem.Instance.SetVolume(AudioSystem.TotalAudio, audioVolume);
            }
        }

        public void OnMainVolume(float audioVolume)
        {
            if (IsLoaded)
            {
                AudioSystem.Instance.SetVolume(AudioSystem.MainAudio, audioVolume);
            }
        }

        public void OnInputVolume(float audioVolume)
        {
            if (IsLoaded)
            {
                AudioSystem.Instance.SetVolume(AudioSystem.InputAudio, audioVolume);
            }
        }

        public void OnSEVolume(float audioVolume)
        {
            if (IsLoaded)
            {
                AudioSystem.Instance.SetVolume(AudioSystem.SEAudio, audioVolume);
            }
        }

        public ConfigureViewModel()
        {
            _detailedHandler = new(DispatcherPriority.Background, UIHandler.Instance.Handler)
            {
                Interval = TimeSpan.FromSeconds(1)
            };
            _detailedHandler.Tick += (sender, e) =>
            {
                DetailedSet = Environment.WorkingSet;
                DetailedAudioItemCount = AudioSystem.Instance.AudioItemCount.ToString(LanguageSystem.Instance.OpenedCountContents);
                DetailedAudioHandlerItemCount = AudioSystem.Instance.AudioHandlerItemCount.ToString(LanguageSystem.Instance.OpenedCountContents);
                DetailedAudioInputValue = (int)AudioInputSystem.Instance.AudioInputValue;
                DetailedHandlingAudioCount = AudioSystem.Instance.GetHandlingAudioCount().ToString(LanguageSystem.Instance.OpenedCountContents);
                DetailedAudioUnitStatus = AudioSystem.Instance.GetAudioUnitStatus() / 100;
                DetailedDrawingItemCount = DrawingSystem.Instance.DrawingItemCount.ToString(LanguageSystem.Instance.OpenedCountContents);
                DetailedMediaItemCount = MediaSystem.Instance.MediaItemCount.ToString(LanguageSystem.Instance.OpenedCountContents);
                DetailedMediaHandlerItemCount = MediaSystem.Instance.MediaHandlerItemCount.ToString(LanguageSystem.Instance.OpenedCountContents);
                DetailedNoteDrawingCount = ViewModels.Instance.MainValue.NoteDrawingCount.ToString(LanguageSystem.Instance.OpenedCountContents);
                MainControllerPowerStatus = ControllerSystem.Instance.MainControllerPower;
                ControllerSystem.Instance.MainControllerPower = null;
            };
        }

        void Init()
        {
            switch (Configure.Instance.DInputControllerVarietyValue)
            {
                case ControllerSystem.DInputControllerVariety.BMS:
                    DInputBMSControllerMode = true;
                    break;
                case ControllerSystem.DInputControllerVariety.IIDX:
                    DInputIIDXControllerMode = true;
                    break;
            }
            switch (Configure.Instance.InputAudioVarietyValue)
            {
                case Configure.InputAudioVariety.DJMAX:
                    DefaultInputAudioVariety = true;
                    break;
                case Configure.InputAudioVariety.IIDX:
                    IIDXInputAudioVariety = true;
                    break;
            }
            switch (Configure.Instance.LoopUnit)
            {
                case 1000:
                    LoopUnit1000 = true;
                    break;
                case 2000:
                    LoopUnit2000 = true;
                    break;
                case 4000:
                    LoopUnit4000 = true;
                    break;
                case 8000:
                    LoopUnit8000 = true;
                    break;
            }
            switch (Configure.Instance.AudioVariety)
            {
                case OUTPUTTYPE.WASAPI:
                    AudioVarietyWASAPI = true;
                    break;
                case OUTPUTTYPE.ASIO:
                    AudioVarietyASIO = true;
                    break;
            }
            switch (GPUConfigure.Instance.GPUModeValue)
            {
                case GPUConfigure.GPUMode.Default:
                    DefaultGPUMode = true;
                    break;
                case GPUConfigure.GPUMode.NVIDIA:
                    NVIDIAGPUMode = true;
                    break;
            }
            switch (Configure.Instance.AudioVisualizerModeValue)
            {
                case Configure.AudioVisualizerMode.AudioVisualizerMode0:
                    AudioVisualizerMode0 = true;
                    break;
                case Configure.AudioVisualizerMode.AudioVisualizerMode1:
                    AudioVisualizerMode1 = true;
                    break;
                case Configure.AudioVisualizerMode.AudioVisualizerMode2:
                    AudioVisualizerMode2 = true;
                    break;
            }
            switch (Configure.Instance.InputMappingValue)
            {
                case Component.InputMapping.Mapping0:
                    InputMapping0 = true;
                    break;
                case Component.InputMapping.Mapping1:
                    InputMapping1 = true;
                    break;
                case Component.InputMapping.Mapping2:
                    InputMapping2 = true;
                    break;
                case Component.InputMapping.Mapping3:
                    InputMapping3 = true;
                    break;
            }
        }

        public override void OnOpened()
        {
            base.OnOpened();
            Init();
            OnSetTabPositionUI();

            var mainViewModel = ViewModels.Instance.MainValue;
            if (!mainViewModel.IsNoteFileMode)
            {
                mainViewModel.ModeComponentValue.SetMultiplier(mainViewModel.Computer.ModeComponentValue.MultiplierValue);
                mainViewModel.ModeComponentValue.SetAudioMultiplier(mainViewModel.Computer.AudioMultiplier);
            }
            _defaultGPUMode = GPUConfigure.Instance.GPUModeValue;
            _defaultWMPointer = Configure.Instance.WMPointer;
            _defaultHunterVariety = Configure.Instance.HunterVarietyV2Value;
            _defaultNetCommentFollow = Configure.Instance.NetCommentFollow;
            _defaultAudioDataLength = Configure.Instance.AudioDataLength;
            _detailedHandler.Start();

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
            hwModeCollection.Sort();
            Utility.SetUICollection(HwModeCollection, hwModeCollection);
        }

        public override void OnCollapsed()
        {
            base.OnCollapsed();
            Configure.Instance.Save(true);
            GPUConfigure.Instance.Save(true);
            PoolSystem.Instance.Wipe();
            var mainViewModel = ViewModels.Instance.MainValue;
            mainViewModel.OnJudgmentMeterMillisModified();
            mainViewModel.HandleAutoComputer();
            ViewModels.Instance.SiteContainerValue.CallSetModeComponent();
            var handlingComputer = ViewModels.Instance.MainValue.GetHandlingComputer();
            if (handlingComputer != null)
            {
                handlingComputer.SetUIMap();
                if (_defaultHunterVariety != Configure.Instance.HunterVarietyV2Value || _defaultNetCommentFollow != Configure.Instance.NetCommentFollow)
                {
                    handlingComputer.InitNetComments();
                }
            }
            if (mainViewModel.IsNoteFileMode)
            {
                mainViewModel.NotifyBPMText();
                mainViewModel.NotifyHighestInputCountText();
            }
            _detailedHandler.Stop();
            if (_defaultGPUMode != GPUConfigure.Instance.GPUModeValue)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.GPUModeModified);
            }
            if (_defaultWMPointer != Configure.Instance.WMPointer)
            {
                NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, LanguageSystem.Instance.WMPointerModified);
            }
            if (_defaultAudioDataLength != Configure.Instance.AudioDataLength)
            {
                mainViewModel.CloseAutoComputer();
                AudioSystem.Instance.Dispose();
                AudioSystem.Instance.Init();
                AudioSystem.Instance.LoadDefaultAudioItems();
                AudioSystem.Instance.LoadBanalAudio();
                Task.Run(() =>
                {
                    BaseUI.Instance.LoadUI(null, Configure.Instance.BaseUIItemValue, false);
                    UI.Instance.LoadUI(null, Configure.Instance.UIItemValue, false);
                });
            }
            mainViewModel.AutoComputer?.SetWait();
            mainViewModel.HandleAutoComputer();
        }
    }
}