using Microsoft.Graphics.Canvas;
using Windows.Graphics.Imaging;
using Windows.Media.Playback;

namespace Qwilight
{
    public sealed class MediaHandlerItem : IHandlerItem, IDisposable
    {
        CanvasBitmap _mediaFrame;

        public HandledMediaItem HandledMediaItem { get; set; }

        public TimeSpan LevyingPosition { get; set; }

        public MediaNote.Mode Mode { get; set; }

        public bool IsAvailable { get; set; }

        public bool IsDefaultAvailable { get; set; }

        public bool IsHandling { get; set; }

        public bool IsDefaultHandling { get; set; }

        public bool IsVisible { get; set; }

        public CanvasBitmap MediaFrame { get; set; }

        public double GetMediaPosition(IMediaHandler mediaHandler) => (mediaHandler.LoopingCounter - LevyingPosition.TotalMilliseconds) % (HandledMediaItem.IsLooping ? HandledMediaItem.Length : double.PositiveInfinity);

        public void Handle(IMediaHandler mediaHandler, double mediaPosition)
        {
            if (mediaPosition < HandledMediaItem.Length)
            {
                HandledMediaItem.Media.PlaybackSession.Position = TimeSpan.Zero;
                HandledMediaItem.Media.PlaybackSession.Position = TimeSpan.FromMilliseconds(mediaPosition);
                HandledMediaItem.Media.PlaybackSession.PlaybackRate = mediaHandler.AudioMultiplier;
                HandledMediaItem.Media.VideoFrameAvailable += OnMediaFrameAvailable;
                HandledMediaItem.Media.Play();
                IsHandling = true;
            }
        }

        public void Stop()
        {
            HandledMediaItem.Media.VideoFrameAvailable -= OnMediaFrameAvailable;
            HandledMediaItem.Media.Pause();
            IsHandling = false;
        }

        void OnMediaFrameAvailable(MediaPlayer sender, object args)
        {
            try
            {
                if (_mediaFrame == null && MediaFrame == null)
                {
                    var mediaSession = sender.PlaybackSession;
                    using var mediaFrame = new SoftwareBitmap(BitmapPixelFormat.Bgra8, (int)mediaSession.NaturalVideoWidth, (int)mediaSession.NaturalVideoHeight, BitmapAlphaMode.Ignore);
                    _mediaFrame = CanvasBitmap.CreateFromSoftwareBitmap(new CanvasDevice(), mediaFrame);
                    MediaFrame = CanvasBitmap.CreateFromSoftwareBitmap(CanvasDevice.GetSharedDevice(), mediaFrame);
                }
                sender.CopyFrameToVideoSurface(_mediaFrame);
                MediaFrame.CopyPixelsFromBitmap(_mediaFrame);
            }
            catch
            {
            }
        }

        public void Pause(bool isPaused)
        {
            if (isPaused)
            {
                HandledMediaItem.Media.Pause();
            }
            else
            {
                HandledMediaItem.Media.Play();
            }
        }

        public void SetMediaPosition(double mediaPosition)
        {
            if (Math.Abs(HandledMediaItem.Media.PlaybackSession.Position.TotalMilliseconds - mediaPosition) >= 500.0)
            {
                HandledMediaItem.Media.PlaybackSession.Position = TimeSpan.FromMilliseconds(mediaPosition);
            }
        }

        public void HandleDefault(IMediaHandler mediaHandler, double mediaPosition)
        {
            if (mediaPosition < HandledMediaItem.Length)
            {
                UIHandler.Instance.HandleParallel(() =>
                {
                    var defaultMedia = new System.Windows.Media.MediaPlayer
                    {
                        IsMuted = true
                    };
                    if (HandledMediaItem.IsLooping)
                    {
                        defaultMedia.MediaEnded += (sender, e) =>
                        {
                            defaultMedia.Position = TimeSpan.Zero;
                            defaultMedia.Play();
                        };
                    }
                    defaultMedia.Open(new(HandledMediaItem.MediaFilePath));
                    defaultMedia.Position = TimeSpan.FromMilliseconds(mediaPosition);
                    defaultMedia.SpeedRatio = mediaHandler.AudioMultiplier;
                    defaultMedia.Play();
                    HandledMediaItem.DefaultMedia = defaultMedia;
                });
                IsDefaultHandling = true;
            }
        }

        public void StopDefault()
        {
            UIHandler.Instance.HandleParallel(() => HandledMediaItem.DefaultMedia?.Stop());
            IsDefaultHandling = false;
        }

        public void PauseDefault(bool isPaused)
        {
            if (isPaused)
            {
                UIHandler.Instance.HandleParallel(() => HandledMediaItem.DefaultMedia?.Pause());
            }
            else
            {
                UIHandler.Instance.HandleParallel(() => HandledMediaItem.DefaultMedia?.Play());
            }
        }

        public void SetDefaultMediaPosition(double mediaPosition)
        {
            var defaultMedia = HandledMediaItem.DefaultMedia;
            if (defaultMedia != null)
            {
                UIHandler.Instance.HandleParallel(() =>
                {
                    if (Math.Abs(defaultMedia.Position.TotalMilliseconds - mediaPosition) >= 500.0)
                    {
                        defaultMedia.Position = TimeSpan.FromMilliseconds(mediaPosition);
                    }
                });
            }
        }

        public void Dispose()
        {
            _mediaFrame?.Dispose();
            _mediaFrame?.Device?.Dispose();
            _mediaFrame = null;
            _mediaFrame?.Dispose();
            _mediaFrame = null;
        }
    }
}