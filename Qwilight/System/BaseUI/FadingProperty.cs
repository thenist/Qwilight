namespace Qwilight
{
    public sealed class FadingProperty
    {
        public int Frame { get; set; }

        public double Framerate { get; set; }

        public double Millis { get; set; }

        public double DrawingStatus { get; set; }

        public HandledDrawingItem?[] HandledDrawingItems { get; set; }
    }
}