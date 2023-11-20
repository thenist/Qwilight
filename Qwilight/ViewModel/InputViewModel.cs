using CommunityToolkit.Mvvm.Input;
using Qwilight.Utilities;
using System.Windows;
using System.Windows.Media;
using Vortice.XInput;
using Windows.Devices.Midi;
using Windows.System;
using GamepadReading = Windows.Gaming.Input.GamepadReading;

namespace Qwilight.ViewModel
{
    public sealed partial class InputViewModel : BaseViewModel
    {
        public enum ControllerMode
        {
            DefaultInput, DInput, XInput, WGI, MIDI
        }

        readonly int[][] _inputMap = new int[17][];
        readonly int[][] _pageMap = new int[17][];
        readonly int[][][] _inputPositionMap = new int[17][][];
        ControllerMode _valueControllerMode;
        Component.InputMode _inputMode;
        int _inputPosition;
        int _page;
        int _endPage;
        bool _allowEssentialsInputs;

        public override double TargetHeight => double.NaN;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public TextDecorationCollection BMSFont { get; set; }

        public ControllerMode ControllerModeValue
        {
            get => _valueControllerMode;

            set => SetProperty(ref _valueControllerMode, value, nameof(ControllerModeValue));
        }

        public bool IsVisibleAllowEssentialsInput => ControllerModeValue == ControllerMode.DefaultInput;

        public Component.InputMode InputMode
        {
            get => _inputMode;

            set
            {
                if (SetProperty(ref _inputMode, value, nameof(InputMode)))
                {
                    BMSFont = new[] { Component.InputMode._4, Component.InputMode._6, Component.InputMode._9, Component.InputMode._5_1, Component.InputMode._7_1, Component.InputMode._10_2, Component.InputMode._14_2 }.Contains(InputMode) ? null : TextDecorations.Strikethrough;
                    OnPropertyChanged(nameof(BMSFont));
                }
            }
        }

        public bool AllowEssentialInputs
        {
            get => _allowEssentialsInputs;

            set => SetProperty(ref _allowEssentialsInputs, value, nameof(AllowEssentialInputs));
        }

        public string[] Inputs { get; } = new string[11];

        public Brush[] InputPaints { get; } = new Brush[11];

        public Brush[] InputNotePaints { get; } = new Brush[11];

        [RelayCommand]
        public void OnInputPosition(int? inputPosition)
        {
            if (inputPosition.HasValue)
            {
                switch (ControllerModeValue)
                {
                    case ControllerMode.DefaultInput:
                        InputImpl(Configure.Instance.DefaultInputBundlesV6.Inputs);
                        break;
                    case ControllerMode.DInput:
                        InputImpl(Configure.Instance.DInputBundlesV4.Inputs);
                        break;
                    case ControllerMode.XInput:
                        InputImpl(Configure.Instance.XInputBundlesV4.Inputs);
                        break;
                    case ControllerMode.WGI:
                        InputImpl(Configure.Instance.WGIBundlesV3.Inputs);
                        break;
                    case ControllerMode.MIDI:
                        InputImpl(Configure.Instance.MIDIBundlesV4.Inputs);
                        break;
                }

                void InputImpl<T>(T[][][] inputConfigure)
                {
                    var inputPositionValue = _inputPositionMap[(int)_inputMode][_page][inputPosition.Value];
                    SetInputPaint(_inputMap[(int)_inputMode][_inputPosition], false);
                    SetInputPaint(_inputMap[(int)_inputMode][inputPositionValue], true);
                    _inputPosition = inputPositionValue;
                }
            }
        }

