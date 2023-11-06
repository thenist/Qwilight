namespace Qwilight.UIComponent
{
    public struct AudioValue : IEquatable<AudioValue>
    {
        public int ID { get; init; }

        public string Name { get; init; }

        public override bool Equals(object obj) => obj is AudioValue audioValue && Equals(audioValue);

        public bool Equals(AudioValue other) => ID == other.ID;

        public override int GetHashCode() => ID.GetHashCode();

        public override string ToString() => Name;

        public static bool operator ==(AudioValue left, AudioValue right) => left.Equals(right);

        public static bool operator !=(AudioValue left, AudioValue right) => !(left == right);
    }
}