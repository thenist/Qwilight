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
    public sealed class MSISystem : DefaultRGBSystem<MsiDeviceProvider>
    {
        public static readonly MSISystem Instance = new();

        MSISystem()
        {
#if X64
            Utility.CopyFile(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "MysticLight_SDK_x64.dll"), Path.Combine(AppContext.BaseDirectory, "x64", "MysticLight_SDK.dll"));
#endif
        }

        public override bool IsAvailable => Configure.Instance.MSI;

        public override Color GetGroupColor() => Colors.Red;

        public override void Toggle()
        {
            Configure.Instance.MSI = !Configure.Instance.MSI;
            base.Toggle();
        }
    }
}
