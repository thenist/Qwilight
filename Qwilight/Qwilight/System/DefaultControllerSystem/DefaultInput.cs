using Windows.System;

namespace Qwilight
{
    public struct DefaultInput : IEquatable<DefaultInput>
    {
        public VirtualKey Data { get; set; }

        public override bool Equals(object obj) => obj is DefaultInput defaultInput && Equals(defaultInput);

        public bool Equals(DefaultInput other)
        {
            return Data == other.Data;
        }

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString() => Data switch
        {
            VirtualKey.None => string.Empty,
            VirtualKey.Number0 => "0",
            VirtualKey.Number1 => "1",
            VirtualKey.Number2 => "2",
            VirtualKey.Number3 => "3",
            VirtualKey.Number4 => "4",
            VirtualKey.Number5 => "5",
            VirtualKey.Number6 => "6",
            VirtualKey.Number7 => "7",
            VirtualKey.Number8 => "8",
            VirtualKey.Number9 => "9",
            (VirtualKey)192 => "`",
            (VirtualKey)186 => ";",
            (VirtualKey)222 => @"'",
            (VirtualKey)220 => @"\",
            (VirtualKey)219 => "[",
            (VirtualKey)221 => "]",
            (VirtualKey)191 => "/",
            (VirtualKey)188 => ",",
            (VirtualKey)190 => ".",
            VirtualKey.LeftMenu => "LM",
            VirtualKey.LeftControl => "LC",
            VirtualKey.LeftShift => "LS",
            VirtualKey.RightMenu => "RM",
            VirtualKey.RightControl => "RC",
            VirtualKey.RightShift => "RS",
            (VirtualKey)189 => "-",
            (VirtualKey)187 => "=",
            VirtualKey.Add => "NP+",
            VirtualKey.Subtract => "NP-",
            VirtualKey.Multiply => "NP*",
            VirtualKey.Divide => "NP/",
            VirtualKey.Decimal => "NP.",
            VirtualKey.PageUp => "PU",
            VirtualKey.PageDown => "PD",
            VirtualKey.NumberKeyLock => "NKL",
            VirtualKey.NumberPad0 => "NP0",
            VirtualKey.NumberPad1 => "NP1",
            VirtualKey.NumberPad2 => "NP2",
            VirtualKey.NumberPad3 => "NP3",
            VirtualKey.NumberPad4 => "NP4",
            VirtualKey.NumberPad5 => "NP5",
            VirtualKey.NumberPad6 => "NP6",
            VirtualKey.NumberPad7 => "NP7",
            VirtualKey.NumberPad8 => "NP8",
            VirtualKey.NumberPad9 => "NP9",
            VirtualKey.Up => "▲",
            VirtualKey.Down => "▼",
            VirtualKey.Right => "▶",
            VirtualKey.Left => "◀",
            VirtualKey.Back => "←",
            _ => Data.ToString(),
        };

        public static bool operator ==(DefaultInput left, DefaultInput right) => left.Equals(right);

        public static bool operator !=(DefaultInput left, DefaultInput right) => !(left == right);
    }
}