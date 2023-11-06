using Microsoft.UI;
using Qwilight.Utilities;
using Windows.UI;
using Brush = System.Windows.Media.Brush;
using Pen = System.Windows.Media.Pen;

namespace Qwilight
{
    public static class Paints
    {
        public static readonly Brush[] NetSiteCommentPaints = new Brush[]
        {
            null,
            DrawingSystem.Instance.GetDefaultPaint(Colors.Green),
            DrawingSystem.Instance.GetDefaultPaint(Colors.Red)
        };
        public static readonly Brush[] LowerStandPaints = new Brush[]
        {
            DrawingSystem.Instance.GetDefaultPaint(Colors.Green),
            DrawingSystem.Instance.GetDefaultPaint(Colors.Yellow),
            DrawingSystem.Instance.GetDefaultPaint(Colors.Red)
        };
        public static readonly Brush ModalPaint = DrawingSystem.Instance.GetDefaultPaint(Colors.Black, 50);
        public static readonly Brush[] PointPaints = new Brush[]
        {
            QwilightComponent.GetBuiltInData<Brush>("NotPointedPaint"),
            QwilightComponent.GetBuiltInData<Brush>("PointedPaint")
        };
        public static readonly Color[] PointColors = new Color[]
        {
            Utility.ModifyColor(QwilightComponent.GetBuiltInData<System.Windows.Media.Color>("NotPointedColor")),
            Utility.ModifyColor(QwilightComponent.GetBuiltInData<System.Windows.Media.Color>("PointedColor"))
        };
        public static readonly Brush PaintFaint = DrawingSystem.Instance.GetDefaultPaint(Colors.Transparent);
        public static readonly Brush Paint0 = DrawingSystem.Instance.GetDefaultPaint(Colors.Black);
        public static readonly Brush Paint1 = DrawingSystem.Instance.GetDefaultPaint(Colors.Red);
        public static readonly Brush Paint2 = DrawingSystem.Instance.GetDefaultPaint(Colors.Yellow);
        public static readonly Brush Paint3 = DrawingSystem.Instance.GetDefaultPaint(Colors.Green);
        public static readonly Brush Paint4 = DrawingSystem.Instance.GetDefaultPaint(Colors.White);
        public static readonly Brush InputPaint1 = DrawingSystem.Instance.GetDefaultPaint(Colors.Red);
        public static readonly Brush InputPaint2 = DrawingSystem.Instance.GetDefaultPaint(Colors.White);
        public static readonly Brush InputPaint3 = DrawingSystem.Instance.GetDefaultPaint(Colors.Cyan);
        public static readonly Color ColorOK = Colors.DarkGreen;
        public static readonly Color ColorInfo = Colors.DarkBlue;
        public static readonly Color ColorFault = Colors.DarkRed;
        public static readonly Color ColorWarning = Colors.DarkGoldenrod;
        public static readonly Brush PaintOK = DrawingSystem.Instance.GetDefaultPaint(Colors.DarkGreen);
        public static readonly Brush PaintInfo = DrawingSystem.Instance.GetDefaultPaint(Colors.DarkBlue);
        public static readonly Brush PaintFault = DrawingSystem.Instance.GetDefaultPaint(Colors.DarkRed);
        public static readonly Brush PaintWarning = DrawingSystem.Instance.GetDefaultPaint(Colors.DarkGoldenrod);
        public static readonly Pen Pen0 = DrawingSystem.Instance.GetPen(Paint0);
        public static readonly Pen Pen4 = DrawingSystem.Instance.GetPen(Paint4);
        public static Brush DefaultPointedPaint = DrawingSystem.Instance.GetDefaultPaint(Utility.ModifyColor(QwilightComponent.GetBuiltInData<System.Windows.Media.Color>("PointedColor")));
    }
}
