using CUESDK;
using Microsoft.UI;
using System.Buffers;
using Windows.System;
using Windows.UI;

namespace Qwilight
{
    public sealed class K70System : BaseRGBSystem
    {
        public static readonly K70System Instance = new();

        static CorsairLedId GetInput(VirtualKey rawInput) => rawInput switch
        {
            VirtualKey.Escape => CorsairLedId.KeyboardEscape,
            VirtualKey.F1 => CorsairLedId.KeyboardF1,
            VirtualKey.F2 => CorsairLedId.KeyboardF2,
            VirtualKey.F3 => CorsairLedId.KeyboardF3,
            VirtualKey.F4 => CorsairLedId.KeyboardF4,
            VirtualKey.F5 => CorsairLedId.KeyboardF5,
            VirtualKey.F6 => CorsairLedId.KeyboardF6,
            VirtualKey.F7 => CorsairLedId.KeyboardF7,
            VirtualKey.F8 => CorsairLedId.KeyboardF8,
            VirtualKey.F9 => CorsairLedId.KeyboardF9,
            VirtualKey.F10 => CorsairLedId.KeyboardF10,
            VirtualKey.F11 => CorsairLedId.KeyboardF11,
            VirtualKey.F12 => CorsairLedId.KeyboardF12,
            VirtualKey.Insert => CorsairLedId.KeyboardInsert,
            VirtualKey.Delete => CorsairLedId.KeyboardDelete,
            (VirtualKey)192 => CorsairLedId.KeyboardGraveAccentAndTilde,
            VirtualKey.Number1 => CorsairLedId.Keyboard1,
            VirtualKey.Number2 => CorsairLedId.Keyboard2,
            VirtualKey.Number3 => CorsairLedId.Keyboard3,
            VirtualKey.Number4 => CorsairLedId.Keyboard4,
            VirtualKey.Number5 => CorsairLedId.Keyboard5,
            VirtualKey.Number6 => CorsairLedId.Keyboard6,
            VirtualKey.Number7 => CorsairLedId.Keyboard7,
            VirtualKey.Number8 => CorsairLedId.Keyboard8,
            VirtualKey.Number9 => CorsairLedId.Keyboard9,
            VirtualKey.Number0 => CorsairLedId.Keyboard0,
            (VirtualKey)189 => CorsairLedId.KeyboardMinusAndUnderscore,
            (VirtualKey)187 => CorsairLedId.KeyboardEqualsAndPlus,
            VirtualKey.Back => CorsairLedId.KeyboardBackspace,
            VirtualKey.Tab => CorsairLedId.KeyboardTab,
            VirtualKey.Q => CorsairLedId.KeyboardQ,
            VirtualKey.W => CorsairLedId.KeyboardW,
            VirtualKey.E => CorsairLedId.KeyboardE,
            VirtualKey.R => CorsairLedId.KeyboardR,
            VirtualKey.T => CorsairLedId.KeyboardT,
            VirtualKey.Y => CorsairLedId.KeyboardY,
            VirtualKey.U => CorsairLedId.KeyboardU,
            VirtualKey.I => CorsairLedId.KeyboardI,
            VirtualKey.O => CorsairLedId.KeyboardO,
            VirtualKey.P => CorsairLedId.KeyboardP,
            (VirtualKey)219 => CorsairLedId.KeyboardBracketLeft,
            (VirtualKey)221 => CorsairLedId.KeyboardBracketRight,
            (VirtualKey)220 => CorsairLedId.KeyboardBackslash,
            VirtualKey.CapitalLock => CorsairLedId.KeyboardCapsLock,
            VirtualKey.A => CorsairLedId.KeyboardA,
            VirtualKey.S => CorsairLedId.KeyboardS,
            VirtualKey.D => CorsairLedId.KeyboardD,
            VirtualKey.F => CorsairLedId.KeyboardF,
            VirtualKey.G => CorsairLedId.KeyboardG,
            VirtualKey.H => CorsairLedId.KeyboardH,
            VirtualKey.J => CorsairLedId.KeyboardJ,
            VirtualKey.K => CorsairLedId.KeyboardK,
            VirtualKey.L => CorsairLedId.KeyboardL,
            (VirtualKey)186 => CorsairLedId.KeyboardSemicolonAndColon,
            (VirtualKey)222 => CorsairLedId.KeyboardApostropheAndDoubleQuote,
            VirtualKey.Enter => CorsairLedId.KeyboardEnter,
            VirtualKey.LeftShift => CorsairLedId.KeyboardLeftShift,
            VirtualKey.Z => CorsairLedId.KeyboardZ,
            VirtualKey.X => CorsairLedId.KeyboardX,
            VirtualKey.C => CorsairLedId.KeyboardC,
            VirtualKey.V => CorsairLedId.KeyboardV,
            VirtualKey.B => CorsairLedId.KeyboardB,
            VirtualKey.N => CorsairLedId.KeyboardN,
            VirtualKey.M => CorsairLedId.KeyboardM,
            (VirtualKey)188 => CorsairLedId.KeyboardCommaAndLessThan,
            (VirtualKey)190 => CorsairLedId.KeyboardPeriodAndBiggerThan,
            (VirtualKey)191 => CorsairLedId.KeyboardSlashAndQuestionMark,
            VirtualKey.RightShift => CorsairLedId.KeyboardRightShift,
            VirtualKey.LeftControl => CorsairLedId.KeyboardLeftCtrl,
            VirtualKey.LeftWindows => CorsairLedId.KeyboardLeftGui,
            VirtualKey.LeftMenu => CorsairLedId.KeyboardLeftAlt,
            VirtualKey.Space => CorsairLedId.KeyboardSpace,
            VirtualKey.RightMenu => CorsairLedId.KeyboardRightAlt,
            VirtualKey.RightControl => CorsairLedId.KeyboardRightCtrl,
            VirtualKey.Left => CorsairLedId.KeyboardLeftArrow,
            VirtualKey.Up => CorsairLedId.KeyboardUpArrow,
            VirtualKey.Down => CorsairLedId.KeyboardDownArrow,
            VirtualKey.Right => CorsairLedId.KeyboardRightArrow,
            VirtualKey.NumberKeyLock => CorsairLedId.KeyboardNumLock,
            VirtualKey.NumberPad0 => CorsairLedId.KeyboardKeypad0,
            VirtualKey.NumberPad1 => CorsairLedId.KeyboardKeypad1,
            VirtualKey.NumberPad2 => CorsairLedId.KeyboardKeypad2,
            VirtualKey.NumberPad3 => CorsairLedId.KeyboardKeypad3,
            VirtualKey.NumberPad4 => CorsairLedId.KeyboardKeypad4,
            VirtualKey.NumberPad5 => CorsairLedId.KeyboardKeypad5,
            VirtualKey.NumberPad6 => CorsairLedId.KeyboardKeypad6,
            VirtualKey.NumberPad7 => CorsairLedId.KeyboardKeypad7,
            VirtualKey.NumberPad8 => CorsairLedId.KeyboardKeypad8,
            VirtualKey.NumberPad9 => CorsairLedId.KeyboardKeypad9,
            VirtualKey.Snapshot => CorsairLedId.KeyboardPrintScreen,
            VirtualKey.Scroll => CorsairLedId.KeyboardScrollLock,
            VirtualKey.Pause => CorsairLedId.KeyboardPauseBreak,
            VirtualKey.Home => CorsairLedId.KeyboardHome,
            VirtualKey.PageUp => CorsairLedId.KeyboardPageUp,
            VirtualKey.End => CorsairLedId.KeyboardEnd,
            VirtualKey.PageDown => CorsairLedId.KeyboardPageDown,
            _ => default,
        };

