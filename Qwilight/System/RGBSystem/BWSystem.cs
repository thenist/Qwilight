using ChromaSDK;
using Microsoft.UI;
using Qwilight.Utilities;
using System.IO;
using Windows.System;
using Windows.UI;

namespace Qwilight
{
    public sealed class BWSystem : BaseRGBSystem
    {
        public static readonly BWSystem Instance = new();

        static Keyboard.RZKEY GetInput(VirtualKey input) => input switch
        {
            VirtualKey.Escape => Keyboard.RZKEY.RZKEY_ESC,
            VirtualKey.F1 => Keyboard.RZKEY.RZKEY_F1,
            VirtualKey.F2 => Keyboard.RZKEY.RZKEY_F2,
            VirtualKey.F3 => Keyboard.RZKEY.RZKEY_F3,
            VirtualKey.F4 => Keyboard.RZKEY.RZKEY_F4,
            VirtualKey.F5 => Keyboard.RZKEY.RZKEY_F5,
            VirtualKey.F6 => Keyboard.RZKEY.RZKEY_F6,
            VirtualKey.F7 => Keyboard.RZKEY.RZKEY_F7,
            VirtualKey.F8 => Keyboard.RZKEY.RZKEY_F8,
            VirtualKey.F9 => Keyboard.RZKEY.RZKEY_F9,
            VirtualKey.F10 => Keyboard.RZKEY.RZKEY_F10,
            VirtualKey.F11 => Keyboard.RZKEY.RZKEY_F11,
            VirtualKey.F12 => Keyboard.RZKEY.RZKEY_F12,
            VirtualKey.Insert => Keyboard.RZKEY.RZKEY_INSERT,
            VirtualKey.Delete => Keyboard.RZKEY.RZKEY_DELETE,
            (VirtualKey)192 => Keyboard.RZKEY.RZKEY_OEM_1,
            VirtualKey.Number1 => Keyboard.RZKEY.RZKEY_1,
            VirtualKey.Number2 => Keyboard.RZKEY.RZKEY_2,
            VirtualKey.Number3 => Keyboard.RZKEY.RZKEY_3,
            VirtualKey.Number4 => Keyboard.RZKEY.RZKEY_4,
            VirtualKey.Number5 => Keyboard.RZKEY.RZKEY_5,
            VirtualKey.Number6 => Keyboard.RZKEY.RZKEY_6,
            VirtualKey.Number7 => Keyboard.RZKEY.RZKEY_7,
            VirtualKey.Number8 => Keyboard.RZKEY.RZKEY_8,
            VirtualKey.Number9 => Keyboard.RZKEY.RZKEY_9,
            VirtualKey.Number0 => Keyboard.RZKEY.RZKEY_0,
            (VirtualKey)189 => Keyboard.RZKEY.RZKEY_OEM_2,
            (VirtualKey)187 => Keyboard.RZKEY.RZKEY_OEM_3,
            VirtualKey.Back => Keyboard.RZKEY.RZKEY_BACKSPACE,
            VirtualKey.Tab => Keyboard.RZKEY.RZKEY_TAB,
            VirtualKey.Q => Keyboard.RZKEY.RZKEY_Q,
            VirtualKey.W => Keyboard.RZKEY.RZKEY_W,
            VirtualKey.E => Keyboard.RZKEY.RZKEY_E,
            VirtualKey.R => Keyboard.RZKEY.RZKEY_R,
            VirtualKey.T => Keyboard.RZKEY.RZKEY_T,
            VirtualKey.Y => Keyboard.RZKEY.RZKEY_Y,
            VirtualKey.U => Keyboard.RZKEY.RZKEY_U,
            VirtualKey.I => Keyboard.RZKEY.RZKEY_I,
            VirtualKey.O => Keyboard.RZKEY.RZKEY_O,
            VirtualKey.P => Keyboard.RZKEY.RZKEY_P,
            (VirtualKey)219 => Keyboard.RZKEY.RZKEY_OEM_4,
            (VirtualKey)221 => Keyboard.RZKEY.RZKEY_OEM_5,
            (VirtualKey)220 => Keyboard.RZKEY.RZKEY_OEM_6,
            VirtualKey.CapitalLock => Keyboard.RZKEY.RZKEY_CAPSLOCK,
            VirtualKey.A => Keyboard.RZKEY.RZKEY_A,
            VirtualKey.S => Keyboard.RZKEY.RZKEY_S,
            VirtualKey.D => Keyboard.RZKEY.RZKEY_D,
            VirtualKey.F => Keyboard.RZKEY.RZKEY_F,
            VirtualKey.G => Keyboard.RZKEY.RZKEY_G,
            VirtualKey.H => Keyboard.RZKEY.RZKEY_H,
            VirtualKey.J => Keyboard.RZKEY.RZKEY_J,
            VirtualKey.K => Keyboard.RZKEY.RZKEY_K,
            VirtualKey.L => Keyboard.RZKEY.RZKEY_L,
            (VirtualKey)186 => Keyboard.RZKEY.RZKEY_OEM_7,
            (VirtualKey)222 => Keyboard.RZKEY.RZKEY_OEM_8,
            VirtualKey.Enter => Keyboard.RZKEY.RZKEY_ENTER,
            VirtualKey.LeftShift => Keyboard.RZKEY.RZKEY_LSHIFT,
            VirtualKey.Z => Keyboard.RZKEY.RZKEY_Z,
            VirtualKey.X => Keyboard.RZKEY.RZKEY_X,
            VirtualKey.C => Keyboard.RZKEY.RZKEY_C,
            VirtualKey.V => Keyboard.RZKEY.RZKEY_V,
            VirtualKey.B => Keyboard.RZKEY.RZKEY_B,
            VirtualKey.N => Keyboard.RZKEY.RZKEY_N,
            VirtualKey.M => Keyboard.RZKEY.RZKEY_M,
            (VirtualKey)188 => Keyboard.RZKEY.RZKEY_OEM_9,
            (VirtualKey)190 => Keyboard.RZKEY.RZKEY_OEM_10,
            (VirtualKey)191 => Keyboard.RZKEY.RZKEY_OEM_11,
            VirtualKey.RightShift => Keyboard.RZKEY.RZKEY_RSHIFT,
            VirtualKey.LeftControl => Keyboard.RZKEY.RZKEY_LCTRL,
            VirtualKey.LeftWindows => Keyboard.RZKEY.RZKEY_LWIN,
            VirtualKey.LeftMenu => Keyboard.RZKEY.RZKEY_LALT,
            VirtualKey.Space => Keyboard.RZKEY.RZKEY_SPACE,
            VirtualKey.RightMenu => Keyboard.RZKEY.RZKEY_RALT,
            VirtualKey.RightControl => Keyboard.RZKEY.RZKEY_RCTRL,
            VirtualKey.Left => Keyboard.RZKEY.RZKEY_LEFT,
            VirtualKey.Up => Keyboard.RZKEY.RZKEY_UP,
            VirtualKey.Down => Keyboard.RZKEY.RZKEY_DOWN,
            VirtualKey.Right => Keyboard.RZKEY.RZKEY_RIGHT,
            VirtualKey.NumberKeyLock => Keyboard.RZKEY.RZKEY_NUMLOCK,
            VirtualKey.NumberPad0 => Keyboard.RZKEY.RZKEY_NUMPAD0,
            VirtualKey.NumberPad1 => Keyboard.RZKEY.RZKEY_NUMPAD1,
            VirtualKey.NumberPad2 => Keyboard.RZKEY.RZKEY_NUMPAD2,
            VirtualKey.NumberPad3 => Keyboard.RZKEY.RZKEY_NUMPAD3,
            VirtualKey.NumberPad4 => Keyboard.RZKEY.RZKEY_NUMPAD4,
            VirtualKey.NumberPad5 => Keyboard.RZKEY.RZKEY_NUMPAD5,
            VirtualKey.NumberPad6 => Keyboard.RZKEY.RZKEY_NUMPAD6,
            VirtualKey.NumberPad7 => Keyboard.RZKEY.RZKEY_NUMPAD7,
            VirtualKey.NumberPad8 => Keyboard.RZKEY.RZKEY_NUMPAD8,
            VirtualKey.NumberPad9 => Keyboard.RZKEY.RZKEY_NUMPAD9,
            VirtualKey.Snapshot => Keyboard.RZKEY.RZKEY_PRINTSCREEN,
            VirtualKey.Scroll => Keyboard.RZKEY.RZKEY_SCROLL,
            VirtualKey.Pause => Keyboard.RZKEY.RZKEY_PAUSE,
            VirtualKey.Home => Keyboard.RZKEY.RZKEY_HOME,
            VirtualKey.PageUp => Keyboard.RZKEY.RZKEY_PAGEUP,
            VirtualKey.End => Keyboard.RZKEY.RZKEY_END,
            VirtualKey.PageDown => Keyboard.RZKEY.RZKEY_PAGEDOWN,
            _ => Keyboard.RZKEY.RZKEY_INVALID
        };

