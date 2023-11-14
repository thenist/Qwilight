using Qwilight.Compute;
using Qwilight.Utilities;
using Qwilight.ViewModel;
using System.Diagnostics;

namespace Qwilight
{
    public sealed class HandlingController<T>
    {
        readonly HashSet<T> _rawInputs = new();
        readonly Stopwatch _loopingHandler = Stopwatch.StartNew();
        readonly Action<T> _onInputViewModel;
        readonly Action<T> _onStandardInputControllerViewModel;
        readonly TimerCallback _onLowerMultiplier = new(state => ViewModels.Instance.MainValue.LowerMultiplier());
        readonly TimerCallback _onHigherMultiplier = new(state => ViewModels.Instance.MainValue.HigherMultiplier());
        readonly TimerCallback _onLowerEntry = new(state =>
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            switch (mainViewModel.ModeValue)
            {
                case MainViewModel.Mode.NoteFile:
                    mainViewModel.LowerEntryItem();
                    break;
                case MainViewModel.Mode.Computing:
                    if (mainViewModel.Computer.IsPausingWindowOpened)
                    {
                        mainViewModel.LowerDefaultSpinningMode();
                    }
                    break;
            }
        });
        readonly TimerCallback _onHigherEntry = new(state =>
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            switch (mainViewModel.ModeValue)
            {
                case MainViewModel.Mode.NoteFile:
                    mainViewModel.HigherEntryItem();
                    break;
                case MainViewModel.Mode.Computing:
                    if (mainViewModel.Computer.IsPausingWindowOpened)
                    {
                        mainViewModel.HigherDefaultSpinningMode();
                    }
                    break;
            }
        });
        readonly DefaultCompute.InputFlag _inputFlag;
        double _loopingCounter;
        double _atLoopingCounter1000;
        Timer _lowerMultiplierHandler;
        Timer _higherMultiplierHandler;
        Timer _lowerEntryHandler;
        Timer _higherEntryHandler;
        Timer _lowerSpinningValueHandler;
        Timer _higherSpinningValueHandler;

        public HandlingController(Action<T> onInputViewModel, Action<T> onStandardInputControllerViewModel, DefaultCompute.InputFlag inputFlag)
        {
            _onInputViewModel = onInputViewModel;
            _onStandardInputControllerViewModel = onStandardInputControllerViewModel;
            _inputFlag = inputFlag;
        }

        public void Init()
        {
            _lowerMultiplierHandler?.Dispose();
            _lowerMultiplierHandler = null;
            _higherMultiplierHandler?.Dispose();
            _higherMultiplierHandler = null;
            _lowerEntryHandler?.Dispose();
            _lowerEntryHandler = null;
            _higherEntryHandler?.Dispose();
            _higherEntryHandler = null;
            _lowerSpinningValueHandler?.Dispose();
            _lowerSpinningValueHandler = null;
            _higherSpinningValueHandler?.Dispose();
            _higherSpinningValueHandler = null;
        }

        public void Input(T rawInput, T[] inputStandardConfigure, T[][][] inputConfigure, bool isInput, byte inputPower = byte.MaxValue)
        {
            var mainViewModel = ViewModels.Instance.MainValue;
            var input = Array.IndexOf(inputStandardConfigure, rawInput);
            if (isInput)
            {
                var isInputWindowOpened = ViewModels.Instance.InputValue.IsOpened;
                var isInputStandardWindowOpened = ViewModels.Instance.InputStandardControllerValue.IsOpened;
                if (isInputWindowOpened)
                {
                    _onInputViewModel(rawInput);
                }
                if (isInputStandardWindowOpened)
                {
                    _onStandardInputControllerViewModel(rawInput);
                }
                switch (input)
                {
                    case InputStandardControllerViewModel.Wait when !isInputWindowOpened && !isInputStandardWindowOpened:
                        mainViewModel.HandleESC();
                        break;
                    case InputStandardControllerViewModel.LowerMultiplier when _lowerMultiplierHandler == null:
                        _lowerMultiplierHandler = new(_onLowerMultiplier, null, 0, (int)QwilightComponent.StandardLoopMillis);
                        break;
                    case InputStandardControllerViewModel.HigherMultiplier when _higherMultiplierHandler == null:
                        _higherMultiplierHandler = new(_onHigherMultiplier, null, 0, (int)QwilightComponent.StandardLoopMillis);
                        break;
                    case InputStandardControllerViewModel.MediaMode:
                        mainViewModel.HandleMediaMode();
                        break;
                    case InputStandardControllerViewModel.HandleUndo:
                        mainViewModel.HandleUndo();
                        break;
                }
            }
            else
            {
                switch (input)
                {
                    case InputStandardControllerViewModel.LowerEntry:
                        _lowerEntryHandler?.Dispose();
                        _lowerEntryHandler = null;
                        break;
                    case InputStandardControllerViewModel.HigherEntry:
                        _higherEntryHandler?.Dispose();
                        _higherEntryHandler = null;
                        break;
                    case InputStandardControllerViewModel.LowerNoteFile:
                        _lowerSpinningValueHandler?.Dispose();
                        _lowerSpinningValueHandler = null;
                        mainViewModel.UndoUnitMultiplier();
                        break;
                    case InputStandardControllerViewModel.HigherNoteFile:
                        _higherSpinningValueHandler?.Dispose();
                        _higherSpinningValueHandler = null;
                        mainViewModel.UndoUnitMultiplier();
                        break;
                    case InputStandardControllerViewModel.LowerMultiplier:
                        _lowerMultiplierHandler?.Dispose();
                        _lowerMultiplierHandler = null;
                        mainViewModel.UndoUnitMultiplier();
                        break;
                    case InputStandardControllerViewModel.HigherMultiplier:
                        _higherMultiplierHandler?.Dispose();
                        _higherMultiplierHandler = null;
                        mainViewModel.UndoUnitMultiplier();
                        break;
                }
            }
            var defaultComputer = mainViewModel.Computer;
            switch (mainViewModel.ModeValue)
            {
                case MainViewModel.Mode.NoteFile:
                    if (isInput)
                    {
                        switch (input)
                        {
                            case InputStandardControllerViewModel.LevyNoteFile:
                                mainViewModel.HandleLevyNoteFile();
                                break;
                            case InputStandardControllerViewModel.LowerEntry:
                                _lowerEntryHandler = new(_onLowerEntry, null, 0, 1000 / 7);
                                break;
                            case InputStandardControllerViewModel.HigherEntry:
                                _higherEntryHandler = new(_onHigherEntry, null, 0, 1000 / 7);
                                break;
                            case InputStandardControllerViewModel.LowerNoteFile:
                                mainViewModel.LowerNoteFile();
                                break;
                            case InputStandardControllerViewModel.HigherNoteFile:
                                mainViewModel.HigherNoteFile();
                                break;
                        }
                    }
                    break;
                case MainViewModel.Mode.Computing:
                    if (isInput)
                    {
                        if (_rawInputs.Add(rawInput))
                        {
                            mainViewModel.Input(inputConfigure, rawInput, true, _inputFlag, inputPower);
                        }
                    }
                    else
                    {
                        if (_rawInputs.Remove(rawInput))
                        {
                            mainViewModel.Input(inputConfigure, rawInput, false, _inputFlag, inputPower);
                        }
                    }
                    break;
                case MainViewModel.Mode.Quit:
                    if (isInput)
                    {
                        switch (input)
                        {
                            case InputStandardControllerViewModel.LevyNoteFile:
                                mainViewModel.HandleEnter();
                                break;
                        }
                    }
                    break;
            }
        }

        public void HandleLooping(bool hasControllers, Action onHandle1000, Action onHandle)
        {
            var millisPolling = 1000.0 / (hasControllers ? Configure.Instance.LoopUnit : 1);
            _atLoopingCounter1000 += millisPolling;
            if (_atLoopingCounter1000 >= 1000.0)
            {
                _atLoopingCounter1000 -= 1000.0;
                Task.Run(onHandle1000);
            }

            if (hasControllers)
            {
                onHandle();
            }

            _loopingCounter += millisPolling;
            var toWait = _loopingCounter - _loopingHandler.GetMillis();
            if (toWait > 0.0)
            {
                Thread.Sleep(TimeSpan.FromMilliseconds(toWait));
            }
        }
    }
}