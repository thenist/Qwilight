namespace Qwilight
{
    public sealed class MediaNote
    {
        public enum Mode
        {
            Default,
            Layer,
            Failed
        }

        public Mode MediaMode { get; set; }

        public IHandledItem MediaItem { get; set; }

        public bool HasContents { get; set; }

        public TimeSpan MediaLevyingPosition { get; set; }

        public double Length { get; set; }
    }
}
