using FMOD;

namespace Qwilight
{
    public sealed class AudioHandlerItem
    {
        public Channel Channel { get; init; }

        public uint? LevyingPosition { get; init; }

        public uint Position { get; set; }

        public uint? Length { get; init; }

        public ulong AudioStandardUnit { get; init; }
    }
}