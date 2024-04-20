using CommunityToolkit.Mvvm.Input;
using Qwilight.Utilities;
using System.Windows;
using System.Windows.Media;
using Vortice.XInput;
using Windows.System;

namespace Qwilight.ViewModel
{
    public sealed partial class InputStandardControllerViewModel : BaseViewModel
    {
        public const int LowerEntry = 0;
        public const int HigherEntry = 1;
        public const int LowerNoteFile = 2;
        public const int HigherNoteFile = 3;
        public const int LevyNoteFile = 4;
        public const int Wait = 5;
        public const int LowerMultiplier = 6;
        public const int HigherMultiplier = 7;
        public const int HalfMultiplier = 8;
        public const int _2XMultiplier = 9;
        public const int LowerAudioMultiplier = 10;
        public const int HigherAudioMultiplier = 11;
        public const int MediaMode = 12;
        public const int VeilDrawing = 13;
        public const int HandleUndo = 14;
        public const int ModifyAutoMode = 15;
        public const int PostItem0 = 16;
        public const int PostItem1 = 17;

        public enum ControllerMode
        {
            DInput, XInput, WGI, MIDI
        }

        int _inputPosition;

        public override double TargetHeight => double.NaN;

        public Brush[] InputPaints { get; } = new Brush[19];

        public string[] Inputs { get; } = new string[19];

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        [RelayCommand]
        void OnInputPosition(int? inputPosition)
        {
            if (inputPosition.HasValue)
            {
                SetInputPaint(_inputPosition, false);
                _inputPosition = inputPosition.Value;
                SetInputPaint(_inputPosition, true);
            }
        }

        public void OnDefaultInputLower(VirtualKey e)
        {
            switch (e)
            {
                case VirtualKey.Left:
                    SetInputPaint(_inputPosition, false);
                    if (_inputPosition == 0)
                    {
                        Close();
                    }
                    else
                    {
                        SetInputPaint(--_inputPosition, true);
                    }
                    break;
                case VirtualKey.Right:
                    SetInputPaint(_inputPosition, false);
                    if (_inputPosition == 17)
                    {
                        Close();
                    }
                    else
                    {
                        SetInputPaint(++_inputPosition, true);
                    }
                    break;
                case VirtualKey.Back:
                case VirtualKey.Delete:
                    switch (ControllerModeValue)
                    {
                        case ControllerMode.DInput:
                            Configure.Instance.DInputBundlesV4.StandardInputs[_inputPosition].Data = default;
                            SetInput(_inputPosition, Configure.Instance.DInputBundlesV4.StandardInputs[_inputPosition]);
                            break;
                        case ControllerMode.XInput:
                            Configure.Instance.XInputBundlesV4.StandardInputs[_inputPosition].Data = new Gamepad
                            {
                                Buttons = GamepadButtons.None
                            };
                            SetInput(_inputPosition, Configure.Instance.XInputBundlesV4.StandardInputs[_inputPosition]);
                            break;
                        case ControllerMode.WGI:
                            Configure.Instance.WGIBundlesV3.StandardInputs[_inputPosition].Data = default;
                            SetInput(_inputPosition, Configure.Instance.WGIBundlesV3.StandardInputs[_inputPosition]);
                            break;
                        case ControllerMode.MIDI:
                            Configure.Instance.MIDIBundlesV4.StandardInputs[_inputPosition].Value = default;
                            SetInput(_inputPosition, Configure.Instance.MIDIBundlesV4.StandardInputs[_inputPosition]);
                            break;
                    }
                    break;
            }
        }

        public void OnDInputLower(HwDInput e)
        {
            if (Utility.AllowInput(Configure.Instance.DInputBundlesV4, e))
            {
                Configure.Instance.DInputBundlesV4.StandardInputs[_inputPosition] = e;
                SetInput(_inputPosition, Configure.Instance.DInputBundlesV4.StandardInputs[_inputPosition]);
            }
        }

        public void OnXInputLower(HwXInput e)
        {
            if (Utility.AllowInput(Configure.Instance.XInputBundlesV4, e))
            {
                Configure.Instance.XInputBundlesV4.StandardInputs[_inputPosition] = e;
                SetInput(_inputPosition, Configure.Instance.XInputBundlesV4.StandardInputs[_inputPosition]);
            }
        }

