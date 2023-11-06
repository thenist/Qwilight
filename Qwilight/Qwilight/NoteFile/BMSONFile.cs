using Qwilight.UIComponent;

namespace Qwilight.NoteFile
{
    public sealed class BMSONFile : BaseNoteFile
    {
        public override NoteVariety NoteVarietyValue { get; } = NoteVariety.BMSON;

        public BMSONFile(string noteFilePath, DefaultEntryItem defaultEntryItem, EntryItem entryItem) : base(noteFilePath, defaultEntryItem, entryItem)
        {
        }
    }
}