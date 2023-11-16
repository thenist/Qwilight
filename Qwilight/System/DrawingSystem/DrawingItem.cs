using Microsoft.Graphics.Canvas;

namespace Qwilight
{
    public struct DrawingItem : IDisposable
    {
        public ICanvasImage Drawing { get; set; }

        public Bound DrawingBound { get; set; }

        public uint AverageColor { get; set; }

        public double AverageHeight { get; set; }

        public void Dispose() => Drawing.Dispose();
    }
}