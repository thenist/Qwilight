using FFmpegInteropX;
using Windows.Media.Playback;

namespace Qwilight
{
    public sealed class HandledMediaItem : IHandledItem, IDisposable
    {
        public FFmpegMediaSource MediaSrc { get; init; }

        public MediaPlayer Media { get; init; }

        public System.Windows.Media.MediaPlayer DefaultMedia { get; set; }

        public string MediaFilePath { get; init; }

        public double Length { get; init; }

        public bool IsLooping { get; init; }

        public IHandlerItem Handle(IMediaHandler mediaHandler, TimeSpan levyingWait, MediaNote.Mode mode) => MediaSystem.Instance.Handle(this, mediaHandler, levyingWait, mode);

        public void Dispose()
        {
            Media.Dispose();
            UIHandler.Instance.HandleParallel(() => DefaultMedia?.Close());
        }
    }
}