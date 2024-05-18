using Microsoft.UI;
using RGB.NET.Core;
using RGB.NET.Devices.CoolerMaster;
using RGB.NET.Devices.Corsair;
using RGB.NET.Devices.SteelSeries;
using Color = Windows.UI.Color;

namespace Qwilight
{
    public sealed class GSSystem : DefaultRGBSystem<SteelSeriesDeviceProvider>
    {
        public static readonly GSSystem Instance = new();

        public override bool IsAvailable => Configure.Instance.GS;

        public override Color GetGroupColor() => Colors.White;

        public override void Toggle()
        {
            Configure.Instance.GS = !Configure.Instance.GS;
            base.Toggle();
        }
    }
}
