using CommunityToolkit.Mvvm.Input;
using Qwilight.Utilities;
using System.Windows;
using System.Windows.Media;
using Windows.System;

namespace Qwilight.ViewModel
{
    public sealed partial class InputStandardViewModel : BaseViewModel
    {
        public const int LowerMultiplier = 0;
        public const int HigherMultiplier = 1;
        public const int HalfMultiplier = 2;
        public const int _2XMultiplier = 3;
        public const int LowerAudioMultiplier = 4;
        public const int HigherAudioMultiplier = 5;
        public const int MediaMode = 6;
        public const int VeilDrawing = 7;
        public const int HandleUndo = 8;
        public const int ModifyAutoMode = 9;
        public const int PostItem0 = 10;
        public const int PostItem1 = 11;

        int _inputPosition;
        bool _allowEssentialInputs;

        public override double TargetHeight => double.NaN;

        public override VerticalAlignment HeightSystem => VerticalAlignment.Bottom;

        public Brush[] InputPaints { get; } = new Brush[13];

        public string[] Inputs { get; } = new string[13];

        public int CallingInputPosition { get; set; }

        public bool AllowEssentialInputs
        {
            get => _allowEssentialInputs;

            set => SetProperty(ref _allowEssentialInputs, value, nameof(AllowEssentialInputs));
        }

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
                    if (AllowEssentialInputs)
                    {
                        SetInputImpl();
                    }
                    else
                    {
                        SetInputPaint(_inputPosition, false);
                        if (_inputPosition == 0)
                        {
                            Close();
                        }
                        else
                        {
                            SetInputPaint(--_inputPosition, true);
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
                        SetInputPaint(_inputPosition, false);
                        if (_inputPosition == 11)
                        {
                            Close();
                        }
                        else
                        {
                            SetInputPaint(++_inputPosition, true);
                        }
                    }
                    break;
                case VirtualKey.Back:
                case VirtualKey.Delete:
                    if (AllowEssentialInputs)
                    {
                        SetInputImpl();
                    }
                    else
                    {
                        Configure.Instance.DefaultInputBundlesV6.StandardInputs[_inputPosition].Data = VirtualKey.None;
                        SetInput(_inputPosition, Configure.Instance.DefaultInputBundlesV6.StandardInputs[_inputPosition]);
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
                var defaultInput = new DefaultInput
                {
                    Data = e
                };
                if (Utility.AllowInput(Configure.Instance.DefaultInputBundlesV6, defaultInput))
                {
                    Configure.Instance.DefaultInputBundlesV6.StandardInputs[_inputPosition] = defaultInput;
                    SetInput(_inputPosition, defaultInput);
                }
            }
        }

        public void SetInput(int inputPosition, DefaultInput defaultInput)
        {
            var defaultInputText = defaultInput.ToString();
            if (string.IsNullOrEmpty(defaultInputText))
            {
                defaultInputText = "❌";
            }
            Inputs[inputPosition + 1] = string.Format(inputPosition switch
            {
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
            }, defaultInputText);
            OnPropertyChanged(nameof(Inputs));
            if (inputPosition == PostItem0 || inputPosition == PostItem1)
            {
                Configure.Instance.NotifySetPostItemInputText();
            }
        }

        public void SetInputPaint(int inputPosition, bool isInput)
        {
            InputPaints[inputPosition + 1] = isInput ? Paints.PointPaints[1] : Brushes.Transparent;
            OnPropertyChanged(nameof(InputPaints));
        }

        public override void OnOpened()
        {
            base.OnOpened();
            AllowEssentialInputs = false;
            for (var i = Configure.Instance.DefaultInputBundlesV6.StandardInputs.Length - 1; i >= 0; --i)
            {
                SetInputPaint(i, false);
                SetInput(i, Configure.Instance.DefaultInputBundlesV6.StandardInputs[i]);
            }
            _inputPosition = CallingInputPosition;
            SetInputPaint(_inputPosition, true);
        }
    }
}