        int[] _dataInputColors;
        int[] _dataInputs;
        int[] _dataPoint;
        int[] _dataAudio;
        int[] _dataPanel;

        BWSystem()
        {
#if X64
            Utility.CopyFile(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "CChromaEditorLibrary64.dll"), Path.Combine(AppContext.BaseDirectory, "CChromaEditorLibrary64.dll"));
#endif
        }

        public override bool IsAvailable => Configure.Instance.BW;

        public override bool Init()
        {
            try
            {
                var data = new APPINFOTYPE
                {
                    Title = "Qwilight",
                    Description = "Qwilight",

                    Author_Name = "Taehui",
                    Author_Contact = "https://taehui.ddns.net",

                    SupportedDevice = 1,
                    Category = 2
                };
                var isOK = ChromaAnimationAPI.InitSDK(ref data) == 0;
                if (isOK)
                {
                    _dataInputColors = new int[ChromaAnimationAPI.GetMaxRow(ChromaAnimationAPI.Device2D.Keyboard) * ChromaAnimationAPI.GetMaxColumn(ChromaAnimationAPI.Device2D.Keyboard)];
                    _dataInputs = new int[ChromaAnimationAPI.GetMaxRow(ChromaAnimationAPI.Device2D.Keyboard) * ChromaAnimationAPI.GetMaxColumn(ChromaAnimationAPI.Device2D.Keyboard)];
                    _dataPoint = new int[ChromaAnimationAPI.GetMaxRow(ChromaAnimationAPI.Device2D.Mouse) * ChromaAnimationAPI.GetMaxColumn(ChromaAnimationAPI.Device2D.Mouse)];
                    _dataAudio = new int[ChromaAnimationAPI.GetMaxLeds(ChromaAnimationAPI.Device1D.Headset)];
                    _dataPanel = new int[ChromaAnimationAPI.GetMaxLeds(ChromaAnimationAPI.Device1D.Mousepad)];
                }
                return isOK;
            }
            catch
            {
                return false;
            }
        }