        public void OnDefaultInputLower(VirtualKey e)
        {
            switch (e)
            {
                case VirtualKey.Left:
                    if (AllowEssentialInputs)
                    {
                        SetInputImpl();
                    }
                    else
                    {
                        switch (ControllerModeValue)
                        {
                            case ControllerMode.DefaultInput:
                                InputImpl(Configure.Instance.DefaultInputBundlesV6.Inputs);
                                break;
                            case ControllerMode.DInput:
                                InputImpl(Configure.Instance.DInputBundlesV4.Inputs);
                                break;
                            case ControllerMode.XInput:
                                InputImpl(Configure.Instance.XInputBundlesV4.Inputs);
                                break;
                            case ControllerMode.WGI:
                                InputImpl(Configure.Instance.WGIBundlesV3.Inputs);
                                break;
                            case ControllerMode.MIDI:
                                InputImpl(Configure.Instance.MIDIBundlesV4.Inputs);
                                break;
                        }
                        void InputImpl<T>(T[][][] inputConfigure)
                        {
                            SetInputPaint(_inputMap[(int)_inputMode][_inputPosition], false);
                            if (_inputPosition - 1 <= (_page > 0 ? _pageMap[(int)_inputMode][_page - 1] : 0))
                            {
                                if (--_page >= 0)
                                {
                                    --_inputPosition;
                                    SetNoteInputPaint();
                                    for (var i = 10; i > 0; --i)
                                    {
                                        SetInput<object>(i);
                                        SetInputPaint(i, false);
                                    }
                                    SetInputs(Configure.Instance.DefaultInputBundlesV6.Inputs);
                                    SetInputPaint(_inputMap[(int)_inputMode][_inputPosition], true);
                                }
                                else
                                {
                                    Close();
                                }
                            }
                            else
                            {
                                SetInputPaint(_inputMap[(int)_inputMode][--_inputPosition], true);
                            }
                        }
                    }
                    break;
                case VirtualKey.Right:
                    if (AllowEssentialInputs)
                    {
                        SetInputImpl();
                    }
                    else
                    {
                        switch (ControllerModeValue)
                        {
                            case ControllerMode.DefaultInput:
                                InputImpl(Configure.Instance.DefaultInputBundlesV6.Inputs);
                                break;
                            case ControllerMode.DInput:
                                InputImpl(Configure.Instance.DInputBundlesV4.Inputs);
                                break;
                            case ControllerMode.XInput:
                                InputImpl(Configure.Instance.XInputBundlesV4.Inputs);
                                break;
                            case ControllerMode.WGI:
                                InputImpl(Configure.Instance.WGIBundlesV3.Inputs);
                                break;
                            case ControllerMode.MIDI:
                                InputImpl(Configure.Instance.MIDIBundlesV4.Inputs);
                                break;
                        }
                        void InputImpl<T>(T[][][] inputConfigure)
                        {
                            SetInputPaint(_inputMap[(int)_inputMode][_inputPosition], false);
                            if (_inputPosition + 1 > _pageMap[(int)_inputMode][_page])
                            {
                                if (++_page < _endPage)
                                {
                                    ++_inputPosition;
                                    SetNoteInputPaint();
                                    for (var i = 10; i > 0; --i)
                                    {
                                        SetInput<object>(i);
                                        SetInputPaint(i, false);
                                    }
                                    SetInputs(inputConfigure);
                                    SetInputPaint(_inputMap[(int)_inputMode][_inputPosition], true);
                                }
                                else
                                {
                                    Close();
                                }
                            }
                            else
                            {
                                SetInputPaint(_inputMap[(int)_inputMode][++_inputPosition], true);
                            }
                        }
                    }
                    break;
                case VirtualKey.Back:
                    if (AllowEssentialInputs)
                    {
                        SetInputImpl();
                    }
                    else
                    {
                        switch (ControllerModeValue)
                        {
                            case ControllerMode.DefaultInput:
                                InputImpl(Configure.Instance.DefaultInputBundlesV6.Inputs, input => input.Data != VirtualKey.None, i => Configure.Instance.DefaultInputBundlesV6.Inputs[(int)_inputMode][_inputPosition][i].Data = VirtualKey.None);
                                break;
                            case ControllerMode.DInput:
                                InputImpl(Configure.Instance.DInputBundlesV4.Inputs, input => input.Data != default, i => Configure.Instance.DInputBundlesV4.Inputs[(int)_inputMode][_inputPosition][i].Data = default);
                                break;
                            case ControllerMode.XInput:
                                InputImpl(Configure.Instance.XInputBundlesV4.Inputs, input => !input.Data.Equals(default(Gamepad)), i => Configure.Instance.XInputBundlesV4.Inputs[(int)_inputMode][_inputPosition][i].Data = default);
                                break;
                            case ControllerMode.WGI:
                                InputImpl(Configure.Instance.WGIBundlesV3.Inputs, input => !input.Data.Equals(default(GamepadReading)), i => Configure.Instance.WGIBundlesV3.Inputs[(int)_inputMode][_inputPosition][i].Data = default);
                                break;
                            case ControllerMode.MIDI:
                                InputImpl(Configure.Instance.MIDIBundlesV4.Inputs, input => input.Data != MidiMessageType.None, i => Configure.Instance.MIDIBundlesV4.Inputs[(int)_inputMode][_inputPosition][i].Data = MidiMessageType.None);
                                break;
                        }
                        void InputImpl<T>(T[][][] inputConfigure, Func<T, bool> isOK, Action<int> onHandle)
                        {
                            for (var i = inputConfigure[(int)_inputMode][_inputPosition].Length - 1; i >= 0; --i)
                            {
                                if (isOK(inputConfigure[(int)_inputMode][_inputPosition][i]))
                                {
                                    onHandle(i);
                                    SetInput(_inputMap[(int)_inputMode][_inputPosition], inputConfigure[(int)_inputMode][_inputPosition]);
                                    break;
                                }
                            }
                        }
                    }
                    break;
                case VirtualKey.Delete:
                    if (AllowEssentialInputs)
                    {
                        SetInputImpl();
                    }
                    else
                    {
                        switch (ControllerModeValue)
                        {
                            case ControllerMode.DefaultInput:
                                InputImpl(Configure.Instance.DefaultInputBundlesV6.Inputs, i => Configure.Instance.DefaultInputBundlesV6.Inputs[(int)_inputMode][_inputPosition][i].Data = VirtualKey.None);
                                break;
                            case ControllerMode.DInput:
                                InputImpl(Configure.Instance.DInputBundlesV4.Inputs, i => Configure.Instance.DInputBundlesV4.Inputs[(int)_inputMode][_inputPosition][i].Data = default);
                                break;
                            case ControllerMode.XInput:
                                InputImpl(Configure.Instance.XInputBundlesV4.Inputs, i => Configure.Instance.XInputBundlesV4.Inputs[(int)_inputMode][_inputPosition][i].Data = default);
                                break;
                            case ControllerMode.WGI:
                                InputImpl(Configure.Instance.WGIBundlesV3.Inputs, i => Configure.Instance.WGIBundlesV3.Inputs[(int)_inputMode][_inputPosition][i].Data = default);
                                break;
                            case ControllerMode.MIDI:
                                InputImpl(Configure.Instance.MIDIBundlesV4.Inputs, i => Configure.Instance.MIDIBundlesV4.Inputs[(int)_inputMode][_inputPosition][i].Data = MidiMessageType.None);
                                break;
                        }
                        void InputImpl<T>(T[][][] inputConfigure, Action<int> onHandle)
                        {
                            for (var i = inputConfigure[(int)_inputMode][_inputPosition].Length - 1; i >= 0; --i)
                            {
                                onHandle(i);
                            }
                            SetInput(_inputMap[(int)_inputMode][_inputPosition], inputConfigure[(int)_inputMode][_inputPosition]);
                        }
                    }
                    break;
                case VirtualKey.F1:
                case VirtualKey.F2:
                case VirtualKey.F3:
                case VirtualKey.F4:
                case VirtualKey.A:
                case VirtualKey.B:
                case VirtualKey.C:
                case VirtualKey.D:
                case VirtualKey.E:
                case VirtualKey.F:
                case VirtualKey.G:
                case VirtualKey.H:
                case VirtualKey.I:
                case VirtualKey.J:
                case VirtualKey.K:
                case VirtualKey.L:
                case VirtualKey.M:
                case VirtualKey.N:
                case VirtualKey.O:
                case VirtualKey.P:
                case VirtualKey.Q:
                case VirtualKey.R:
                case VirtualKey.S:
                case VirtualKey.T:
                case VirtualKey.U:
                case VirtualKey.V:
                case VirtualKey.W:
                case VirtualKey.X:
                case VirtualKey.Y:
                case VirtualKey.Z:
                case VirtualKey.Number0:
                case VirtualKey.Number1:
                case VirtualKey.Number2:
                case VirtualKey.Number3:
                case VirtualKey.Number4:
                case VirtualKey.Number5:
                case VirtualKey.Number6:
                case VirtualKey.Number7:
                case VirtualKey.Number8:
                case VirtualKey.Number9:
                case VirtualKey.Space:
                case (VirtualKey)188:
                case (VirtualKey)190:
                case (VirtualKey)191:
                case (VirtualKey)186:
                case (VirtualKey)192:
                case (VirtualKey)219:
                case (VirtualKey)221:
                case (VirtualKey)220:
                case VirtualKey.LeftControl:
                case VirtualKey.Control:
                case VirtualKey.LeftMenu:
                case VirtualKey.Menu:
                case VirtualKey.LeftShift:
                case VirtualKey.RightShift:
                case VirtualKey.RightControl:
                case VirtualKey.Kanji:
                case VirtualKey.RightMenu:
                case VirtualKey.Hangul:
                case (VirtualKey)222:
                case (VirtualKey)187:
                case (VirtualKey)189:
                case VirtualKey.CapitalLock:
                case VirtualKey.Insert:
                case VirtualKey.Home:
                case VirtualKey.End:
                case VirtualKey.PageUp:
                case VirtualKey.PageDown:
                case VirtualKey.NumberKeyLock:
                case VirtualKey.Add:
                case VirtualKey.Subtract:
                case VirtualKey.Multiply:
                case VirtualKey.Divide:
                case VirtualKey.Decimal:
                case VirtualKey.NumberPad0:
                case VirtualKey.NumberPad1:
                case VirtualKey.NumberPad2:
                case VirtualKey.NumberPad3:
                case VirtualKey.NumberPad4:
                case VirtualKey.NumberPad5:
                case VirtualKey.NumberPad6:
                case VirtualKey.NumberPad7:
                case VirtualKey.NumberPad8:
                case VirtualKey.NumberPad9:
                case VirtualKey.Tab:
                case VirtualKey.Up:
                case VirtualKey.Down:
                    SetInputImpl();
                    break;
            }
            void SetInputImpl()
            {
                if (ControllerModeValue == ControllerMode.DefaultInput)
                {
                    var defaultInput = new DefaultInput
                    {
                        Data = e
                    };
                    if (Utility.AllowInput(Configure.Instance.DefaultInputBundlesV6, defaultInput, _inputMode))
                    {
                        for (var i = 0; i < Configure.Instance.DefaultInputBundlesV6.Inputs[(int)_inputMode][_inputPosition].Length; ++i)
                        {
                            if (Configure.Instance.DefaultInputBundlesV6.Inputs[(int)_inputMode][_inputPosition][i].Data == VirtualKey.None)
                            {
                                Configure.Instance.DefaultInputBundlesV6.Inputs[(int)_inputMode][_inputPosition][i] = defaultInput;
                                break;
                            }
                        }
                    }
                    SetInputs(Configure.Instance.DefaultInputBundlesV6.Inputs);
                }
            }
        }

