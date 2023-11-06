using Windows.Devices.Midi;

namespace Qwilight
{
    public struct MIDI : IEquatable<MIDI>
    {
        public MidiMessageType Data { get; set; }

        public byte Value { get; set; }

        public override bool Equals(object obj) => obj is MIDI mMIDI && Equals(mMIDI);

        public bool Equals(MIDI other) => Data == other.Data && Value == other.Value;

        public override int GetHashCode() => HashCode.Combine(Data, Value);

        public override string ToString() => Data != MidiMessageType.None ? $"{Data} {Value}" : string.Empty;

        public static bool operator ==(MIDI left, MIDI right) => left.Equals(right);

        public static bool operator !=(MIDI left, MIDI right) => !(left == right);
    }
}