using Microsoft.UI;
using Windows.UI;

namespace Qwilight
{
    public sealed class GSSystem : DefaultRGBSystem
    {
        public static readonly GSSystem Instance = new();

        public override bool IsAvailable => Configure.Instance.GS;

        public override Color GetMeterColor() => Colors.White;

        public override void Toggle()
        {
            Configure.Instance.GS = !Configure.Instance.GS;
            base.Toggle();
        }
    }
}
