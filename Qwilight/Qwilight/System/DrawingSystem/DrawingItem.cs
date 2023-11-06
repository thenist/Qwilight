using Microsoft.Graphics.Canvas;

namespace Qwilight
{
    public struct DrawingItem : IDisposable
    {
        readonly bool _disposable;

        public DrawingItem(bool disposable = true)
        {
            _disposable = disposable;
        }

        public ICanvasImage Drawing { get; set; }

        public Bound DrawingBound { get; set; }

        public uint AverageColor { get; set; }

        public double StandardHeight { get; set; }

        public void Dispose()
        {
            if (_disposable)
            {
                Drawing.Dispose();
            }
        }
    }
}