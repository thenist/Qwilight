using Windows.System;
using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight
{
    public abstract class BaseRGBSystem : IDisposable
    {
        public BaseRGBSystem()
        {
            GroupPaint = new Lazy<Brush>(() => DrawingSystem.Instance.GetDefaultPaint(GetGroupColor()));
        }

        public abstract bool IsAvailable { get; }

        public abstract bool Init();

        public bool IsHandling { get; set; }

        public abstract void SetInputColor(VirtualKey input, uint value);

        public abstract void SetStatusColors(double status, uint value0, uint value1, uint value2, uint value3);

        public abstract void SetEtcColor(uint value);

        public abstract void OnBeforeHandle();

        public abstract void OnHandled();

        public abstract Color GetGroupColor();

        public Lazy<Brush> GroupPaint { get; }

        public abstract void Dispose();

        public virtual void Toggle()
        {
            if (IsAvailable)
            {
                RGBSystem.Instance.HandleIfAvailable();
            }
            else
            {
                Dispose();
            }
        }
    }
}
