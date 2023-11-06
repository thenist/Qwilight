using System.Security.Cryptography;

namespace Qwilight.XOR
{
    public struct XORInt32
    {
        int _value;
        int _salt;

        XORInt32(int value)
        {
            _salt = RandomNumberGenerator.GetInt32(int.MaxValue);
            _value = value ^ _salt;
        }

        int GetValue() => _value ^ _salt;

        public static implicit operator int(XORInt32 value) => value.GetValue();

        public static implicit operator XORInt32(int value) => new XORInt32(value);
    }
}
