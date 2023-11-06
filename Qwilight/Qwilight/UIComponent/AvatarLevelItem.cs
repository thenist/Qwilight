using Qwilight.NoteFile;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public struct AvatarLevelItem
    {
        public string Title { get; init; }

        public string LevelText { get; init; }

        public BaseNoteFile.Level LevelValue { get; init; }

        public string Date { get; init; }

        public Brush LevelPaint => BaseUI.Instance.LevelPaints[(int)LevelValue];
    }
}
