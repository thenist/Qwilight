using FMOD;

namespace Qwilight
{
    public sealed class AudioHandlerItem
    {
        public Channel Channel { get; set; }

        public uint? LevyingPosition { get; set; }

        public uint Position { get; set; }

        public uint? Length { get; set; }

        public ulong AudioStandardUnit { get; set; }
    }
}