        public void OnDInputLower(HwDInput e)
        {
            if (Utility.AllowInput(Configure.Instance.DInputBundlesV4, e, _inputMode))
            {
                for (var i = 0; i < Configure.Instance.DInputBundlesV4.Inputs[(int)_inputMode][_inputPosition].Length; ++i)
                {
                    if (Configure.Instance.DInputBundlesV4.Inputs[(int)_inputMode][_inputPosition][i].Data == default)
                    {
                        Configure.Instance.DInputBundlesV4.Inputs[(int)_inputMode][_inputPosition][i] = e;
                        break;
                    }
                }
            }
            SetInputs(Configure.Instance.DInputBundlesV4.Inputs);
        }

        public void OnXInputLower(HwXInput e)
        {
            if (Utility.AllowInput(Configure.Instance.XInputBundlesV4, e, _inputMode))
            {
                for (var i = 0; i < Configure.Instance.XInputBundlesV4.Inputs[(int)_inputMode][_inputPosition].Length; ++i)
                {
                    if (Configure.Instance.XInputBundlesV4.Inputs[(int)_inputMode][_inputPosition][i].Data.Equals(default(Gamepad)))
                    {
                        Configure.Instance.XInputBundlesV4.Inputs[(int)_inputMode][_inputPosition][i] = e;
                        break;
                    }
                }
            }
            SetInputs(Configure.Instance.XInputBundlesV4.Inputs);
        }

