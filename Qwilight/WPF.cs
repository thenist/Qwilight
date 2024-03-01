using System.Windows.Media;

namespace Qwilight
{
    public static class WPF
    {
        static TimeSpan _time;
        static event EventHandler PaintEvent;

        public static event EventHandler Paint
        {
            add
            {
                if (PaintEvent == null)
                {
                    CompositionTarget.Rendering += OnPaint;
                }
                PaintEvent += value;
            }

            remove
            {
                PaintEvent -= value;
                if (PaintEvent == null)
                {
                    CompositionTarget.Rendering -= OnPaint;
                }
            }
        }

        static void OnPaint(object sender, EventArgs e)
        {
            var time = (e as RenderingEventArgs).RenderingTime;
            if (time > _time)
            {
                _time = time;
                PaintEvent(sender, e);
            }
        }
    }
}
