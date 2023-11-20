using System.Windows.Controls;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static string GetHandledText(MediaElement view)
        {
            if (view.NaturalDuration.HasTimeSpan)
            {
                return $"{view.Position.Minutes}:{view.Position.Seconds.ToString("00")}／{view.NaturalDuration.TimeSpan.Minutes}:{view.NaturalDuration.TimeSpan.Seconds.ToString("00")}";
            }
            else
            {
                return string.Empty;
            }
        }

        public static void SetPosition(MediaElement view, double value)
        {
            if (value < 100.0)
            {
                if (view.NaturalDuration.HasTimeSpan)
                {
                    var targetPosition = view.NaturalDuration.TimeSpan * value / 100.0;
                    if (Math.Abs((view.Position - targetPosition).TotalSeconds) >= 1.0)
                    {
                        view.Position = view.NaturalDuration.TimeSpan * value / 100.0;
                    }
                }
            }
            else
            {
                view.Stop();
            }
        }
    }
}