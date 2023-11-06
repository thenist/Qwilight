namespace Qwilight
{
    public sealed class MoveValue<T>
    {
        public T TargetValue { get; set; }

        public T Value { get; set; }

        public MoveValue(T defaultValue = default)
        {
            TargetValue = defaultValue;
            Value = defaultValue;
        }
    }
}
