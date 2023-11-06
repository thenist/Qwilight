using Microsoft.Graphics.Canvas;
using Qwilight.UIComponent;
using Qwilight.Utilities;
using System.Collections.ObjectModel;
using Windows.Graphics.Imaging;
using Windows.Media.Capture;
using Windows.Media.Capture.Frames;
using Windows.Media.MediaProperties;

namespace Qwilight
{
    public sealed class MediaInputSystem : Model
    {
        public static readonly MediaInputSystem Instance = QwilightComponent.GetBuiltInData<MediaInputSystem>(nameof(MediaInputSystem));

        readonly CanvasDevice _mediaFrameSystem = new CanvasDevice();
        MediaCapture _mediaInputSystem;
        CanvasBitmap _mediaFrame;
        MediaFrameReader _mediaInputComputer;
        MediaInputItem? _mediaInputItem;
        MediaInputQuality? _mediaInputQuality;

        public MediaInputItem? MediaInputItemValue
        {
            get => _mediaInputItem;

            set
            {
                if (SetProperty(ref _mediaInputItem, value, nameof(MediaInputItemValue)) && value.HasValue)
                {
                    InitMediaInput();

                    async void InitMediaInput()
                    {
                        _mediaInputComputer = await _mediaInputSystem.CreateFrameReaderAsync(_mediaInputSystem.FrameSources[value.Value.ID], MediaEncodingSubtypes.Bgra8);
                        _mediaInputComputer.FrameArrived += OnMediaFrameAvailable;
                        await _mediaInputComputer.StartAsync();
                    }
                }
            }
        }

        public MediaInputQuality? MediaInputQualityValue
        {
            get => _mediaInputQuality;

            set
            {
                if (SetProperty(ref _mediaInputQuality, value, nameof(MediaInputQualityValue)) && value.HasValue)
                {
                    SetMediaInputQuality();

                    async void SetMediaInputQuality()
                    {
                        await _mediaInputSystem.VideoDeviceController.SetMediaStreamPropertiesAsync(MediaStreamType.VideoRecord, value.Value.Data);
                    }
                }
            }
        }

        public ObservableCollection<MediaInputQuality> MediaInputQualityCollection { get; } = new();

        public ObservableCollection<MediaInputItem> MediaInputItemCollection { get; } = new();

        public CanvasBitmap MediaFrame { get; set; }

        public MediaInputSystem() => GetMediaInputItems();

        public void PaintMediaInput(CanvasDrawingSession targetSession, ref Bound r, float mediaInputFaint)
        {
            if (MediaFrame != null)
            {
                var mediaFrameBound = MediaFrame.Bounds;
                Utility.SetFilledMediaDrawing(ref r, Configure.Instance.IsMediaFill, mediaFrameBound.Width, mediaFrameBound.Height, r.Position0, r.Position1, r.Length, r.Height);
                targetSession.PaintDrawing(ref r, MediaFrame, mediaInputFaint);
            }
        }

        public async void GetMediaInputItems()
        {
            if (Configure.Instance.MediaInput)
            {
                try
                {
                    _mediaInputSystem?.Dispose();
                    _mediaInputSystem = new MediaCapture();
                    await _mediaInputSystem.InitializeAsync(new MediaCaptureInitializationSettings
                    {
                        MemoryPreference = MediaCaptureMemoryPreference.Cpu,
                        StreamingCaptureMode = StreamingCaptureMode.Video
                    });
                    Utility.SetUICollection(MediaInputItemCollection, _mediaInputSystem.FrameSources.Select(pair => new MediaInputItem
                    {
                        ID = pair.Key,
                        Name = pair.Value.Info.DeviceInformation.Name
                    }).ToArray());
                    Utility.SetUICollection(MediaInputQualityCollection, _mediaInputSystem.VideoDeviceController.GetAvailableMediaStreamProperties(MediaStreamType.VideoRecord).Cast<VideoEncodingProperties>().Select(vep => new MediaInputQuality
                    {
                        Data = vep
                    }).Order().ToArray());
                }
                catch
                {
                    MediaInputItemCollection.Clear();
                    MediaInputQualityCollection.Clear();
                }
                MediaInputItemValue = MediaInputItemCollection.Count > 0 ? MediaInputItemCollection.First() : null;
                MediaInputQualityValue = MediaInputQualityCollection.Count > 0 ? MediaInputQualityCollection.First() : null;
            }
        }

        public void CloseMediaInput()
        {
            _mediaInputComputer?.Dispose();
            _mediaInputSystem?.Dispose();
        }

        void OnMediaFrameAvailable(MediaFrameReader sender, MediaFrameArrivedEventArgs args)
        {
            try
            {
                using (var mediaFrame = sender.TryAcquireLatestFrame().VideoMediaFrame.SoftwareBitmap)
                using (var mediaFrameModified = mediaFrame.BitmapPixelFormat != BitmapPixelFormat.Bgra8 || mediaFrame.BitmapAlphaMode != BitmapAlphaMode.Ignore ? SoftwareBitmap.Convert(mediaFrame, BitmapPixelFormat.Bgra8, BitmapAlphaMode.Ignore) : mediaFrame)
                {
                    _mediaFrame?.Dispose();
                    _mediaFrame = CanvasBitmap.CreateFromSoftwareBitmap(_mediaFrameSystem, mediaFrameModified);
                    if (MediaFrame == null || MediaFrame.Bounds.Width != _mediaFrame.Bounds.Width || MediaFrame.Bounds.Height != _mediaFrame.Bounds.Height)
                    {
                        MediaFrame?.Dispose();
                        MediaFrame = CanvasBitmap.CreateFromSoftwareBitmap(CanvasDevice.GetSharedDevice(), mediaFrameModified);
                    }
                }
                MediaFrame.CopyPixelsFromBitmap(_mediaFrame);
            }
            catch
            {
            }
        }
    }
}
