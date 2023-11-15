using Google.Protobuf;
using Microsoft.Graphics.Canvas;
using Qwilight.ViewModel;
using Steamworks;
using System.Buffers;
using System.IO;
using Windows.Graphics.DirectX;

namespace Qwilight
{
    public sealed class ValveSystem : IDisposable
    {
        static readonly string FaultEntryPath = Path.Combine(QwilightComponent.FaultEntryPath, nameof(ValveSystem));

        public static readonly ValveSystem Instance = new();

        public string ValveName { get; set; }

        public ByteString ValveDrawing { get; set; }

        public async ValueTask Init()
        {
            if (QwilightComponent.IsValve)
            {
                try
                {
                    SteamClient.Init(QwilightComponent.AssetsClientJSON.valve);
                    SteamScreenshots.Hooked = true;
                    SteamScreenshots.OnScreenshotRequested += ViewModels.Instance.MainValue.HandleF12;
                    ValveName = SteamClient.Name;
                    var largeAvatarDrawing = await SteamFriends.GetLargeAvatarAsync(SteamClient.SteamId).ConfigureAwait(false);
                    if (largeAvatarDrawing.HasValue)
                    {
                        var largeAvatarDrawingValue = largeAvatarDrawing.Value;
                        var largeAvatarLength = largeAvatarDrawingValue.Width;
                        var largeAvatarHeight = largeAvatarDrawingValue.Height;
                        var data = largeAvatarDrawing.Value.Data;
                        var row = 4 * largeAvatarLength;
                        var length = (int)(row * largeAvatarHeight);
                        var target = ArrayPool<byte>.Shared.Rent(length);
                        try
                        {
                            for (var i = 0; i < largeAvatarHeight; ++i)
                            {
                                for (var j = 0; j < largeAvatarLength; ++j)
                                {
                                    var m = row * i + 4 * j;
                                    target[m] = data[m + 2];
                                    target[m + 1] = data[m + 1];
                                    target[m + 2] = data[m];
                                    target[m + 3] = data[m + 3];
                                }
                            }
                            using (var avatarDrawing = CanvasBitmap.CreateFromBytes(CanvasDevice.GetSharedDevice(), target, (int)largeAvatarLength, (int)largeAvatarHeight, DirectXPixelFormat.B8G8R8A8UIntNormalized))
                            using (var rms = PoolSystem.Instance.GetDataFlow())
                            {
                                await avatarDrawing.SaveAsync(rms.AsRandomAccessStream(), CanvasBitmapFileFormat.Png);
                                rms.Position = 0;
                                ValveDrawing = ByteString.CopyFrom(rms.GetBuffer(), 0, (int)rms.Length);
                            }
                        }
                        finally
                        {
                            ArrayPool<byte>.Shared.Return(target);
                        }
                    }
                    else
                    {
                        ValveDrawing = ByteString.Empty;
                    }
                }
                catch (Exception e)
                {
                    Environment.Exit(e.HResult);
                }
            }
        }

        public void Dispose()
        {
            if (QwilightComponent.IsValve)
            {
                SteamClient.Shutdown();
            }
        }
    }
}