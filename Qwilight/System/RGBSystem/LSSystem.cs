using LedCSharp;
using Microsoft.UI;
using Qwilight.Utilities;
using System.IO;
using Windows.System;
using Windows.UI;

namespace Qwilight
{
    public sealed class LSSystem : BaseRGBSystem
    {
        public static readonly LSSystem Instance = new();

        static keyboardNames GetInput(VirtualKey rawInput) => rawInput switch
        {
            VirtualKey.Escape => keyboardNames.ESC,
            VirtualKey.F1 => keyboardNames.F1,
            VirtualKey.F2 => keyboardNames.F2,
            VirtualKey.F3 => keyboardNames.F3,
            VirtualKey.F4 => keyboardNames.F4,
            VirtualKey.F5 => keyboardNames.F5,
            VirtualKey.F6 => keyboardNames.F6,
            VirtualKey.F7 => keyboardNames.F7,
            VirtualKey.F8 => keyboardNames.F8,
            VirtualKey.F9 => keyboardNames.F9,
            VirtualKey.F10 => keyboardNames.F10,
            VirtualKey.F11 => keyboardNames.F11,
            VirtualKey.F12 => keyboardNames.F12,
            VirtualKey.Insert => keyboardNames.INSERT,
            VirtualKey.Delete => keyboardNames.KEYBOARD_DELETE,
            (VirtualKey)192 => keyboardNames.TILDE,
            VirtualKey.Number1 => keyboardNames.ONE,
            VirtualKey.Number2 => keyboardNames.TWO,
            VirtualKey.Number3 => keyboardNames.THREE,
            VirtualKey.Number4 => keyboardNames.FOUR,
            VirtualKey.Number5 => keyboardNames.FIVE,
            VirtualKey.Number6 => keyboardNames.SIX,
            VirtualKey.Number7 => keyboardNames.SEVEN,
            VirtualKey.Number8 => keyboardNames.EIGHT,
            VirtualKey.Number9 => keyboardNames.NINE,
            VirtualKey.Number0 => keyboardNames.ZERO,
            (VirtualKey)189 => keyboardNames.MINUS,
            (VirtualKey)187 => keyboardNames.EQUALS,
            VirtualKey.Back => keyboardNames.BACKSPACE,
            VirtualKey.Tab => keyboardNames.TAB,
            VirtualKey.Q => keyboardNames.Q,
            VirtualKey.W => keyboardNames.W,
            VirtualKey.E => keyboardNames.E,
            VirtualKey.R => keyboardNames.R,
            VirtualKey.T => keyboardNames.T,
            VirtualKey.Y => keyboardNames.Y,
            VirtualKey.U => keyboardNames.U,
            VirtualKey.I => keyboardNames.I,
            VirtualKey.O => keyboardNames.O,
            VirtualKey.P => keyboardNames.P,
            (VirtualKey)219 => keyboardNames.OPEN_BRACKET,
            (VirtualKey)221 => keyboardNames.CLOSE_BRACKET,
            (VirtualKey)220 => keyboardNames.BACKSLASH,
            VirtualKey.CapitalLock => keyboardNames.CAPS_LOCK,
            VirtualKey.A => keyboardNames.A,
            VirtualKey.S => keyboardNames.S,
            VirtualKey.D => keyboardNames.D,
            VirtualKey.F => keyboardNames.F,
            VirtualKey.G => keyboardNames.G,
            VirtualKey.H => keyboardNames.H,
            VirtualKey.J => keyboardNames.J,
            VirtualKey.K => keyboardNames.K,
            VirtualKey.L => keyboardNames.L,
            (VirtualKey)186 => keyboardNames.SEMICOLON,
            (VirtualKey)222 => keyboardNames.APOSTROPHE,
            VirtualKey.Enter => keyboardNames.ENTER,
            VirtualKey.LeftShift => keyboardNames.LEFT_SHIFT,
            VirtualKey.Z => keyboardNames.Z,
            VirtualKey.X => keyboardNames.X,
            VirtualKey.C => keyboardNames.C,
            VirtualKey.V => keyboardNames.V,
            VirtualKey.B => keyboardNames.B,
            VirtualKey.N => keyboardNames.N,
            VirtualKey.M => keyboardNames.M,
            (VirtualKey)188 => keyboardNames.COMMA,
            (VirtualKey)190 => keyboardNames.PERIOD,
            (VirtualKey)191 => keyboardNames.FORWARD_SLASH,
            VirtualKey.RightShift => keyboardNames.RIGHT_SHIFT,
            VirtualKey.LeftControl => keyboardNames.LEFT_CONTROL,
            VirtualKey.LeftWindows => keyboardNames.LEFT_WINDOWS,
            VirtualKey.LeftMenu => keyboardNames.LEFT_ALT,
            VirtualKey.Space => keyboardNames.SPACE,
            VirtualKey.RightMenu => keyboardNames.RIGHT_ALT,
            VirtualKey.RightControl => keyboardNames.RIGHT_CONTROL,
            VirtualKey.Left => keyboardNames.ARROW_LEFT,
            VirtualKey.Up => keyboardNames.ARROW_UP,
            VirtualKey.Down => keyboardNames.ARROW_DOWN,
            VirtualKey.Right => keyboardNames.ARROW_RIGHT,
            VirtualKey.NumberKeyLock => keyboardNames.NUM_LOCK,
            VirtualKey.NumberPad0 => keyboardNames.NUM_ZERO,
            VirtualKey.NumberPad1 => keyboardNames.NUM_ONE,
            VirtualKey.NumberPad2 => keyboardNames.NUM_TWO,
            VirtualKey.NumberPad3 => keyboardNames.NUM_THREE,
            VirtualKey.NumberPad4 => keyboardNames.NUM_FOUR,
            VirtualKey.NumberPad5 => keyboardNames.NUM_FIVE,
            VirtualKey.NumberPad6 => keyboardNames.NUM_SIX,
            VirtualKey.NumberPad7 => keyboardNames.NUM_SEVEN,
            VirtualKey.NumberPad8 => keyboardNames.NUM_EIGHT,
            VirtualKey.NumberPad9 => keyboardNames.NUM_NINE,
            VirtualKey.Snapshot => keyboardNames.PRINT_SCREEN,
            VirtualKey.Scroll => keyboardNames.SCROLL_LOCK,
            VirtualKey.Pause => keyboardNames.PAUSE_BREAK,
            VirtualKey.Home => keyboardNames.HOME,
            VirtualKey.PageUp => keyboardNames.PAGE_UP,
            VirtualKey.End => keyboardNames.END,
            VirtualKey.PageDown => keyboardNames.PAGE_DOWN,
            _ => default
        };

