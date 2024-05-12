using Microsoft.UI;
using Qwilight.Utilities;
using System.IO;
using Windows.System;
using Windows.UI;
using Wooting;

namespace Qwilight
{
    public sealed class WootingSystem : BaseRGBSystem
    {
        public static readonly WootingSystem Instance = new();

        public WootingSystem()
        {
#if X64
            Utility.CopyFile(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "wooting-rgb-sdk64.dll"), Path.Combine(AppContext.BaseDirectory, "wooting-rgb-sdk.dll"));
#endif
        }

        static WootingKey.Keys GetInput(VirtualKey input) => input switch
        {
            VirtualKey.Escape => WootingKey.Keys.Esc,
            VirtualKey.F1 => WootingKey.Keys.F1,
            VirtualKey.F2 => WootingKey.Keys.F2,
            VirtualKey.F3 => WootingKey.Keys.F3,
            VirtualKey.F4 => WootingKey.Keys.F4,
            VirtualKey.F5 => WootingKey.Keys.F5,
            VirtualKey.F6 => WootingKey.Keys.F6,
            VirtualKey.F7 => WootingKey.Keys.F7,
            VirtualKey.F8 => WootingKey.Keys.F8,
            VirtualKey.F9 => WootingKey.Keys.F9,
            VirtualKey.F10 => WootingKey.Keys.F10,
            VirtualKey.F11 => WootingKey.Keys.F11,
            VirtualKey.F12 => WootingKey.Keys.F12,
            VirtualKey.Insert => WootingKey.Keys.Insert,
            VirtualKey.Delete => WootingKey.Keys.Delete,
            (VirtualKey)192 => WootingKey.Keys.Tilda,
            VirtualKey.Number1 => WootingKey.Keys.N1,
            VirtualKey.Number2 => WootingKey.Keys.N2,
            VirtualKey.Number3 => WootingKey.Keys.N3,
            VirtualKey.Number4 => WootingKey.Keys.N4,
            VirtualKey.Number5 => WootingKey.Keys.N5,
            VirtualKey.Number6 => WootingKey.Keys.N6,
            VirtualKey.Number7 => WootingKey.Keys.N7,
            VirtualKey.Number8 => WootingKey.Keys.N8,
            VirtualKey.Number9 => WootingKey.Keys.N9,
            VirtualKey.Number0 => WootingKey.Keys.N0,
            (VirtualKey)189 => WootingKey.Keys.Minus,
            (VirtualKey)187 => WootingKey.Keys.Equals,
            VirtualKey.Back => WootingKey.Keys.Backspace,
            VirtualKey.Tab => WootingKey.Keys.Tab,
            VirtualKey.Q => WootingKey.Keys.Q,
            VirtualKey.W => WootingKey.Keys.W,
            VirtualKey.E => WootingKey.Keys.E,
            VirtualKey.R => WootingKey.Keys.R,
            VirtualKey.T => WootingKey.Keys.T,
            VirtualKey.Y => WootingKey.Keys.Y,
            VirtualKey.U => WootingKey.Keys.U,
            VirtualKey.I => WootingKey.Keys.I,
            VirtualKey.O => WootingKey.Keys.O,
            VirtualKey.P => WootingKey.Keys.P,
            (VirtualKey)219 => WootingKey.Keys.OpenBracket,
            (VirtualKey)221 => WootingKey.Keys.CloseBracket,
            (VirtualKey)220 => WootingKey.Keys.ANSI_Backslash,
            VirtualKey.CapitalLock => WootingKey.Keys.CapsLock,
            VirtualKey.A => WootingKey.Keys.A,
            VirtualKey.S => WootingKey.Keys.S,
            VirtualKey.D => WootingKey.Keys.D,
            VirtualKey.F => WootingKey.Keys.F,
            VirtualKey.G => WootingKey.Keys.G,
            VirtualKey.H => WootingKey.Keys.H,
            VirtualKey.J => WootingKey.Keys.J,
            VirtualKey.K => WootingKey.Keys.K,
            VirtualKey.L => WootingKey.Keys.L,
            (VirtualKey)186 => WootingKey.Keys.SemiColon,
            (VirtualKey)222 => WootingKey.Keys.Apostophe,
            VirtualKey.Enter => WootingKey.Keys.Enter,
            VirtualKey.LeftShift => WootingKey.Keys.LeftShift,
            VirtualKey.Z => WootingKey.Keys.Z,
            VirtualKey.X => WootingKey.Keys.X,
            VirtualKey.C => WootingKey.Keys.C,
            VirtualKey.V => WootingKey.Keys.V,
            VirtualKey.B => WootingKey.Keys.B,
            VirtualKey.N => WootingKey.Keys.N,
            VirtualKey.M => WootingKey.Keys.M,
            (VirtualKey)188 => WootingKey.Keys.Comma,
            (VirtualKey)190 => WootingKey.Keys.Period,
            (VirtualKey)191 => WootingKey.Keys.Slash,
            VirtualKey.RightShift => WootingKey.Keys.RightShift,
            VirtualKey.LeftControl => WootingKey.Keys.LeftCtrl,
            VirtualKey.LeftWindows => WootingKey.Keys.LeftWin,
            VirtualKey.LeftMenu => WootingKey.Keys.LeftAlt,
            VirtualKey.Space => WootingKey.Keys.Space,
            VirtualKey.RightMenu => WootingKey.Keys.RightAlt,
            VirtualKey.RightControl => WootingKey.Keys.RightControl,
            VirtualKey.Left => WootingKey.Keys.Left,
            VirtualKey.Up => WootingKey.Keys.Up,
            VirtualKey.Down => WootingKey.Keys.Down,
            VirtualKey.Right => WootingKey.Keys.Right,
            VirtualKey.NumberKeyLock => WootingKey.Keys.NumLock,
            VirtualKey.NumberPad0 => WootingKey.Keys.Num0,
            VirtualKey.NumberPad1 => WootingKey.Keys.Num1,
            VirtualKey.NumberPad2 => WootingKey.Keys.Num2,
            VirtualKey.NumberPad3 => WootingKey.Keys.Num3,
            VirtualKey.NumberPad4 => WootingKey.Keys.Num4,
            VirtualKey.NumberPad5 => WootingKey.Keys.Num5,
            VirtualKey.NumberPad6 => WootingKey.Keys.Num6,
            VirtualKey.NumberPad7 => WootingKey.Keys.Num7,
            VirtualKey.NumberPad8 => WootingKey.Keys.Num8,
            VirtualKey.NumberPad9 => WootingKey.Keys.Num9,
            VirtualKey.Snapshot => WootingKey.Keys.PrintScreen,
            VirtualKey.Scroll => WootingKey.Keys.Mode_ScrollLock,
            VirtualKey.Pause => WootingKey.Keys.PauseBreak,
            VirtualKey.Home => WootingKey.Keys.Home,
            VirtualKey.PageUp => WootingKey.Keys.PageUp,
            VirtualKey.End => WootingKey.Keys.End,
            VirtualKey.PageDown => WootingKey.Keys.PageDown,
            _ => WootingKey.Keys.None
        };

