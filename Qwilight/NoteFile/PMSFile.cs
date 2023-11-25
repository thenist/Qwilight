using Qwilight.UIComponent;

namespace Qwilight.NoteFile
{
    public sealed class PMSFile : BMSFile
    {
        public override int DataID => 1;

        public PMSFile(string noteFilePath, DefaultEntryItem defaultEntryItem, EntryItem entryItem) : base(noteFilePath, defaultEntryItem, entryItem)
        {
        }
    }
}