        public void OnWGILower(WGI e)
        {
            if (Utility.AllowInput(Configure.Instance.WGIBundlesV3, e, _inputMode))
            {
                for (var i = 0; i < Configure.Instance.WGIBundlesV3.Inputs[(int)_inputMode][_inputPosition].Length; ++i)
                {
                    if (Configure.Instance.WGIBundlesV3.Inputs[(int)_inputMode][_inputPosition][i].Data.Equals(default(GamepadReading)))
                    {
                        Configure.Instance.WGIBundlesV3.Inputs[(int)_inputMode][_inputPosition][i] = e;
                        break;
                    }
                }
            }
            SetInputs(Configure.Instance.WGIBundlesV3.Inputs);
        }

        public void OnMIDILower(MIDI e)
        {
            if (Utility.AllowInput(Configure.Instance.MIDIBundlesV4, e, _inputMode))
            {
                for (var i = 0; i < Configure.Instance.MIDIBundlesV4.Inputs[(int)_inputMode][_inputPosition].Length; ++i)
                {
                    if (Configure.Instance.MIDIBundlesV4.Inputs[(int)_inputMode][_inputPosition][i].Data == MidiMessageType.None)
                    {
                        Configure.Instance.MIDIBundlesV4.Inputs[(int)_inputMode][_inputPosition][i] = e;
                        break;
                    }
                }
            }
            SetInputs(Configure.Instance.MIDIBundlesV4.Inputs);
        }