        readonly CorsairLedId[] _rgbIDs;
        readonly Dictionary<CorsairLedId, uint> _rgbIDMap = new();
        readonly CorsairLedColor[] _rgbColors;
        int _rgbCount;

        K70System()
        {
            _rgbIDs = (Enum.GetValues(typeof(CorsairLedId)) as CorsairLedId[]).Where(input => (CorsairLedId.KeyboardEscape <= input && input <= CorsairLedId.KeyboardFn) || input == CorsairLedId.KeyboardLogo || (CorsairLedId.KeyboardLightPipeZone1 <= input && input <= CorsairLedId.KeyboardLightPipeZone19) || (CorsairLedId.KeyboardLightPipeZone20 <= input && input <= CorsairLedId.KeyboardProfile)).ToArray();
            _rgbColors = new CorsairLedColor[_rgbIDs.Length];
        }

        public override bool IsAvailable => Configure.Instance.K70;

        public override bool Init()
        {
            CorsairLightingSDK.PerformProtocolHandshake();
            return CorsairLightingSDK.GetLastError() == CorsairError.Success && CorsairLightingSDK.RequestControl(CorsairAccessMode.ExclusiveLightingControl);
        }

        public override void SetInputColor(VirtualKey rawInput, uint value)
        {
            var input = GetInput(rawInput);
            if (input != CorsairLedId.Invalid)
            {
                _rgbIDMap[input] = value;
            }
        }

        public override void SetStatusColors(double status, uint value0, uint value1, uint value2, uint value3)
        {
        }

        public override void SetEtcColor(uint value)
        {
            _rgbIDMap[CorsairLedId.KeyboardLogo] = value;
            for (var i = CorsairLedId.KeyboardLightPipeZone19; i >= CorsairLedId.KeyboardLightPipeZone1; --i)
            {
                _rgbIDMap[i] = value;
            }
            for (var i = CorsairLedId.KeyboardProfile; i >= CorsairLedId.KeyboardLightPipeZone20; --i)
            {
                _rgbIDMap[i] = value;
            }
        }

        public override void OnBeforeHandle()
        {
            _rgbIDMap.Clear();
            _rgbCount = CorsairLightingSDK.GetDeviceCount();
        }

        public override void OnHandled()
        {
            for (var i = _rgbIDs.Length - 1; i >= 0; --i)
            {
                var input = _rgbIDs[i];
                _rgbColors[i] = _rgbIDMap.TryGetValue(input, out var value) ? new()
                {
                    LedId = input,
                    R = (int)(value & 255),
                    G = (int)((value >> 8) & 255),
                    B = (int)(value >> 16)
                } : new()
                {
                    LedId = input
                };
            }
            for (var i = _rgbCount - 1; i >= 0; --i)
            {
                CorsairLightingSDK.SetLedsColorsBufferByDeviceIndex(i, _rgbColors);
            }
            CorsairLightingSDK.SetLedsColorsFlushBuffer();
        }

        public override Color GetMeterColor() => Colors.Yellow;

        public override void Dispose()
        {
            lock (RGBSystem.Instance.HandlingCSX)
            {
                if (IsHandling)
                {
                    IsHandling = false;
                    CorsairLightingSDK.ReleaseControl(CorsairAccessMode.ExclusiveLightingControl);
                }
            }
        }

        public override void Toggle()
        {
            Configure.Instance.K70 = !Configure.Instance.K70;
            base.Toggle();
        }
    }
}
