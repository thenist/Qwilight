using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class HandledLevelIDItem
    {
        public Brush PointedPaint => Paints.DefaultPointedPaint;

        public string LevelID { get; init; }
    }
}
