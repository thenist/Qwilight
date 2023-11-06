using System.Security.Cryptography;

namespace Qwilight.XOR
{
    public struct XORFloat64
    {
        long _value;
        int _salt;

        XORFloat64(double value)
        {
            _salt = RandomNumberGenerator.GetInt32(int.MaxValue);
            _value = BitConverter.DoubleToInt64Bits(value) ^ _salt;
        }

        double GetValue() => BitConverter.Int64BitsToDouble(_value ^ _salt);

        public static implicit operator double(XORFloat64 value) => value.GetValue();

        public static implicit operator XORFloat64(double value) => new XORFloat64(value);
    }
}
