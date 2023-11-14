using Windows.Gaming.Input;

namespace Qwilight
{
    public struct WGI : IEquatable<WGI>
    {
        public GamepadReading Data { get; set; }

        public override bool Equals(object obj) => obj is WGI mWGI && Equals(mWGI);

        public bool Equals(WGI other)
        {
            return Data.Buttons == other.Data.Buttons &&
                Data.LeftTrigger == other.Data.LeftTrigger &&
                Data.RightTrigger == other.Data.RightTrigger &&
                Data.LeftThumbstickX == other.Data.LeftThumbstickX &&
                Data.LeftThumbstickY == other.Data.LeftThumbstickY &&
                Data.RightThumbstickX == other.Data.RightThumbstickX &&
                Data.RightThumbstickY == other.Data.RightThumbstickY;
        }

        public override int GetHashCode() => HashCode.Combine(Data.Buttons, Data.LeftTrigger, Data.RightTrigger, Data.LeftThumbstickX, Data.LeftThumbstickY, Data.RightThumbstickX, Data.RightThumbstickY);

        public override string ToString()
        {
            switch (Data.Buttons)
            {
                case GamepadButtons.A:
                case GamepadButtons.B:
                case GamepadButtons.Menu:
                case GamepadButtons.View:
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
                case GamepadButtons.LeftThumbstick:
                    return "LT";
                case GamepadButtons.RightShoulder:
                    return "RS";
                case GamepadButtons.RightThumbstick:
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
            if (Data.LeftThumbstickX > 0)
            {
                return "+LTX";
            }
            if (Data.LeftThumbstickX < 0)
            {
                return "-LTX";
            }
            if (Data.LeftThumbstickY > 0)
            {
                return "+LTY";
            }
            if (Data.LeftThumbstickY < 0)
            {
                return "-LTY";
            }
            if (Data.RightThumbstickX > 0)
            {
                return "+RTX";
            }
            if (Data.RightThumbstickX < 0)
            {
                return "-RTX";
            }
            if (Data.RightThumbstickY > 0)
            {
                return "+RTY";
            }
            if (Data.RightThumbstickY < 0)
            {
                return "-RTY";
            }
            return default;
        }

        public static bool operator ==(WGI left, WGI right) => left.Equals(right);

        public static bool operator !=(WGI left, WGI right) => !(left == right);
    }
}