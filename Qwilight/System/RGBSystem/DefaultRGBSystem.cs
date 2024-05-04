using RGB.NET.Core;
using System.Collections.Frozen;
using Windows.System;

namespace Qwilight
{
    public abstract class DefaultRGBSystem : BaseRGBSystem
    {
        static LedId GetInput(VirtualKey rawInput) => rawInput switch
        {
            VirtualKey.Escape => LedId.Keyboard_Escape,
            VirtualKey.F1 => LedId.Keyboard_F1,
            VirtualKey.F2 => LedId.Keyboard_F2,
            VirtualKey.F3 => LedId.Keyboard_F3,
            VirtualKey.F4 => LedId.Keyboard_F4,
            VirtualKey.F5 => LedId.Keyboard_F5,
            VirtualKey.F6 => LedId.Keyboard_F6,
            VirtualKey.F7 => LedId.Keyboard_F7,
            VirtualKey.F8 => LedId.Keyboard_F8,
            VirtualKey.F9 => LedId.Keyboard_F9,
            VirtualKey.F10 => LedId.Keyboard_F10,
            VirtualKey.F11 => LedId.Keyboard_F11,
            VirtualKey.F12 => LedId.Keyboard_F12,
            VirtualKey.Insert => LedId.Keyboard_Insert,
            VirtualKey.Delete => LedId.Keyboard_Delete,
            (VirtualKey)192 => LedId.Keyboard_GraveAccentAndTilde,
            VirtualKey.Number1 => LedId.Keyboard_1,
            VirtualKey.Number2 => LedId.Keyboard_2,
            VirtualKey.Number3 => LedId.Keyboard_3,
            VirtualKey.Number4 => LedId.Keyboard_4,
            VirtualKey.Number5 => LedId.Keyboard_5,
            VirtualKey.Number6 => LedId.Keyboard_6,
            VirtualKey.Number7 => LedId.Keyboard_7,
            VirtualKey.Number8 => LedId.Keyboard_8,
            VirtualKey.Number9 => LedId.Keyboard_9,
            VirtualKey.Number0 => LedId.Keyboard_0,
            (VirtualKey)189 => LedId.Keyboard_MinusAndUnderscore,
            (VirtualKey)187 => LedId.Keyboard_EqualsAndPlus,
            VirtualKey.Back => LedId.Keyboard_Backspace,
            VirtualKey.Tab => LedId.Keyboard_Tab,
            VirtualKey.Q => LedId.Keyboard_Q,
            VirtualKey.W => LedId.Keyboard_W,
            VirtualKey.E => LedId.Keyboard_E,
            VirtualKey.R => LedId.Keyboard_R,
            VirtualKey.T => LedId.Keyboard_T,
            VirtualKey.Y => LedId.Keyboard_Y,
            VirtualKey.U => LedId.Keyboard_U,
            VirtualKey.I => LedId.Keyboard_I,
            VirtualKey.O => LedId.Keyboard_O,
            VirtualKey.P => LedId.Keyboard_P,
            (VirtualKey)219 => LedId.Keyboard_BracketLeft,
            (VirtualKey)221 => LedId.Keyboard_BracketRight,
            (VirtualKey)220 => LedId.Keyboard_Backslash,
            VirtualKey.CapitalLock => LedId.Keyboard_CapsLock,
            VirtualKey.A => LedId.Keyboard_A,
            VirtualKey.S => LedId.Keyboard_S,
            VirtualKey.D => LedId.Keyboard_D,
            VirtualKey.F => LedId.Keyboard_F,
            VirtualKey.G => LedId.Keyboard_G,
            VirtualKey.H => LedId.Keyboard_H,
            VirtualKey.J => LedId.Keyboard_J,
            VirtualKey.K => LedId.Keyboard_K,
            VirtualKey.L => LedId.Keyboard_L,
            (VirtualKey)186 => LedId.Keyboard_SemicolonAndColon,
            (VirtualKey)222 => LedId.Keyboard_ApostropheAndDoubleQuote,
            VirtualKey.Enter => LedId.Keyboard_Enter,
            VirtualKey.LeftShift => LedId.Keyboard_LeftShift,
            VirtualKey.Z => LedId.Keyboard_Z,
            VirtualKey.X => LedId.Keyboard_X,
            VirtualKey.C => LedId.Keyboard_C,
            VirtualKey.V => LedId.Keyboard_V,
            VirtualKey.B => LedId.Keyboard_B,
            VirtualKey.N => LedId.Keyboard_N,
            VirtualKey.M => LedId.Keyboard_M,
            (VirtualKey)188 => LedId.Keyboard_CommaAndLessThan,
            (VirtualKey)190 => LedId.Keyboard_PeriodAndBiggerThan,
            (VirtualKey)191 => LedId.Keyboard_SlashAndQuestionMark,
            VirtualKey.RightShift => LedId.Keyboard_RightShift,
            VirtualKey.LeftControl => LedId.Keyboard_LeftCtrl,
            VirtualKey.LeftWindows => LedId.Keyboard_LeftGui,
            VirtualKey.LeftMenu => LedId.Keyboard_LeftAlt,
            VirtualKey.Space => LedId.Keyboard_Space,
            VirtualKey.RightMenu => LedId.Keyboard_RightAlt,
            VirtualKey.RightControl => LedId.Keyboard_RightCtrl,
            VirtualKey.Left => LedId.Keyboard_ArrowLeft,
            VirtualKey.Up => LedId.Keyboard_ArrowUp,
            VirtualKey.Down => LedId.Keyboard_ArrowDown,
            VirtualKey.Right => LedId.Keyboard_ArrowRight,
            VirtualKey.NumberKeyLock => LedId.Keyboard_NumLock,
            VirtualKey.NumberPad0 => LedId.Keyboard_Num0,
            VirtualKey.NumberPad1 => LedId.Keyboard_Num1,
            VirtualKey.NumberPad2 => LedId.Keyboard_Num2,
            VirtualKey.NumberPad3 => LedId.Keyboard_Num3,
            VirtualKey.NumberPad4 => LedId.Keyboard_Num4,
            VirtualKey.NumberPad5 => LedId.Keyboard_Num5,
            VirtualKey.NumberPad6 => LedId.Keyboard_Num6,
            VirtualKey.NumberPad7 => LedId.Keyboard_Num7,
            VirtualKey.NumberPad8 => LedId.Keyboard_Num8,
            VirtualKey.NumberPad9 => LedId.Keyboard_Num9,
            VirtualKey.Snapshot => LedId.Keyboard_PrintScreen,
            VirtualKey.Scroll => LedId.Keyboard_ScrollLock,
            VirtualKey.Pause => LedId.Keyboard_PauseBreak,
            VirtualKey.Home => LedId.Keyboard_Home,
            VirtualKey.PageUp => LedId.Keyboard_PageUp,
            VirtualKey.End => LedId.Keyboard_End,
            VirtualKey.PageDown => LedId.Keyboard_PageDown,
            _ => LedId.Invalid,
        };

