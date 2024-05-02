using Microsoft.UI;
using Windows.UI;

namespace Qwilight
{
    public sealed class WootingSystem : DefaultRGBSystem
    {
        public static readonly WootingSystem Instance = new();

        public override bool IsAvailable => Configure.Instance.Wooting;

        public override Color GetMeterColor() => Colors.Yellow;

        public override void Toggle()
        {
            Configure.Instance.Wooting = !Configure.Instance.Wooting;
            base.Toggle();
        }
    }
}