        public InputViewModel()
        {
            _inputMap[(int)Component.InputMode._4] = new[] { default, 4, 5, 6, 7 };
            _inputMap[(int)Component.InputMode._5] = new[] { default, 4, 5, 6, 7, 8 };
            _inputMap[(int)Component.InputMode._6] = new[] { default, 3, 4, 5, 6, 7, 8 };
            _inputMap[(int)Component.InputMode._7] = new[] { default, 3, 4, 5, 6, 7, 8, 9 };
            _inputMap[(int)Component.InputMode._8] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9 };
            _inputMap[(int)Component.InputMode._9] = new[] { default, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            _inputMap[(int)Component.InputMode._10] = new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 };
            _inputMap[(int)Component.InputMode._5_1] = new[] { default, 1, 4, 5, 6, 7, 8 };
            _inputMap[(int)Component.InputMode._7_1] = new[] { default, 1, 3, 4, 5, 6, 7, 8, 9 };
            _inputMap[(int)Component.InputMode._10_2] = new[] { default,
                1, 4, 5, 6, 7, 8,
                3, 4, 5, 6, 7, 10
            };
            _inputMap[(int)Component.InputMode._14_2] = new[] { default,
                1, 3, 4, 5, 6, 7, 8, 9,
                2, 3, 4, 5, 6, 7, 8, 10
            };
            _inputMap[(int)Component.InputMode._24_2] = new[] { default,
                1, 4, 5, 6, 7, 8,
                3, 4, 5, 6, 7, 8, 9,
                4, 5, 6, 7, 8, 2, 3,
                4, 5, 6, 7, 8, 10
            };
            _inputMap[(int)Component.InputMode._48_4] = new[] { default,
                1, 2, 4, 5, 6, 7, 8,
                3, 4, 5, 6, 7, 8, 9,
                4, 5, 6, 7, 8,
                3, 4, 5, 6, 7, 8, 9,
                4, 5, 6, 7, 8,
                3, 4, 5, 6, 7, 8, 9,
                4, 5, 6, 7, 8,
                2, 3, 4, 5, 6, 7, 8, 9, 10
            };
            _pageMap[(int)Component.InputMode._4] = new[] { 4 };
            _pageMap[(int)Component.InputMode._5] = new[] { 5 };
            _pageMap[(int)Component.InputMode._6] = new[] { 6 };
            _pageMap[(int)Component.InputMode._7] = new[] { 7 };
            _pageMap[(int)Component.InputMode._8] = new[] { 8 };
            _pageMap[(int)Component.InputMode._9] = new[] { 9 };
            _pageMap[(int)Component.InputMode._10] = new[] { 10 };
            _pageMap[(int)Component.InputMode._5_1] = new[] { 6 };
            _pageMap[(int)Component.InputMode._7_1] = new[] { 8 };
            _pageMap[(int)Component.InputMode._10_2] = new[] { 6, 12 };
            _pageMap[(int)Component.InputMode._14_2] = new[] { 8, 16 };
            _pageMap[(int)Component.InputMode._24_2] = new[] { 6, 13, 18, 26 };
            _pageMap[(int)Component.InputMode._48_4] = new[] { 7, 14, 19, 26, 31, 38, 43, 52 };
            _inputPositionMap[(int)Component.InputMode._4] = new[] { new[] { default, default, default, default, 1, 2, 3, 4, default, default, default } };
            _inputPositionMap[(int)Component.InputMode._5] = new[] { new[] { default, default, default, default, 1, 2, 3, 4, 5, default, default } };
            _inputPositionMap[(int)Component.InputMode._6] = new[] { new[] { default, default, default, 1, 2, 3, 4, 5, 6, default, default } };
            _inputPositionMap[(int)Component.InputMode._7] = new[] { new[] { default, default, default, 1, 2, 3, 4, 5, 6, 7, default } };
            _inputPositionMap[(int)Component.InputMode._8] = new[] { new[] { default, default, 1, 2, 3, 4, 5, 6, 7, 8, default } };
            _inputPositionMap[(int)Component.InputMode._9] = new[] { new[] { default, default, 1, 2, 3, 4, 5, 6, 7, 8, 9 } };
            _inputPositionMap[(int)Component.InputMode._10] = new[] { new[] { default, 1, 2, 3, 4, 5, 6, 7, 8, 9, 10 } };
            _inputPositionMap[(int)Component.InputMode._5_1] = new[] { new[] { default, 1, default, default, 2, 3, 4, 5, 6, default, default } };
            _inputPositionMap[(int)Component.InputMode._7_1] = new[] { new[] { default, 1, default, 2, 3, 4, 5, 6, 7, 8, default } };
            _inputPositionMap[(int)Component.InputMode._10_2] = new[] {
                new[] { default, 1, default, default, 2, 3, 4, 5, 6, default, default },
                new[] { default, default, default, 7, 8, 9, 10, 11, default, default, 12 }
            };
            _inputPositionMap[(int)Component.InputMode._14_2] = new[] {
                new[] { default, 1, default, 2, 3, 4, 5, 6, 7, 8, default },
                new[] { default, default, 9, 10, 11, 12, 13, 14, 15, default, 16 }
            };
            _inputPositionMap[(int)Component.InputMode._24_2] = new[] {
                new[] { default, 1, default, default, 2, 3, 4, 5, 6, default, default, default },
                new[] { default, default, default, 7, 8, 9, 10, 11, 12, 13, default, default },
                new[] { default, default, default, default, 14, 15, 16, 17, 18, default, default },
                new[] { default, default, 19, 20, 21, 22, 23, 24, 25, default, 26 }
            };
            _inputPositionMap[(int)Component.InputMode._48_4] = new[] {
                new[] { default, 1, 2, default, 3, 4, 5, 6, 7, default, default },
                new[] { default, default, default, 8, 9, 10, 11, 12, 13, 14, default },
                new[] { default, default, default, default, 15, 16, 17, 18, 19, default, default },
                new[] { default, default, default, 20, 21, 22, 23, 24, 25, 26, default },
                new[] { default, default, default, default, 27, 28, 29, 30, 31, default, default },
                new[] { default, default, default, 32, 33, 34, 35, 36, 37, 38, default},
                new[] { default, default, default, default, 39, 40, 41, 42, 43, default, default },
                new[] { default, default, 44, 45, 46, 47, 48, 49, 50, 51, 52 }
            };
        }

