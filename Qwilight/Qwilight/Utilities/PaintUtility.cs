using Microsoft.Graphics.Canvas;
using Microsoft.Graphics.Canvas.Brushes;
using Microsoft.Graphics.Canvas.Text;
using Microsoft.UI;
using Windows.Foundation;
using Windows.UI;
using DrawingContext = System.Windows.Media.DrawingContext;
using FormattedText = System.Windows.Media.FormattedText;
using ImageSource = System.Windows.Media.ImageSource;

namespace Qwilight.Utilities
{
    public static partial class Utility
    {
        public static void PaintDrawing(this CanvasDrawingSession targetSession, ref Bound r, ref Bound s, DrawingItem? drawingItem, float faint = 1F, CanvasComposite drawingComposition = CanvasComposite.SourceOver)
        {
            if (drawingItem.HasValue && r.CanPaint && faint > 0F)
            {
                var drawingItemValue = drawingItem.Value;
                targetSession.DrawImage(drawingItemValue.Drawing, (Rect)r, (Rect)s, faint, CanvasImageInterpolation.Linear, drawingComposition);
            }
        }

        public static void PaintDrawing(this CanvasDrawingSession targetSession, ref Bound r, DrawingItem? drawingItem, float faint = 1F, CanvasComposite drawingComposition = CanvasComposite.SourceOver)
        {
            if (drawingItem.HasValue && r.CanPaint && faint > 0F)
            {
                var drawingItemValue = drawingItem.Value;
                targetSession.DrawImage(drawingItemValue.Drawing, (Rect)r, drawingItemValue.DrawingBound, faint, CanvasImageInterpolation.Linear, drawingComposition);
            }
        }

        public static void PaintDrawing(this CanvasDrawingSession targetSession, ref Bound r, CanvasBitmap drawing, float faint = 1F)
        {
            if (drawing != null && r.CanPaint && faint > 0F)
            {
                targetSession.DrawImage(drawing, (Rect)r, drawing.Bounds, faint);
            }
        }

        public static void PaintText(this CanvasDrawingSession targetSession, CanvasTextLayout textItem, ref Bound r, Color textColor)
        {
            if (textItem != null)
            {
                targetSession.DrawTextLayout(textItem, r, textColor);
            }
        }

        public static void PaintVisibleText(this CanvasDrawingSession targetSession, CanvasTextLayout textItem, ref Bound r, Color textColor)
        {
            if (textItem != null)
            {
                r.Position0 += 1.0;
                r.Position1 += 1.0;
                targetSession.PaintText(textItem, ref r, Colors.Black);
                r.Position0 -= 1.0;
                r.Position1 -= 1.0;
                targetSession.PaintText(textItem, ref r, textColor);
            }
        }

        public static void PaintText(this CanvasDrawingSession targetSession, CanvasTextLayout textItem, ref Bound r, ICanvasBrush textPaint)
        {
            if (textItem != null)
            {
                targetSession.DrawTextLayout(textItem, r, textPaint);
            }
        }

        public static void PaintVisibleText(this CanvasDrawingSession targetSession, CanvasTextLayout textItem, ref Bound r, ICanvasBrush textPaint, ICanvasBrush textVisiblePaint)
        {
            if (textItem != null)
            {
                r.Position0 += 1.0;
                r.Position1 += 1.0;
                targetSession.PaintText(textItem, ref r, textVisiblePaint);
                r.Position0 -= 1.0;
                r.Position1 -= 1.0;
                targetSession.PaintText(textItem, ref r, textPaint);
            }
        }

        public static void PaintText(this DrawingContext targetSession, FormattedText textItem, ref Bound r)
        {
            if (textItem != null)
            {
                targetSession.DrawText(textItem, r);
            }
        }

        public static void PaintVisibleText(this DrawingContext targetSession, FormattedText textItem, FormattedText textVisibleItem, ref Bound r)
        {
            if (textItem != null)
            {
                r.Position0 += 1.0;
                r.Position1 += 1.0;
                targetSession.PaintText(textVisibleItem, ref r);
                r.Position0 -= 1.0;
                r.Position1 -= 1.0;
                targetSession.PaintText(textItem, ref r);
            }
        }

        public static void PaintDrawing(this DrawingContext targetSession, ref Bound r, ImageSource drawing)
        {
            if (drawing != null && r.CanPaint)
            {
                targetSession.DrawImage(drawing, r);
            }
        }
    }
}