using Vortice.DirectInput;

namespace Qwilight
{
    public struct HwDInput : IEquatable<HwDInput>
    {
        public JoystickOffset? Data { get; set; }

        public int Value { get; set; }

        public string HwID { get; set; }

        public override bool Equals(object obj) => obj is HwDInput hwDInput && Equals(hwDInput);

        public bool Equals(HwDInput other) => Data == other.Data && Value == other.Value && HwID == other.HwID;

        public override int GetHashCode() => HashCode.Combine(Data, Value, HwID);

        public override string ToString()
        {
            switch (Data)
            {
                case JoystickOffset.X:
                case JoystickOffset.RotationX:
                case JoystickOffset.VelocityX:
                case JoystickOffset.AngularVelocityX:
                case JoystickOffset.AccelerationX:
                case JoystickOffset.AngularAccelerationX:
                case JoystickOffset.ForceX:
                case JoystickOffset.TorqueX:
                case JoystickOffset.Y:
                case JoystickOffset.RotationY:
                case JoystickOffset.VelocityY:
                case JoystickOffset.AngularVelocityY:
                case JoystickOffset.AccelerationY:
                case JoystickOffset.AngularAccelerationY:
                case JoystickOffset.ForceY:
                case JoystickOffset.TorqueY:
                case JoystickOffset.Z:
                case JoystickOffset.RotationZ:
                case JoystickOffset.VelocityZ:
                case JoystickOffset.AngularVelocityZ:
                case JoystickOffset.AccelerationZ:
                case JoystickOffset.AngularAccelerationZ:
                case JoystickOffset.ForceZ:
                case JoystickOffset.TorqueZ:
                    var sign = this.Value > 32767 ? "+" : this.Value < 32511 ? "-" : null;
                    if (sign != null)
                    {
                        switch (Data)
                        {
                            case JoystickOffset.X:
                            case JoystickOffset.Y:
                            case JoystickOffset.Z:
                                return $"{sign}{Data}";
                            case JoystickOffset.RotationX:
                                return $"{sign}RX";
                            case JoystickOffset.VelocityX:
                                return $"{sign}VX";
                            case JoystickOffset.AngularVelocityX:
                                return $"{sign}AVX";
                            case JoystickOffset.AccelerationX:
                                return $"{sign}AX";
                            case JoystickOffset.AngularAccelerationX:
                                return $"{sign}AAX";
                            case JoystickOffset.ForceX:
                                return $"{sign}FX";
                            case JoystickOffset.TorqueX:
                                return $"{sign}TX";
                            case JoystickOffset.RotationY:
                                return $"{sign}RY";
                            case JoystickOffset.VelocityY:
                                return $"{sign}VY";
                            case JoystickOffset.AngularVelocityY:
                                return $"{sign}AVY";
                            case JoystickOffset.AccelerationY:
                                return $"{sign}AY";
                            case JoystickOffset.AngularAccelerationY:
                                return $"{sign}AAY";
                            case JoystickOffset.ForceY:
                                return $"{sign}FY";
                            case JoystickOffset.TorqueY:
                                return $"{sign}TY";
                            case JoystickOffset.RotationZ:
                                return $"{sign}RZ";
                            case JoystickOffset.VelocityZ:
                                return $"{sign}VZ";
                            case JoystickOffset.AngularVelocityZ:
                                return $"{sign}AVZ";
                            case JoystickOffset.AccelerationZ:
                                return $"{sign}AZ";
                            case JoystickOffset.AngularAccelerationZ:
                                return $"{sign}AAZ";
                            case JoystickOffset.ForceZ:
                                return $"{sign}FZ";
                            case JoystickOffset.TorqueZ:
                                return $"{sign}TZ";
                        }
                    }
                    break;
                case JoystickOffset.PointOfViewControllers0:
                case JoystickOffset.PointOfViewControllers1:
                case JoystickOffset.PointOfViewControllers2:
                case JoystickOffset.PointOfViewControllers3:
                    var value = this.Value / 9000;
                    switch (Data)
                    {
                        case JoystickOffset.PointOfViewControllers0:
                            return $"P0{value}";
                        case JoystickOffset.PointOfViewControllers1:
                            return $"P1{value}";
                        case JoystickOffset.PointOfViewControllers2:
                            return $"P2{value}";
                        case JoystickOffset.PointOfViewControllers3:
                            return $"P3{value}";
                    }
                    break;
                case JoystickOffset.Sliders0:
                case JoystickOffset.Sliders1:
                case JoystickOffset.ForceSliders0:
                case JoystickOffset.ForceSliders1:
                case null:
                    break;
                default:
                    return $"B{Data - JoystickOffset.Buttons0}";
            }
            return string.Empty;
        }

        public static bool operator ==(HwDInput left, HwDInput right) => left.Equals(right);

        public static bool operator !=(HwDInput left, HwDInput right) => !(left == right);
    }
}