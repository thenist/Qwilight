namespace Qwilight
{
    public class Elapsable<T>
    {
        public T Value { get; set; }

        public double Elapsed { get; set; }

        public void Elapse(double millis) => Elapsed += millis;

        public bool IsElapsed(double millis) => Elapsed >= millis;

        public static Elapsable<T> GetElapsable(T value) => new Elapsable<T>
        {
            Value = value
        };
    }
}
