using Microsoft.UI;
using Qwilight.Utilities;
using RGB.NET.Core;
using RGB.NET.Devices.CoolerMaster;
using RGB.NET.Devices.Corsair;
using RGB.NET.Devices.Msi;
using System.IO;
using Color = Windows.UI.Color;

namespace Qwilight
{
    public sealed class CMSystem : DefaultRGBSystem<CoolerMasterDeviceProvider>
    {
        public static readonly CMSystem Instance = new();

        CMSystem()
        {
#if X64
            Utility.CopyFile(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "SDKDLL.dll"), Path.Combine(AppContext.BaseDirectory, "x64", "CMSDK.dll"));
#endif
        }

        public override bool IsAvailable => Configure.Instance.CM;

        public override Color GetGroupColor() => Colors.Purple;

        public override void Toggle()
        {
            Configure.Instance.CM = !Configure.Instance.CM;
            base.Toggle();
        }
    }
}