        public override void SetInputColor(VirtualKey rawInput, uint value)
        {
            var input = GetInput(rawInput);
            if (input != Keyboard.RZKEY.RZKEY_INVALID)
            {
                _dataInputColors[ChromaAnimationAPI.GetMaxColumn(ChromaAnimationAPI.Device2D.Keyboard) * (((int)input & 65280) >> 8) + ((int)input & 255)] = (int)value;
                _dataInputs[ChromaAnimationAPI.GetMaxColumn(ChromaAnimationAPI.Device2D.Keyboard) * (((int)input & 65280) >> 8) + ((int)input & 255)] = (int)value;
            }
        }

        public override void SetStatusColors(double status, uint value0, uint value1, uint value2, uint value3)
        {
            if (!double.IsNaN(status))
            {
                var value = _dataPanel.Length * status;
                var valueInt = (int)value;
                for (var i = valueInt - 1; i >= 0; --i)
                {
                    _dataPanel[i] = (int)Utility.GetColor(value0, value1, value2, value3);
                }
                var remainder = value - valueInt;
                if (remainder > 0.0)
                {
                    _dataPanel[valueInt] = (int)Utility.GetColor((uint)(value0 * remainder), (uint)(value1 * remainder), (uint)(value2 * remainder), (uint)(value3 * remainder));
                }
            }
        }

        public override void SetEtcColor(uint value)
        {
            Array.Fill(_dataAudio, (int)value);
            Array.Fill(_dataPoint, (int)value);
        }

        public override void OnBeforeHandle()
        {
            Array.Clear(_dataInputColors, 0, _dataInputColors.Length);
            Array.Clear(_dataInputs, 0, _dataInputs.Length);
            Array.Clear(_dataAudio, 0, _dataAudio.Length);
            Array.Clear(_dataPoint, 0, _dataPoint.Length);
            Array.Clear(_dataPanel, 0, _dataPanel.Length);
        }

        public override void OnHandled()
        {
            ChromaAnimationAPI.SetCustomColorFlag2D((int)ChromaAnimationAPI.Device2D.Keyboard, _dataInputColors);
            ChromaAnimationAPI.SetEffectKeyboardCustom2D((int)ChromaAnimationAPI.Device2D.Keyboard, _dataInputColors, _dataInputs);
            ChromaAnimationAPI.SetEffectCustom1D((int)ChromaAnimationAPI.Device1D.Headset, _dataAudio);
            ChromaAnimationAPI.SetEffectCustom2D((int)ChromaAnimationAPI.Device2D.Mouse, _dataPoint);
            ChromaAnimationAPI.SetEffectCustom1D((int)ChromaAnimationAPI.Device1D.Mousepad, _dataPanel);
        }

        public override Color GetMeterColor() => Colors.Green;

        public override void Dispose()
        {
            lock (RGBSystem.Instance.HandlingCSX)
            {
                if (IsHandling)
                {
                    IsHandling = false;
                    try
                    {
                        ChromaAnimationAPI.StopAll();
                        ChromaAnimationAPI.CloseAll();
                        ChromaAnimationAPI.Uninit();
                    }
                    catch (DllNotFoundException)
                    {
                    }
                }
            }
        }

        public override void Toggle()
        {
            Configure.Instance.BW = !Configure.Instance.BW;
            base.Toggle();
        }
    }
}