        static RGB.NET.Core.Color GetColor(uint valueColor) => new((byte)(valueColor & 255), (byte)((valueColor & 65280) >> 8), (byte)((valueColor & 16711680) >> 16), (byte)((valueColor & 4278190080) >> 24));

        RGBSurface _rgbSystem;
        FrozenDictionary<LedId, Led> _rgbs;
        ManualUpdateTrigger _trigger;

        public abstract void Load(RGBSurface rgbSystem);

        public override bool Init()
        {
            _rgbSystem = new();
            Load(_rgbSystem);
            _rgbSystem.AlignDevices();
            _rgbs = _rgbSystem.Leds.ToFrozenDictionary(rgb => rgb.Id, rgb => rgb);
            _trigger = new();
            _trigger.Start();
            _rgbSystem.RegisterUpdateTrigger(_trigger);
            return true;
        }

        public override void SetInputColor(VirtualKey rawInput, uint value)
        {
            var input = GetInput(rawInput);
            if (input != LedId.Invalid && _rgbs.TryGetValue(input, out var rgb))
            {
                rgb.Color = GetColor(value);
            }
        }

        public override void SetStatusColors(double status, uint value0, uint value1, uint value2, uint value3)
        {
        }

        public override void SetEtcColor(uint value)
        {
        }

        public override void OnBeforeHandle()
        {
        }

        public override void OnHandled()
        {
            _trigger.TriggerUpdate();
        }

        public override void Dispose()
        {
            lock (RGBSystem.Instance.HandlingCSX)
            {
                if (IsHandling)
                {
                    IsHandling = false;
                    _rgbSystem.UnregisterUpdateTrigger(_trigger);
                    _trigger.Dispose();
                    _rgbSystem.Dispose();
                }
            }
        }
    }
}
