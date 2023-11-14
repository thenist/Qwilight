using Qwilight.UIComponent;

namespace Qwilight.NoteFile
{
    public class BMSFile : BaseNoteFile
    {
        public override NoteVariety NoteVarietyValue { get; } = NoteVariety.BMS;

        public BMSFile(string noteFilePath, DefaultEntryItem defaultEntryItem, EntryItem entryItem) : base(noteFilePath, defaultEntryItem, entryItem)
        {
        }
    }
}