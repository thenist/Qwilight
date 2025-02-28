﻿using Microsoft.UI;
using Qwilight.Utilities;
using RGB.NET.Core;
using RGB.NET.Devices.CoolerMaster;
using RGB.NET.Devices.Corsair;
using RGB.NET.Devices.SteelSeries;
using System.IO;
using Color = Windows.UI.Color;

namespace Qwilight
{
    public sealed class K70System : DefaultRGBSystem
    {
        public static readonly K70System Instance = new();

        public override IRGBDeviceProvider System { get; } = CorsairDeviceProvider.Instance;

        K70System()
        {
#if X64
            Utility.CopyFile(Path.Combine(QwilightComponent.CPUAssetsEntryPath, "iCUESDK.x64_2019.dll"), Path.Combine(AppContext.BaseDirectory, "x64", "iCUESDK.x64_2019.dll"));
#endif
        }

        public override bool IsAvailable => Configure.Instance.K70;

        public override Color GetGroupColor() => Colors.Red;

        public override void Toggle()
        {
            Configure.Instance.K70 = !Configure.Instance.K70;
            base.Toggle();
        }
    }
}
