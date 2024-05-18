using Microsoft.UI;
using Qwilight.Utilities;
using RGB.NET.Core;
using RGB.NET.Devices.Corsair;
using System.IO;
using Color = Windows.UI.Color;

namespace Qwilight
{
    public sealed class K70System : DefaultRGBSystem
    {
        public static readonly K70System Instance = new();

        K70System()
        {
#if X64
            Utility.CopyFile(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "iCUESDK.x64_2019.dll"), Path.Combine(AppContext.BaseDirectory, "x64", "iCUESDK.x64_2019.dll"));
#endif
        }

        public override void Load(RGBSurface rgbSystem)
        {
            rgbSystem.Load(CorsairDeviceProvider.Instance);
        }

        public override bool IsAvailable => Configure.Instance.K70;

        public override Color GetMeterColor() => Colors.Red;

        public override void Toggle()
        {
            Configure.Instance.K70 = !Configure.Instance.K70;
            base.Toggle();
        }
    }
}
