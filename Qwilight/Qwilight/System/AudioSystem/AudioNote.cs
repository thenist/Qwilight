namespace Qwilight
{
    public struct AudioNote
    {
        public AudioItem? AudioItem { get; set; }

        public uint? Length { get; set; }

        public uint AudioLevyingPosition { get; set; }

        public int Salt { get; set; }
    }
}
