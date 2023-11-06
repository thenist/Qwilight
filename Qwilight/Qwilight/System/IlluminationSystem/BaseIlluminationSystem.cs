using Windows.System;
using Windows.UI;

namespace Qwilight
{
    public abstract class BaseIlluminationSystem : IDisposable
    {
        public abstract bool IsAvailable { get; }

        public abstract bool Init();

        public bool IsHandling { get; set; }

        public abstract void SetInputColor(VirtualKey input, uint value);

        public abstract void SetStatusColors(double status, uint value0, uint value1, uint value2, uint value3);

        public abstract void SetEtcColor(uint value);

        public abstract void OnBeforeHandle();

        public abstract void OnHandled();

        public abstract Color GetMeterColor();

        public abstract void Dispose();

        public virtual void Toggle()
        {
            if (IsAvailable)
            {
                IlluminationSystem.Instance.HandleIfAvailable();
            }
            else
            {
                Dispose();
            }
        }
    }
}
