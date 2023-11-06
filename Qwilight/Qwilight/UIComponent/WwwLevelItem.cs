using Qwilight.NoteFile;
using System.Windows.Media;

namespace Qwilight.UIComponent
{
    public sealed class WwwLevelItem
    {
        public Brush PointedPaint => BaseUI.Instance.LevelPaints[(int)LevelValue];

        public string LevelID { get; init; }

        public string NoteID { get; init; }

        public string Title { get; init; }

        public string Comment { get; init; }

        public string LevelText { get; init; }

        public BaseNoteFile.Level LevelValue { get; init; }

        public bool Handled { get; init; }

        public double Avatars { get; init; }

        public string AvatarCountContents => Avatars.ToString(LanguageSystem.Instance.AvatarCountContents);

        public override bool Equals(object obj) => obj is WwwLevelItem wwwLevelItem &&
            LevelID == wwwLevelItem.LevelID &&
            NoteID == wwwLevelItem.NoteID &&
            Title == wwwLevelItem.Title &&
            Comment == wwwLevelItem.Comment &&
            LevelText == wwwLevelItem.LevelText &&
            LevelValue == wwwLevelItem.LevelValue &&
            Handled == wwwLevelItem.Handled &&
            Avatars == wwwLevelItem.Avatars;

        public override int GetHashCode() => HashCode.Combine(LevelID, NoteID, Title, Comment, LevelText, LevelValue, Handled, Avatars);
    }
}