        void SetInputs<T>(T[][][] inputConfigure)
        {
            for (var i = (_page > 0 ? _pageMap[(int)_inputMode][_page - 1] : 0) + 1; i <= _pageMap[(int)_inputMode][_page]; ++i)
            {
                SetInput(_inputMap[(int)_inputMode][i], inputConfigure[(int)_inputMode][i]);
            }
        }

        void SetNoteInputPaint()
        {
            Array.Fill(InputNotePaints, null);
            switch (InputMode)
            {
                case Component.InputMode._4:
                    InputNotePaints[4] = Paints.Paint4;
                    InputNotePaints[5] = Brushes.Cyan;
                    InputNotePaints[6] = Brushes.Cyan;
                    InputNotePaints[7] = Paints.Paint4;
                    break;
                case Component.InputMode._5:
                    InputNotePaints[4] = Paints.Paint4;
                    InputNotePaints[5] = Brushes.Cyan;
                    InputNotePaints[6] = Paints.Paint4;
                    InputNotePaints[7] = Brushes.Cyan;
                    InputNotePaints[8] = Paints.Paint4;
                    break;
                case Component.InputMode._6:
                    InputNotePaints[3] = Paints.Paint4;
                    InputNotePaints[4] = Brushes.Cyan;
                    InputNotePaints[5] = Paints.Paint4;
                    InputNotePaints[6] = Paints.Paint4;
                    InputNotePaints[7] = Brushes.Cyan;
                    InputNotePaints[8] = Paints.Paint4;
                    break;
                case Component.InputMode._7:
                    InputNotePaints[3] = Paints.Paint4;
                    InputNotePaints[4] = Brushes.Cyan;
                    InputNotePaints[5] = Paints.Paint4;
                    InputNotePaints[6] = Brushes.Cyan;
                    InputNotePaints[7] = Paints.Paint4;
                    InputNotePaints[8] = Brushes.Cyan;
                    InputNotePaints[9] = Paints.Paint4;
                    break;
                case Component.InputMode._8:
                    InputNotePaints[2] = Paints.Paint4;
                    InputNotePaints[3] = Brushes.Cyan;
                    InputNotePaints[4] = Paints.Paint4;
                    InputNotePaints[5] = Brushes.Cyan;
                    InputNotePaints[6] = Brushes.Cyan;
                    InputNotePaints[7] = Paints.Paint4;
                    InputNotePaints[8] = Brushes.Cyan;
                    InputNotePaints[9] = Paints.Paint4;
                    break;
                case Component.InputMode._9:
                    InputNotePaints[2] = Paints.Paint4;
                    InputNotePaints[3] = Brushes.Cyan;
                    InputNotePaints[4] = Paints.Paint4;
                    InputNotePaints[5] = Brushes.Cyan;
                    InputNotePaints[6] = Paints.Paint4;
                    InputNotePaints[7] = Brushes.Cyan;
                    InputNotePaints[8] = Paints.Paint4;
                    InputNotePaints[9] = Brushes.Cyan;
                    InputNotePaints[10] = Paints.Paint4;
                    break;
                case Component.InputMode._10:
                    InputNotePaints[1] = Paints.Paint4;
                    InputNotePaints[2] = Brushes.Cyan;
                    InputNotePaints[3] = Paints.Paint4;
                    InputNotePaints[4] = Brushes.Cyan;
                    InputNotePaints[5] = Paints.Paint4;
                    InputNotePaints[6] = Paints.Paint4;
                    InputNotePaints[7] = Brushes.Cyan;
                    InputNotePaints[8] = Paints.Paint4;
                    InputNotePaints[9] = Brushes.Cyan;
                    InputNotePaints[10] = Paints.Paint4;
                    break;
                case Component.InputMode._5_1:
                    InputNotePaints[1] = Paints.Paint1;
                    InputNotePaints[4] = Paints.Paint4;
                    InputNotePaints[5] = Brushes.Cyan;
                    InputNotePaints[6] = Paints.Paint4;
                    InputNotePaints[7] = Brushes.Cyan;
                    InputNotePaints[8] = Paints.Paint4;
                    break;
                case Component.InputMode._7_1:
                    InputNotePaints[1] = Paints.Paint1;
                    InputNotePaints[3] = Paints.Paint4;
                    InputNotePaints[4] = Brushes.Cyan;
                    InputNotePaints[5] = Paints.Paint4;
                    InputNotePaints[6] = Brushes.Cyan;
                    InputNotePaints[7] = Paints.Paint4;
                    InputNotePaints[8] = Brushes.Cyan;
                    InputNotePaints[9] = Paints.Paint4;
                    break;
                case Component.InputMode._10_2:
                    switch (_page)
                    {
                        case 0:
                            InputNotePaints[1] = Paints.Paint1;
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            break;
                        case 1:
                            InputNotePaints[3] = Paints.Paint4;
                            InputNotePaints[4] = Brushes.Cyan;
                            InputNotePaints[5] = Paints.Paint4;
                            InputNotePaints[6] = Brushes.Cyan;
                            InputNotePaints[7] = Paints.Paint4;
                            InputNotePaints[10] = Paints.Paint1;
                            break;
                    }
                    break;
                case Component.InputMode._14_2:
                    switch (_page)
                    {
                        case 0:
                            InputNotePaints[1] = Paints.Paint1;
                            InputNotePaints[3] = Paints.Paint4;
                            InputNotePaints[4] = Brushes.Cyan;
                            InputNotePaints[5] = Paints.Paint4;
                            InputNotePaints[6] = Brushes.Cyan;
                            InputNotePaints[7] = Paints.Paint4;
                            InputNotePaints[8] = Brushes.Cyan;
                            InputNotePaints[9] = Paints.Paint4;
                            break;
                        case 1:
                            InputNotePaints[2] = Paints.Paint4;
                            InputNotePaints[3] = Brushes.Cyan;
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            InputNotePaints[10] = Paints.Paint1;
                            break;
                    }
                    break;
                case Component.InputMode._24_2:
                    switch (_page)
                    {
                        case 0:
                            InputNotePaints[1] = Paints.Paint1;
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            break;
                        case 1:
                            InputNotePaints[3] = Paints.Paint4;
                            InputNotePaints[4] = Brushes.Cyan;
                            InputNotePaints[5] = Paints.Paint4;
                            InputNotePaints[6] = Brushes.Cyan;
                            InputNotePaints[7] = Paints.Paint4;
                            InputNotePaints[8] = Brushes.Cyan;
                            InputNotePaints[9] = Paints.Paint4;
                            break;
                        case 2:
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            break;
                        case 3:
                            InputNotePaints[2] = Paints.Paint4;
                            InputNotePaints[3] = Brushes.Cyan;
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            InputNotePaints[10] = Paints.Paint1;
                            break;
                    }
                    break;
                case Component.InputMode._48_4:
                    switch (_page)
                    {
                        case 0:
                            InputNotePaints[1] = Paints.Paint1;
                            InputNotePaints[2] = Paints.Paint1;
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            break;
                        case 1:
                            InputNotePaints[3] = Paints.Paint4;
                            InputNotePaints[4] = Brushes.Cyan;
                            InputNotePaints[5] = Paints.Paint4;
                            InputNotePaints[6] = Brushes.Cyan;
                            InputNotePaints[7] = Paints.Paint4;
                            InputNotePaints[8] = Brushes.Cyan;
                            InputNotePaints[9] = Paints.Paint4;
                            break;
                        case 2:
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            break;
                        case 3:
                            InputNotePaints[3] = Paints.Paint4;
                            InputNotePaints[4] = Brushes.Cyan;
                            InputNotePaints[5] = Paints.Paint4;
                            InputNotePaints[6] = Brushes.Cyan;
                            InputNotePaints[7] = Paints.Paint4;
                            InputNotePaints[8] = Brushes.Cyan;
                            InputNotePaints[9] = Paints.Paint4;
                            break;
                        case 4:
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            break;
                        case 5:
                            InputNotePaints[3] = Paints.Paint4;
                            InputNotePaints[4] = Brushes.Cyan;
                            InputNotePaints[5] = Paints.Paint4;
                            InputNotePaints[6] = Brushes.Cyan;
                            InputNotePaints[7] = Paints.Paint4;
                            InputNotePaints[8] = Brushes.Cyan;
                            InputNotePaints[9] = Paints.Paint4;
                            break;
                        case 6:
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            break;
                        case 7:
                            InputNotePaints[2] = Paints.Paint4;
                            InputNotePaints[3] = Brushes.Cyan;
                            InputNotePaints[4] = Paints.Paint4;
                            InputNotePaints[5] = Brushes.Cyan;
                            InputNotePaints[6] = Paints.Paint4;
                            InputNotePaints[7] = Brushes.Cyan;
                            InputNotePaints[8] = Paints.Paint4;
                            InputNotePaints[9] = Paints.Paint1;
                            InputNotePaints[10] = Paints.Paint1;
                            break;
                    }
                    break;
            }
            OnPropertyChanged(nameof(InputNotePaints));
        }

