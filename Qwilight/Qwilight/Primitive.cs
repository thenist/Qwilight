namespace Qwilight
{
    public sealed class Primitive<T>
    {
        T _value;

        public Primitive(T value)
        {
            SetValue(value);
        }

        public void SetValue(T value)
        {
            _value = value;
        }

        public static implicit operator T(Primitive<T> value)
        {
            return value._value;
        }
    }
}
