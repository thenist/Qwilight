using Microsoft.Graphics.Canvas.Brushes;
using System.Collections.Concurrent;
using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight.UIComponent
{
    public struct AvatarTitle
    {
        public string TitleNBSP => string.IsNullOrEmpty(Title) ? string.Empty : $"{Title} ";

        public string Title { get; init; }

        public Brush TitlePaint { get; init; }

        public ICanvasBrush[] TitlePaints { get; init; }

        public AvatarTitle(string title, Brush titlePaint, Color titleColor)
        {
            Title = title;
            TitlePaint = titlePaint;
            TitlePaints = PoolSystem.Instance.GetFaintPaint(titleColor);
        }
    }
}
