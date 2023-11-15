using CommunityToolkit.Mvvm.Messaging;
using Qwilight.Compute;
using Qwilight.MSG;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;
using System.IO;
using Vortice.DirectInput;
using Windows.System;
using Windows.Win32.Foundation;

namespace Qwilight
{
    public sealed class DefaultControllerSystem : Model, IHandleInput
    {
        public enum InputAPI
        {
            DefaultInput, DInput
        }

        public static readonly DefaultControllerSystem Instance = QwilightComponent.GetBuiltInData<DefaultControllerSystem>(nameof(DefaultControllerSystem));

        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(DefaultControllerSystem));

        readonly HashSet<DefaultInput> _defaultInputs = new();
        readonly Stopwatch _loopingHandler = Stopwatch.StartNew();
        readonly TimerCallback _onLowerMultiplier = new(state => ViewModels.Instance.MainValue.LowerMultiplier());
        readonly TimerCallback _onHigherMultiplier = new(state => ViewModels.Instance.MainValue.HigherMultiplier());
        readonly TimerCallback _onLowerAudioMultiplier = new(state => ViewModels.Instance.MainValue.LowerAudioMultiplier());
        readonly TimerCallback _onHigherAudioMultiplier = new(state => ViewModels.Instance.MainValue.HigherAudioMultiplier());
        readonly Dictionary<VirtualKey, double> _fastInputMillis = new();
        DefaultSystem _defaultSystem;
        DInputSystem _dInputSystem;
        string _lastDefaultControllerInput;
        Timer _lowerMultiplierHandler;
        Timer _higherMultiplierHandler;
        Timer _lowerAudioMultiplierHandler;
        Timer _higherAudioMultiplierHandler;
        Timer _lowerSpinningValueHandler;
        Timer _higherSpinningValueHandler;

        public void Init()
        {
            _lowerMultiplierHandler?.Dispose();
            _lowerMultiplierHandler = null;
            _higherMultiplierHandler?.Dispose();
            _higherMultiplierHandler = null;
            _lowerAudioMultiplierHandler?.Dispose();
            _lowerAudioMultiplierHandler = null;
            _higherAudioMultiplierHandler?.Dispose();
            _higherAudioMultiplierHandler = null;
            _lowerSpinningValueHandler?.Dispose();
            _lowerSpinningValueHandler = null;
            _higherSpinningValueHandler?.Dispose();
            _higherSpinningValueHandler = null;
        }

        public string LastDefaultControllerInput
        {
            get => _lastDefaultControllerInput;

            set => SetProperty(ref _lastDefaultControllerInput, value, nameof(LastDefaultControllerInput));
        }

        public void HandleSystem()
        {
            if (_defaultSystem != null)
            {
                _defaultSystem.Dispose();
                _defaultSystem = null;
            }
            if (_dInputSystem != null)
            {
                _dInputSystem.Dispose();
                _dInputSystem = null;
            }
            switch (Configure.Instance.DefaultControllerInputAPI)
            {
                case InputAPI.DefaultInput:
                    _defaultSystem = new DefaultSystem(this);
                    _defaultSystem.HandleSystem();
                    break;
                case InputAPI.DInput:
                    _dInputSystem = new DInputSystem(this);
                    WeakReferenceMessenger.Default.Send<ICC>(new()
                    {
                        IDValue = ICC.ID.GetWindowHandle,
                        Contents = new Action<HWND>(handle => _dInputSystem.HandleSystem(handle))
                    });
                    break;
            }
        }