        readonly keyboardNames[] _rgbIDs = (Enum.GetValues(typeof(keyboardNames)) as keyboardNames[]);
        readonly Dictionary<keyboardNames, uint> _rgbIDMap = new();

        LSSystem()
        {
            Utility.CopyFile(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "LogitechLedEnginesWrapper.dll"), Path.Combine(AppContext.BaseDirectory, "LogitechLedEnginesWrapper.dll"));
        }

        public override bool IsAvailable => Configure.Instance.LS;

        public override bool Init()
        {
            try
            {
                var isOK = LogitechGSDK.LogiLedInitWithName("Qwilight");
                if (isOK)
                {
                    LogitechGSDK.LogiLedSetTargetDevice(LogitechGSDK.LOGI_DEVICETYPE_ALL);
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
            if (input != default)
            {
                _rgbIDMap[input] = value;
            }
        }

        public override void SetStatusColors(double status, uint value0, uint value1, uint value2, uint value3)
        {
        }

        public override void SetEtcColor(uint value)
        {
            LogitechGSDK.LogiLedSetLightingForTargetZone(DeviceType.Headset, 0, (int)(100 * (value & 255) / 255), (int)(100 * ((value >> 8) & 255) / 255), (int)(100 * ((value >> 16) & 255) / 255));
            LogitechGSDK.LogiLedSetLightingForTargetZone(DeviceType.Keyboard, 0, (int)(100 * (value & 255) / 255), (int)(100 * ((value >> 8) & 255) / 255), (int)(100 * ((value >> 16) & 255) / 255));
            LogitechGSDK.LogiLedSetLightingForTargetZone(DeviceType.Mouse, 0, (int)(100 * (value & 255) / 255), (int)(100 * ((value >> 8) & 255) / 255), (int)(100 * ((value >> 16) & 255) / 255));
            LogitechGSDK.LogiLedSetLightingForTargetZone(DeviceType.Mousemat, 0, (int)(100 * (value & 255) / 255), (int)(100 * ((value >> 8) & 255) / 255), (int)(100 * ((value >> 16) & 255) / 255));
            LogitechGSDK.LogiLedSetLightingForTargetZone(DeviceType.Speaker, 0, (int)(100 * (value & 255) / 255), (int)(100 * ((value >> 8) & 255) / 255), (int)(100 * ((value >> 16) & 255) / 255));
        }

        public override void OnBeforeHandle()
        {
            _rgbIDMap.Clear();
        }

        public override void OnHandled()
        {
            foreach (var input in _rgbIDs)
            {
                var value = _rgbIDMap.GetValueOrDefault(input);
                LogitechGSDK.LogiLedSetLightingForKeyWithKeyName(input, (int)(100 * (value & 255) / 255), (int)(100 * ((value >> 8) & 255) / 255), (int)(100 * ((value >> 16) & 255) / 255));
            }
        }

        public override Color GetMeterColor() => Colors.Cyan;

        public override void Dispose()
        {
            lock (RGBSystem.Instance.HandlingCSX)
            {
                if (IsHandling)
                {
                    IsHandling = false;
                    LogitechGSDK.LogiLedShutdown();
                }
            }
        }

        public override void Toggle()
        {
            Configure.Instance.LS = !Configure.Instance.LS;
            base.Toggle();
        }
    }
}
