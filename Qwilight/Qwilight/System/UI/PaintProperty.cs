namespace Qwilight
{
    public sealed class PaintProperty
    {
        public enum ID
        {
            Position0, Position1, Length, Height, Alt, Frame, Framerate, Mode, Pipeline, Composition
        }

        public Dictionary<ID, string[]> ValueCallMap { get; } = new();

        public Dictionary<ID, string[]> IntCallMap { get; } = new();

        public Dictionary<ID, string[]> AltCallMap { get; } = new();

        public Dictionary<ID, double> ValueMap { get; } = new();

        public Dictionary<ID, int> IntMap { get; } = new();

        public Dictionary<ID, int> AltMap { get; } = new();

        public DrawingItem?[] Drawings { get; set; }
    }
}