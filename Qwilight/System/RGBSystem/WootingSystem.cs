using Microsoft.UI;
using Qwilight.Utilities;
using RGB.NET.Core;
using RGB.NET.Devices.Wooting;
using System.IO;
using Color = Windows.UI.Color;

namespace Qwilight
{
    public sealed class WootingSystem : DefaultRGBSystem
    {
        public static readonly WootingSystem Instance = new();

        WootingSystem()
        {
#if X64
            Utility.CopyFile(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "wooting-rgb-sdk64.dll"), Path.Combine(AppContext.BaseDirectory, "wooting-rgb-sdk64.dll"));
#endif
        }

        public override void Load(RGBSurface rgbSystem)
        {
            rgbSystem.Load(WootingDeviceProvider.Instance);
        }

        public override bool IsAvailable => Configure.Instance.Wooting;

        public override Color GetMeterColor() => Colors.Yellow;

        public override void Toggle()
        {
            Configure.Instance.Wooting = !Configure.Instance.Wooting;
            base.Toggle();
        }
    }
}
