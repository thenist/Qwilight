using NAudio.CoreAudioApi;

namespace Qwilight.UIComponent
{
    public struct WaveValue : IEquatable<WaveValue>
    {
        public MMDevice System { get; init; }

        public string Name { get; init; }

        public override bool Equals(object obj) => obj is WaveValue waveValue && Equals(waveValue);

        public bool Equals(WaveValue other) => System == other.System;

        public override int GetHashCode() => System.GetHashCode();

        public override string ToString() => Name;

        public static bool operator ==(WaveValue left, WaveValue right) => left.Equals(right);

        public static bool operator !=(WaveValue left, WaveValue right) => !(left == right);
    }
}