        public override bool IsAvailable => Configure.Instance.Wooting;

        public override bool Init()
        {
            try
            {
                var isConnected = RGBControl.IsConnected();
                Console.WriteLine("isConnected: " + isConnected);
                return isConnected;
            }
            catch
            {
                return false;
            }
        }

        public override void SetInputColor(VirtualKey rawInput, uint value)
        {
            var deviceCount = RGBControl.GetDeviceCount();
            Console.WriteLine("deviceCount: " + deviceCount);
            for (var i = (byte)(deviceCount - 1); i >= 0; --i)
            {
                RGBControl.SetControlDevice(i);
                var input = GetInput(rawInput);
                if (input != WootingKey.Keys.None)
                {
                    var result = RGBControl.SetKey(input, (byte)(value & 255), (byte)((value & 65280) >> 8), (byte)((value & 16711680) >> 16));
                    Console.WriteLine("SetKey: " + result);
                }
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
            var result = RGBControl.UpdateKeyboard();
            Console.WriteLine("UpdateKeyboard: " + result);
        }

        public override Color GetMeterColor() => Colors.Yellow;

        public override void Dispose()
        {
            lock (RGBSystem.Instance.HandlingCSX)
            {
                if (IsHandling)
                {
                    IsHandling = false;
                    RGBControl.Close();
                }
            }
        }

        public override void Toggle()
        {
            Configure.Instance.Wooting = !Configure.Instance.Wooting;
            base.Toggle();
        }
    }
}
