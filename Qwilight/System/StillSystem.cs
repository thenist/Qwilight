using Microsoft.Graphics.Canvas;
using Microsoft.UI;
using Qwilight.Utilities;
using System.IO;
using Windows.ApplicationModel.DataTransfer;
using Windows.Graphics;
using Windows.Graphics.Capture;
using Windows.Graphics.DirectX;
using Windows.Storage;
using Windows.Storage.Streams;
using WindowId = Windows.UI.WindowId;

namespace Qwilight
{
    public sealed class StillSystem
    {
        public static readonly StillSystem Instance = new();
        public static readonly string EntryPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyPictures), "Qwilight");

        readonly CanvasDevice _targetSystem = new();
        GraphicsCaptureItem _window;
        SizeInt32 _lastView;

        public void Init(nint handle)
        {
            try
            {
                _window = GraphicsCaptureItem.TryCreateFromWindowId(new WindowId(Win32Interop.GetWindowIdFromWindow(handle).Value));
                _lastView = _window.Size;
            }
            catch
            {
            }
        }

        public void Save()
        {
            if (_window != null)
            {
                UIHandler.Instance.HandleParallel(() =>
                {
                    GraphicsCaptureSession session = null;
                    var framePool = Direct3D11CaptureFramePool.Create(_targetSystem, DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, _window.Size);
                    framePool.FrameArrived += (sender, args) => UIHandler.Instance.HandleParallel(async () =>
                    {
                        CanvasBitmap drawing;
                        try
                        {
                            using (var frame = sender.TryGetNextFrame())
                            {
                                if (frame.ContentSize != _lastView)
                                {
                                    _lastView = frame.ContentSize;
                                    sender.Recreate(_targetSystem, DirectXPixelFormat.B8G8R8A8UIntNormalized, 2, _lastView);
                                }
                                drawing = CanvasBitmap.CreateFromDirect3D11Surface(_targetSystem, frame.Surface);
                            }
                        }
                        finally
                        {
                            session.Dispose();
                            sender.Dispose();
                        }
                        try
                        {
                            Directory.CreateDirectory(EntryPath);
                            var pngFilePath = Path.Combine(EntryPath, $"{DateTime.Now:F}.png".Replace(':', ' '));
                            await drawing.SaveAsync(pngFilePath, CanvasBitmapFileFormat.Png);
                            var dataBundle = new DataPackage();
                            dataBundle.SetBitmap(RandomAccessStreamReference.CreateFromFile(await StorageFile.GetFileFromPathAsync(pngFilePath)));
                            Clipboard.SetContent(dataBundle);
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Info, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.SavedStillDrawing, pngFilePath), true, "F12", () => Utility.OpenAs(pngFilePath));
                        }
                        catch (IOException e)
                        {
                            NotifySystem.Instance.Notify(NotifySystem.NotifyVariety.Warning, NotifySystem.NotifyConfigure.Default, string.Format(LanguageSystem.Instance.FaultStillDrawing, e.Message));
                        }
                    });
                    session = framePool.CreateCaptureSession(_window);
                    session.StartCapture();
                });
            }
        }
    }
}
