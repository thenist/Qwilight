using Microsoft.UI;
using RGB.NET.Core;
using RGB.NET.Devices.SteelSeries;
using Color = Windows.UI.Color;

namespace Qwilight
{
    public sealed class GSSystem : DefaultRGBSystem
    {
        public static readonly GSSystem Instance = new();

        public override void Load(RGBSurface rgbSystem)
        {
            rgbSystem.Load(SteelSeriesDeviceProvider.Instance);
        }

        public override bool IsAvailable => Configure.Instance.GS;

        public override Color GetMeterColor() => Colors.White;

        public override void Toggle()
        {
            Configure.Instance.GS = !Configure.Instance.GS;
            base.Toggle();
        }
    }
}
