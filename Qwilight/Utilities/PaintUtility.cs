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

        public static void SetFilledMediaDrawing(ref Bound r, bool isMediaFill, double mediaSoftwareLength, double mediaSoftwareHeight, double mediaPosition0, double mediaPosition1, double mediaLength, double mediaHeight)
        {
            if (isMediaFill)
            {
                r.Set(mediaPosition0, mediaPosition1, mediaLength, mediaHeight);
            }
            else
            {
                if (mediaLength / mediaSoftwareLength > mediaHeight / mediaSoftwareHeight)
                {
                    mediaSoftwareLength = mediaHeight * mediaSoftwareLength / mediaSoftwareHeight;
                    r.Set(mediaPosition0 + (mediaLength - mediaSoftwareLength) / 2, mediaPosition1, mediaSoftwareLength, mediaHeight);
                }
                else
                {
                    mediaSoftwareHeight = mediaLength * mediaSoftwareHeight / mediaSoftwareLength;
                    r.Set(mediaPosition0, mediaPosition1 + (mediaHeight - mediaSoftwareHeight) / 2, mediaLength, mediaSoftwareHeight);
                }
            }
        }

        public static void PaintAudioVisualizer(CanvasDrawingSession targetSession, ref Bound r, int audioVisualizerFaint, double audioVisualizerPosition0, double audioVisualizerPosition1, double audioVisualizerLength, double audioVisualizerHeight)
        {
            if (Configure.Instance.AudioVisualizer && audioVisualizerFaint > 0)
            {
                var audioMainVisualizerPaint = DrawingSystem.Instance.AudioVisualizerMainPaints[audioVisualizerFaint];
                var audioInputVisualizerPaint = DrawingSystem.Instance.AudioVisualizerInputPaints[audioVisualizerFaint];
                var audioVisualizerCount = Configure.Instance.AudioVisualizerCount;
                var audioVisualizerUnitLength = audioVisualizerLength / audioVisualizerCount;
                for (var i = audioVisualizerCount - 1; i >= 0; --i)
                {
                    var mainAudioVisualizerValue = audioVisualizerHeight * AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.MainAudio, i);
                    var inputAudioVisualizerValue = audioVisualizerHeight * AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.InputAudio, i);
                    if (mainAudioVisualizerValue > 0.0)
                    {
                        r.Set(audioVisualizerPosition0 + audioVisualizerUnitLength * i, audioVisualizerPosition1 + Configure.Instance.GetAudioVisualizerModifier(audioVisualizerHeight, mainAudioVisualizerValue), audioVisualizerUnitLength, mainAudioVisualizerValue);
                        targetSession.FillRectangle(r, audioMainVisualizerPaint);
                    }
                    if (inputAudioVisualizerValue > 0.0)
                    {
                        r.Set(audioVisualizerPosition0 + audioVisualizerUnitLength * i, audioVisualizerPosition1 + Configure.Instance.GetAudioVisualizerModifier(audioVisualizerHeight, inputAudioVisualizerValue), audioVisualizerUnitLength, inputAudioVisualizerValue);
                        targetSession.FillRectangle(r, audioInputVisualizerPaint);
                    }
                }
            }
        }

        public static void PaintAudioVisualizer(DrawingContext targetSession, ref Bound r, int audioVisualizerFaint, double audioVisualizerPosition0, double audioVisualizerPosition1, double audioVisualizerLength, double audioVisualizerHeight)
        {
            if (Configure.Instance.AudioVisualizer && audioVisualizerFaint > 0)
            {
                var audioMainVisualizerPaint = Configure.Instance.AudioVisualizerMainPaints[audioVisualizerFaint];
                var audioInputVisualizerPaint = Configure.Instance.AudioVisualizerInputPaints[audioVisualizerFaint];
                var audioVisualizerCount = Configure.Instance.AudioVisualizerCount;
                var audioVisualizerUnitLength = audioVisualizerLength / audioVisualizerCount;
                for (var i = audioVisualizerCount - 1; i >= 0; --i)
                {
                    var mainAudioVisualizerValue = audioVisualizerHeight * AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.MainAudio, i);
                    var inputAudioVisualizerValue = audioVisualizerHeight * AudioSystem.Instance.GetAudioVisualizerValue(AudioSystem.InputAudio, i);
                    if (mainAudioVisualizerValue > 0.0)
                    {
                        r.Set(audioVisualizerPosition0 + audioVisualizerUnitLength * i, audioVisualizerPosition1 + Configure.Instance.GetAudioVisualizerModifier(audioVisualizerHeight, mainAudioVisualizerValue), audioVisualizerUnitLength, mainAudioVisualizerValue);
                        targetSession.DrawRectangle(audioMainVisualizerPaint, null, r);
                    }
                    if (inputAudioVisualizerValue > 0.0)
                    {
                        r.Set(audioVisualizerPosition0 + audioVisualizerUnitLength * i, audioVisualizerPosition1 + Configure.Instance.GetAudioVisualizerModifier(audioVisualizerHeight, inputAudioVisualizerValue), audioVisualizerUnitLength, inputAudioVisualizerValue);
                        targetSession.DrawRectangle(audioInputVisualizerPaint, null, r);
                    }
                }
            }
        }
    }
}