        public void SetInput<T>(int inputPosition, T[] toInputs = null)
        {
            var input = string.Join(", ", toInputs?.Where(toInput => !string.IsNullOrEmpty(toInput?.ToString())) ?? Array.Empty<T>());
            if (string.IsNullOrEmpty(input) && InputNotePaints[inputPosition] != null)
            {
                input = "❌";
            }
            Inputs[inputPosition] = input;
            OnPropertyChanged(nameof(Inputs));
        }

        public void SetInputPaint(int inputPosition, bool isInput)
        {
            InputPaints[inputPosition] = isInput ? Paints.PointPaints[1] : Brushes.Transparent;
            OnPropertyChanged(nameof(InputPaints));
        }

        public override void OnOpened()
        {
            base.OnOpened();
            AllowEssentialInputs = false;
            _inputPosition = 1;
            _page = 0;
            _endPage = _pageMap[(int)_inputMode].Length;
            SetNoteInputPaint();
            for (var i = 10; i > 0; --i)
            {
                SetInput<object>(i);
                SetInputPaint(i, false);
            }
            switch (ControllerModeValue)
            {
                case ControllerMode.DefaultInput:
                    InputImpl(Configure.Instance.DefaultInputBundlesV6.Inputs);
                    break;
                case ControllerMode.DInput:
                    InputImpl(Configure.Instance.DInputBundlesV4.Inputs);
                    break;
                case ControllerMode.XInput:
                    InputImpl(Configure.Instance.XInputBundlesV4.Inputs);
                    break;
                case ControllerMode.WGI:
                    InputImpl(Configure.Instance.WGIBundlesV3.Inputs);
                    break;
                case ControllerMode.MIDI:
                    InputImpl(Configure.Instance.MIDIBundlesV4.Inputs);
                    break;
            }
            void InputImpl<T>(T[][][] inputConfigure)
            {
                for (var i = Component.InputCounts[(int)_inputMode] / _endPage; i > 0; --i)
                {
                    SetInput(_inputMap[(int)_inputMode][i], inputConfigure[(int)_inputMode][i]);
                }
            }
            SetInputPaint(_inputMap[(int)_inputMode][1], true);
        }
    }
}