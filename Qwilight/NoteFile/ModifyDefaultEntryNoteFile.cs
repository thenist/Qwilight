using Qwilight.ViewModel;
using System.Windows.Media;

namespace Qwilight.NoteFile
{
    public sealed class ModifyDefaultEntryNoteFile : BaseNoteFile
    {
        public static readonly ModifyDefaultEntryNoteFile Instance = new();

        public override NoteVariety NoteVarietyValue => default;

        public override bool IsLogical => true;

        public override ImageSource LogicalDrawing => BaseUI.Instance.DefaultEntryConfigureDrawing;

        public override bool Equals(object obj) => obj is ModifyDefaultEntryNoteFile;

        public override int GetHashCode() => 0;

        ModifyDefaultEntryNoteFile() : base(default, default, default) => OnLevyNoteFile = new(() => ViewModels.Instance.ModifyDefaultEntryValue.Open());
    }
}