        public void OnWGILower(WGI e)
        {
            if (Utility.AllowInput(Configure.Instance.WGIBundlesV3, e))
            {
                Configure.Instance.WGIBundlesV3.StandardInputs[_inputPosition] = e;
                SetInput(_inputPosition, Configure.Instance.WGIBundlesV3.StandardInputs[_inputPosition]);
            }
        }

        public void OnMIDILower(MIDI e)
        {
            if (Utility.AllowInput(Configure.Instance.MIDIBundlesV4, e))
            {
                Configure.Instance.MIDIBundlesV4.StandardInputs[_inputPosition] = e;
                SetInput(_inputPosition, Configure.Instance.MIDIBundlesV4.StandardInputs[_inputPosition]);
            }
        }

        public ControllerMode ControllerModeValue { get; set; }

        public void SetInput<T>(int inputPosition, T toInput)
        {
            var input = toInput.ToString();
            if (string.IsNullOrEmpty(input))
            {
                input = "❌";
            }
            Inputs[inputPosition + 1] = string.Format(inputPosition switch
            {
                LowerEntry => LanguageSystem.Instance.LowerEntryContents,
                HigherEntry => LanguageSystem.Instance.HigherEntryContents,
                LowerNoteFile => LanguageSystem.Instance.LowerNoteFileContents,
                HigherNoteFile => LanguageSystem.Instance.HigherNoteFileContents,
                LevyNoteFile => LanguageSystem.Instance.LevyNoteFileContents,
                Wait => LanguageSystem.Instance.WaitContents,
                LowerMultiplier => LanguageSystem.Instance.LowerMultiplierContents,
                HigherMultiplier => LanguageSystem.Instance.HigherMultiplierContents,
                ModifyAutoMode => LanguageSystem.Instance.ModifyAutoModeContents,
                HandleUndo => LanguageSystem.Instance.HandleUndoContents,
                MediaMode => "BGA ({0})",
                LowerAudioMultiplier => LanguageSystem.Instance.LowerAudioMultiplierContents,
                HigherAudioMultiplier => LanguageSystem.Instance.HigherAudioMultiplierContents,
                PostItem0 => LanguageSystem.Instance.PostItem0Contents,
                PostItem1 => LanguageSystem.Instance.PostItem1Contents,
                VeilDrawing => LanguageSystem.Instance.VeilDrawingContents,
                HalfMultiplier => LanguageSystem.Instance.HalfMultiplierContents,
                _2XMultiplier => LanguageSystem.Instance._2XMultiplierContents,
                _ => default
            }, input);
            OnPropertyChanged(nameof(Inputs));
        }

        public void SetInputPaint(int inputPosition, bool isInput)
        {
            InputPaints[inputPosition + 1] = isInput ? Paints.PointPaints[1] : Brushes.Transparent;
            OnPropertyChanged(nameof(InputPaints));
        }

        public override void OnOpened()
        {
            base.OnOpened();
            switch (ControllerModeValue)
            {
                case ControllerMode.DInput:
                    for (var i = Configure.Instance.DInputBundlesV4.StandardInputs.Length - 1; i >= 0; --i)
                    {
                        SetInputPaint(i, false);
                        SetInput(i, Configure.Instance.DInputBundlesV4.StandardInputs[i]);
                    }
                    break;
                case ControllerMode.XInput:
                    for (var i = Configure.Instance.XInputBundlesV4.StandardInputs.Length - 1; i >= 0; --i)
                    {
                        SetInputPaint(i, false);
                        SetInput(i, Configure.Instance.XInputBundlesV4.StandardInputs[i]);
                    }
                    break;
                case ControllerMode.WGI:
                    for (var i = Configure.Instance.WGIBundlesV3.StandardInputs.Length - 1; i >= 0; --i)
                    {
                        SetInputPaint(i, false);
                        SetInput(i, Configure.Instance.WGIBundlesV3.StandardInputs[i]);
                    }
                    break;
                case ControllerMode.MIDI:
                    for (var i = Configure.Instance.MIDIBundlesV4.StandardInputs.Length - 1; i >= 0; --i)
                    {
                        SetInputPaint(i, false);
                        SetInput(i, Configure.Instance.MIDIBundlesV4.StandardInputs[i]);
                    }
                    break;
            }
            _inputPosition = 0;
            SetInputPaint(_inputPosition, true);
        }
    }
}