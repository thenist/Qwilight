using FFmpegInteropX;
using Windows.Media.Playback;

namespace Qwilight
{
    public struct HandledMediaItem : IHandledItem, IDisposable
    {
        public FFmpegMediaSource MediaSrc { get; init; }

        public MediaPlayer Media { get; init; }

        public System.Windows.Media.MediaPlayer DefaultMedia { get; init; }

        public double Length { get; init; }

        public IHandlerItem Handle(IMediaHandler mediaHandler, TimeSpan levyingWait, MediaNote.Mode mode, bool isLooping) => MediaSystem.Instance.Handle(this, mediaHandler, levyingWait, mode, isLooping);

        public void Dispose()
        {
            Media.Dispose();
            HandlingUISystem.Instance.HandleParallel(DefaultMedia.Close);
        }
    }
}