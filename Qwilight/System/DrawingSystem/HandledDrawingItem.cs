using System.Windows.Media;

namespace Qwilight
{
    public struct HandledDrawingItem : IHandledItem, IDisposable
    {
        public double Length => double.PositiveInfinity;

        public bool IsLooping => true;

        public DrawingItem? Drawing { get; init; }

        public ImageSource DefaultDrawing { get; init; }

        public IHandlerItem Handle(IMediaHandler mediaHandler, TimeSpan levyingWait, MediaNote.Mode mode) => new DrawingHandlerItem
        {
            HandledDrawingItem = this
        };

        public void Dispose()
        {
            Drawing?.Dispose();
        }
    }
}