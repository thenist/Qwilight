using Windows.UI;
using Brush = System.Windows.Media.Brush;

namespace Qwilight.UIComponent
{
    public struct AvatarTitle
    {
        public string Title { get; init; }

        public Brush TitlePaint { get; init; }

        public Color TitleColor { get; init; }

        public AvatarTitle(string title, Brush titlePaint, Color titleColor)
        {
            Title = title;
            TitlePaint = titlePaint;
            TitleColor = titleColor;
        }
    }
}