        public void HandleInput(VirtualKey rawInput, bool isInput)
        {
            if (rawInput != VirtualKey.None)
            {
                var millis = _loopingHandler.GetMillis();
                if (millis - _fastInputMillis.GetValueOrDefault(rawInput) >= Configure.Instance.FastInputMillis)
                {
                    _fastInputMillis[rawInput] = millis;
                    var isAlt = Utility.HasInput(VirtualKey.LeftMenu);
                    if (rawInput != VirtualKey.F4 || !isAlt)
                    {
                        var mainViewModel = ViewModels.Instance.MainValue;
                        var inputViewModel = ViewModels.Instance.InputValue;
                        var inputStandardViewModel = ViewModels.Instance.InputStandardValue;
                        var inputStandardControllerViewModel = ViewModels.Instance.InputStandardControllerValue;
                        var valueConfigureViewModel = ViewModels.Instance.ConfigureValue;
                        var defaultInput = new DefaultInput
                        {
                            Data = rawInput
                        };
                        var input = Array.IndexOf(Configure.Instance.DefaultInputBundlesV6.StandardInputs, defaultInput);
                        if (ViewModels.Instance.ConfigureValue.IsOpened)
                        {
                            LastDefaultControllerInput = $"{(isInput ? "＋" : "－")} {nameof(Key)}: {rawInput}";
                        }
                        if (isInput)
                        {
                            if (inputViewModel.IsOpened)
                            {
                                inputViewModel.OnDefaultInputLower(rawInput);
                            }
                            if (inputStandardViewModel.IsOpened)
                            {
                                inputStandardViewModel.OnDefaultInputLower(rawInput);
                            }
                            if (inputStandardControllerViewModel.IsOpened)
                            {
                                inputStandardControllerViewModel.OnDefaultInputLower(rawInput);
                            }
                            var atModeComponentWindow = valueConfigureViewModel.IsOpened && valueConfigureViewModel.TabPosition == 0 && valueConfigureViewModel.TabPositionComputing == 0;
                            switch (rawInput)
                            {
                                case VirtualKey.F1 when atModeComponentWindow:
                                case VirtualKey.F2 when atModeComponentWindow:
                                case VirtualKey.F3 when atModeComponentWindow:
                                case VirtualKey.F4 when atModeComponentWindow:
                                case VirtualKey.F5 when atModeComponentWindow:
                                case VirtualKey.F6 when atModeComponentWindow:
                                case VirtualKey.F7 when atModeComponentWindow:
                                case VirtualKey.F8 when atModeComponentWindow:
                                    var modeComponentValue = mainViewModel.ModeComponentValue;
                                    var i = rawInput - VirtualKey.F1;
                                    var modeComponentBundle = Configure.Instance.ModeComponentBundles[i];
                                    var modeComponentBundleValue = modeComponentBundle.Value;
                                    if (mainViewModel.CanModifyModeComponent)
                                    {
                                        modeComponentValue.CopyAs(modeComponentBundleValue);
                                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.LoadedModeComponent, i + 1, modeComponentBundle.Name), true, null, null, NotifySystem.ModeComponentID);
                                    }
                                    else if (modeComponentValue.CanModifyMultiplier || modeComponentValue.CanModifyAudioMultiplier)
                                    {
                                        if (modeComponentValue.CanModifyMultiplier)
                                        {
                                            modeComponentValue.MultiplierValue = modeComponentBundleValue.MultiplierValue;
                                        }
                                        if (modeComponentValue.CanModifyAudioMultiplier)
                                        {
                                            modeComponentValue.AudioMultiplier = modeComponentBundleValue.AudioMultiplier;
                                        }
                                        NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.OK, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.LoadedMultiplier, i + 1, modeComponentBundle.Name), true, null, null, NotifySystem.ModeComponentID);
                                    }
                                    break;
                                case VirtualKey.F6:
                                    mainViewModel.HandleF6();
                                    break;
                                case VirtualKey.F7:
                                    mainViewModel.HandleF7();
                                    break;
                                case VirtualKey.F8:
                                    mainViewModel.HandleF8();
                                    break;
                                case VirtualKey.F9:
                                    mainViewModel.HandleF9();
                                    break;
                                case VirtualKey.F10:
                                    mainViewModel.HandleF10();
                                    break;
                                case VirtualKey.F11:
                                    mainViewModel.HandleF11();
                                    break;
                                case VirtualKey.F12 when !QwilightComponent.IsValve:
                                    mainViewModel.HandleF12();
                                    break;
                                case VirtualKey.Escape when !isAlt:
                                    mainViewModel.HandleESC();
                                    break;
                                case VirtualKey.Space:
                                    mainViewModel.HandleSpace();
                                    break;
                                case VirtualKey.Pause:
                                    mainViewModel.Pause();
                                    break;
                                case VirtualKey.Enter when isAlt:
                                    Configure.Instance.WindowedMode = !Configure.Instance.WindowedMode;
                                    StrongReferenceMessenger.Default.Send(new SetWindowedMode());
                                    break;
                            }
                            switch (input)
                            {
                                case InputStandardViewModel.LowerMultiplier when _lowerMultiplierHandler == null:
                                    _lowerMultiplierHandler = new(_onLowerMultiplier, null, 0, (int)QwilightComponent.StandardLoopMillis);
                                    break;
                                case InputStandardViewModel.HigherMultiplier when _higherMultiplierHandler == null:
                                    _higherMultiplierHandler = new(_onHigherMultiplier, null, 0, (int)QwilightComponent.StandardLoopMillis);
                                    break;
                                case InputStandardViewModel.LowerAudioMultiplier when _lowerAudioMultiplierHandler == null:
                                    _onLowerAudioMultiplier(null);
                                    _lowerAudioMultiplierHandler = new(_onLowerAudioMultiplier, null, (int)QwilightComponent.StandardWaitMillis, (int)QwilightComponent.StandardLoopMillis);
                                    break;
                                case InputStandardViewModel.HigherAudioMultiplier when _higherAudioMultiplierHandler == null:
                                    _onHigherAudioMultiplier(null);
                                    _higherAudioMultiplierHandler = new(_onHigherAudioMultiplier, null, (int)QwilightComponent.StandardWaitMillis, (int)QwilightComponent.StandardLoopMillis);
                                    break;
                                case InputStandardViewModel.Media:
                                    mainViewModel.HandleMediaMode();
                                    break;
                                case InputStandardViewModel.Undo:
                                    mainViewModel.HandleUndo();
                                    break;
                            }
                        }
                        else
                        {
                            switch (rawInput)
                            {
                                case VirtualKey.Left:
                                    _lowerSpinningValueHandler?.Dispose();
                                    _lowerSpinningValueHandler = null;
                                    mainViewModel.UndoUnitMultiplier();
                                    break;
                                case VirtualKey.Right:
                                    _higherSpinningValueHandler?.Dispose();
                                    _higherSpinningValueHandler = null;
                                    mainViewModel.UndoUnitMultiplier();
                                    break;
                                case (VirtualKey)179:
                                    mainViewModel.Pause();
                                    break;
                            }
                            switch (input)
                            {
                                case InputStandardViewModel.LowerMultiplier:
                                    _lowerMultiplierHandler?.Dispose();
                                    _lowerMultiplierHandler = null;
                                    mainViewModel.UndoUnitMultiplier();
                                    break;
                                case InputStandardViewModel.HigherMultiplier:
                                    _higherMultiplierHandler?.Dispose();
                                    _higherMultiplierHandler = null;
                                    mainViewModel.UndoUnitMultiplier();
                                    break;
                                case InputStandardViewModel.LowerAudioMultiplier:
                                    _lowerAudioMultiplierHandler?.Dispose();
                                    _lowerAudioMultiplierHandler = null;
                                    break;
                                case InputStandardViewModel.HigherAudioMultiplier:
                                    _higherAudioMultiplierHandler?.Dispose();
                                    _higherAudioMultiplierHandler = null;
                                    break;
                            }
                        }
                        switch (mainViewModel.ModeValue)
                        {
                            case MainViewModel.Mode.NoteFile:
                                switch (rawInput)
                                {
                                    case VirtualKey.LeftShift:
                                        mainViewModel.HandleShift(isInput);
                                        break;
                                    case VirtualKey.F1 when isInput:
                                        mainViewModel.HandleF1();
                                        break;
                                    case VirtualKey.F5 when isInput:
                                        mainViewModel.HandleF5();
                                        break;
                                    case (VirtualKey)177:
                                        mainViewModel.LowerEntryItem();
                                        break;
                                    case (VirtualKey)176:
                                        mainViewModel.HigherEntryItem();
                                        break;
                                }
                                break;
                            case MainViewModel.Mode.Computing:
                                var defaultComputer = mainViewModel.Computer;
                                if (isInput)
                                {
                                    switch (rawInput)
                                    {
                                        case VirtualKey.Up:
                                            if (defaultComputer.IsPausingWindowOpened)
                                            {
                                                mainViewModel.HigherDefaultSpinningMode();
                                            }
                                            break;
                                        case VirtualKey.Down:
                                            if (defaultComputer.IsPausingWindowOpened)
                                            {
                                                mainViewModel.LowerDefaultSpinningMode();
                                            }
                                            break;
                                        case VirtualKey.Enter when !isAlt:
                                            mainViewModel.HandleEnter();
                                            break;
                                    }
                                    switch (input)
                                    {
                                        case InputStandardViewModel.ModifyAutoMode:
                                            mainViewModel.HandleModifyAutoMode();
                                            break;
                                        case InputStandardViewModel.PostItem0:
                                            mainViewModel.PostItem(0);
                                            break;
                                        case InputStandardViewModel.PostItem1:
                                            mainViewModel.PostItem(1);
                                            break;
                                    }
                                }
                                if (isInput)
                                {
                                    if (_defaultInputs.Add(defaultInput))
                                    {
                                        mainViewModel.Input(Configure.Instance.DefaultInputBundlesV6.Inputs, defaultInput, true, DefaultCompute.InputFlag.DefaultController);
                                    }
                                }
                                else
                                {
                                    if (_defaultInputs.Remove(defaultInput))
                                    {
                                        mainViewModel.Input(Configure.Instance.DefaultInputBundlesV6.Inputs, defaultInput, false, DefaultCompute.InputFlag.DefaultController);
                                    }
                                }
                                break;
                            case MainViewModel.Mode.Quit:
                                if (isInput)
                                {
                                    defaultComputer = mainViewModel.Computer;
                                    switch (rawInput)
                                    {
                                        case VirtualKey.Left when defaultComputer.LevyingComputingPosition > 0:
                                            defaultComputer.NotifyCompute(-1);
                                            break;
                                        case VirtualKey.Right when defaultComputer.LevyingComputingPosition < defaultComputer.HighestComputingPosition:
                                            defaultComputer.NotifyCompute(1);
                                            break;
                                        case VirtualKey.Enter when !isAlt:
                                            mainViewModel.HandleEnter();
                                            break;
                                    }
                                }
                                break;
                        }
                    }
                }
            }
        }
    }
}