using System.Windows.Media;

namespace Qwilight.NoteFile
{
    public sealed class SaltNoteFile : BaseNoteFile
    {
        public static readonly SaltNoteFile Instance = new();

        public override NoteVariety NoteVarietyValue => default;

        public override bool IsLogical => true;

        public override ImageSource LogicalDrawing => BaseUI.Instance.SaltDrawing;

        public override string LogicalAudioFileName => "Salt";

        public override bool Equals(object obj) => obj is SaltNoteFile;

        public override int GetHashCode() => 0;

        SaltNoteFile() : base(default, default, default)
        {
        }
    }
}