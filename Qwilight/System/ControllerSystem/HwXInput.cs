using Vortice.XInput;

namespace Qwilight
{
    public struct HwXInput : IEquatable<HwXInput>
    {
        public Gamepad Data { get; set; }

        public override bool Equals(object obj) => obj is HwXInput hwXInput && Equals(hwXInput);

        public bool Equals(HwXInput other)
        {
            return Data.Buttons == other.Data.Buttons &&
                Data.LeftTrigger == other.Data.LeftTrigger &&
                Data.RightTrigger == other.Data.RightTrigger &&
                Data.LeftThumbX == other.Data.LeftThumbX &&
                Data.LeftThumbY == other.Data.LeftThumbY &&
                Data.RightThumbX == other.Data.RightThumbX &&
                Data.RightThumbY == other.Data.RightThumbY;
        }

        public override int GetHashCode() => Data.GetHashCode();

        public override string ToString()
        {
            switch (Data.Buttons)
            {
                case GamepadButtons.A:
                case GamepadButtons.B:
                case GamepadButtons.Back:
                case GamepadButtons.Start:
                case GamepadButtons.X:
                case GamepadButtons.Y:
                    return Data.Buttons.ToString();
                case GamepadButtons.DPadDown:
                    return "DPD";
                case GamepadButtons.DPadLeft:
                    return "DPL";
                case GamepadButtons.DPadRight:
                    return "DPR";
                case GamepadButtons.DPadUp:
                    return "DPU";
                case GamepadButtons.LeftShoulder:
                    return "LS";
                case GamepadButtons.LeftThumb:
                    return "LT";
                case GamepadButtons.RightShoulder:
                    return "RS";
                case GamepadButtons.RightThumb:
                    return "RT";
            }
            if (Data.LeftTrigger > 0)
            {
                return "+LT";
            }
            if (Data.RightTrigger > 0)
            {
                return "+RT";
            }
            if (Data.LeftThumbX > 0)
            {
                return "+LTX";
            }
            if (Data.LeftThumbX < 0)
            {
                return "-LTX";
            }
            if (Data.LeftThumbY > 0)
            {
                return "+LTY";
            }
            if (Data.LeftThumbY < 0)
            {
                return "-LTY";
            }
            if (Data.RightThumbX > 0)
            {
                return "+RTX";
            }
            if (Data.RightThumbX < 0)
            {
                return "-RTX";
            }
            if (Data.RightThumbY > 0)
            {
                return "+RTY";
            }
            if (Data.RightThumbY < 0)
            {
                return "-RTY";
            }
            return default;
        }

        public static bool operator ==(HwXInput left, HwXInput right) => left.Equals(right);

        public static bool operator !=(HwXInput left, HwXInput right) => !(left == right);
    }
}