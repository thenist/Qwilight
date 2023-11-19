namespace Qwilight
{
    public struct AudioNote
    {
        public AudioItem? AudioItem { get; init; }

        public uint? Length { get; init; }

        public uint AudioLevyingPosition { get; init; }

        public int Salt { get; init; }
    }
}
